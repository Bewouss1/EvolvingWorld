using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldEvent", menuName = "WorldEvent/WorldEventData")]
public class WorldEventData : ScriptableObject
{
    public string eventName;
    [TextArea(2, 4)]
    public string description;

    [Header("Timing (en secondes)")]
    [Tooltip("Délai avant que les indices apparaissent (phase avertissement)")]
    public float warningDelay;
    [Tooltip("Durée de la phase d'avertissement avant que les prix changent")]
    public float activeDelay;
    [Tooltip("Durée de la phase active (prix modifiés)")]
    public float duration;

    [Header("Modificateurs de prix")]
    public PriceModifier[] priceModifiers;
}

[System.Serializable]
public class PriceModifier
{
    public ItemData item;
    public float buyMultiplier = 1f;
    public float sellMultiplier = 1f;
}
