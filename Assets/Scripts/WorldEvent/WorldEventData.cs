using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldEvent", menuName = "WorldEvent/WorldEventData")]
public class WorldEventData : ScriptableObject
{
    public string eventName;
    [TextArea(2, 4)]
    public string description;

    [Header("Timing (en secondes)")]
    [Tooltip("Délai avant que l'événement se déclenche")]
    public float delay;
    [Tooltip("Durée de l'événement une fois actif")]
    public float duration;

    [Header("Modificateurs de prix")]
    public PriceModifier[] priceModifiers;

    [Header("Effets visuels sur PNJ")]
    public GameObject vfxPrefab;
    [Tooltip("Position du VFX par rapport au PNJ")]
    public Vector3 vfxOffset = Vector3.up * 2f;
    [Tooltip("Rôles des PNJ affectés par le VFX")]
    public NpcRole[] affectedNpcRoles;
}

[System.Serializable]
public class PriceModifier
{
    public ItemData item;
    public float buyMultiplier = 1f;
    public float sellMultiplier = 1f;
}
