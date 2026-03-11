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

    List<WorldEventData> warningEvents = new List<WorldEventData>();
    List<WorldEventData> activeEvents = new List<WorldEventData>();

    public IReadOnlyList<WorldEventData> ActiveEvents => activeEvents;
    public IReadOnlyList<WorldEventData> WarningEvents => warningEvents;

    public event Action OnEventsChanged;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    void Start()
    {
        foreach (var evt in scheduledEvents)
        {
            if (evt != null)
                ScheduleEvent(evt);
        }
    }

    /// <summary>
    /// Programme un événement : avertissement → actif → fin.
    /// </summary>
    public void ScheduleEvent(WorldEventData eventData)
    {
        StartCoroutine(EventRoutine(eventData));
    }

    IEnumerator EventRoutine(WorldEventData eventData)
    {
        // Attendre avant la phase d'avertissement
        if (eventData.warningDelay > 0f)
            yield return new WaitForSeconds(eventData.warningDelay);

        // Phase avertissement (indices visibles, prix inchangés)
        StartWarning(eventData);

        // Attendre avant l'activation des prix
        if (eventData.activeDelay > 0f)
            yield return new WaitForSeconds(eventData.activeDelay);

        // Phase active (prix modifiés)
        ActivateEvent(eventData);

        // Attendre la durée de la phase active
        if (eventData.duration > 0f)
        {
            yield return new WaitForSeconds(eventData.duration);
            DeactivateEvent(eventData);
        }
    }

    public void StartWarning(WorldEventData eventData)
    {
        if (warningEvents.Contains(eventData)) return;
        warningEvents.Add(eventData);
        OnEventsChanged?.Invoke();
    }

    public void ActivateEvent(WorldEventData eventData)
    {
        warningEvents.Remove(eventData);
        if (activeEvents.Contains(eventData)) return;
        activeEvents.Add(eventData);
        OnEventsChanged?.Invoke();
    }

    public void DeactivateEvent(WorldEventData eventData)
    {
        warningEvents.Remove(eventData);
        if (!activeEvents.Remove(eventData)) return;
        OnEventsChanged?.Invoke();
    }

    public bool IsWarning(WorldEventData eventData)
    {
        return warningEvents.Contains(eventData);
    }

    public bool IsActive(WorldEventData eventData)
    {
        return activeEvents.Contains(eventData);
    }

    /// <summary>
    /// Retourne le prix modifié d'un item en cumulant les événements actifs (pas les avertissements).
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
}
