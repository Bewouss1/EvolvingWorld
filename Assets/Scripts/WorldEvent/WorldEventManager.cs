using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventManager : MonoBehaviour
{
    static WorldEventManager instance;
    public static WorldEventManager Instance => instance;

    [Header("Événements programmés au lancement")]
    [SerializeField] WorldEventData[] scheduledEvents;

    // Événements actifs et leurs VFX instanciés
    List<WorldEventData> activeEvents = new List<WorldEventData>();
    Dictionary<WorldEventData, List<GameObject>> activeVfx = new Dictionary<WorldEventData, List<GameObject>>();

    public IReadOnlyList<WorldEventData> ActiveEvents => activeEvents;

    public event Action OnEventsChanged;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    void Start()
    {
        // Lancer les événements configurés dans l'Inspector
        foreach (var evt in scheduledEvents)
        {
            if (evt != null)
                ScheduleEvent(evt);
        }
    }

    /// <summary>
    /// Programme un événement : attend le délai, l'active, puis le désactive après sa durée.
    /// </summary>
    public void ScheduleEvent(WorldEventData eventData)
    {
        StartCoroutine(EventRoutine(eventData));
    }

    IEnumerator EventRoutine(WorldEventData eventData)
    {
        // Attendre le délai
        if (eventData.delay > 0f)
            yield return new WaitForSeconds(eventData.delay);

        ActivateEvent(eventData);

        // Attendre la durée
        if (eventData.duration > 0f)
        {
            yield return new WaitForSeconds(eventData.duration);
            DeactivateEvent(eventData);
        }
    }

    /// <summary>
    /// Active un événement immédiatement (sans passer par le timer).
    /// </summary>
    public void ActivateEvent(WorldEventData eventData)
    {
        if (activeEvents.Contains(eventData)) return;

        activeEvents.Add(eventData);
        SpawnVfx(eventData);
        OnEventsChanged?.Invoke();
    }

    public void DeactivateEvent(WorldEventData eventData)
    {
        if (!activeEvents.Remove(eventData)) return;

        DespawnVfx(eventData);
        OnEventsChanged?.Invoke();
    }

    public bool IsActive(WorldEventData eventData)
    {
        return activeEvents.Contains(eventData);
    }

    /// <summary>
    /// Retourne le prix modifié d'un item en cumulant tous les événements actifs.
    /// </summary>
    public int GetModifiedPrice(ItemData item, bool isBuy)
    {
        int basePrice = isBuy ? item.buyPrice : item.sellPrice;
        float multiplier = 1f;

        foreach (var evt in activeEvents)
        {
            foreach (var mod in evt.priceModifiers)
            {
                if (mod.item == item)
                    multiplier *= isBuy ? mod.buyMultiplier : mod.sellMultiplier;
            }
        }

        return Mathf.Max(1, Mathf.RoundToInt(basePrice * multiplier));
    }

    // === VFX ===

    void SpawnVfx(WorldEventData eventData)
    {
        if (eventData.vfxPrefab == null || eventData.affectedNpcRoles == null) return;

        var vfxList = new List<GameObject>();
        var allNpcs = FindObjectsByType<NpcIdentity>(FindObjectsSortMode.None);

        foreach (var npc in allNpcs)
        {
            foreach (NpcRole role in eventData.affectedNpcRoles)
            {
                if (npc.HasRole(role))
                {
                    var vfx = Instantiate(eventData.vfxPrefab, npc.transform);
                    vfx.transform.localPosition = eventData.vfxOffset;
                    vfxList.Add(vfx);
                    break; // Un seul VFX par PNJ même s'il matche plusieurs rôles
                }
            }
        }

        activeVfx[eventData] = vfxList;
    }

    void DespawnVfx(WorldEventData eventData)
    {
        if (!activeVfx.TryGetValue(eventData, out var vfxList)) return;

        foreach (var vfx in vfxList)
        {
            if (vfx != null) Destroy(vfx);
        }

        activeVfx.Remove(eventData);
    }
}
