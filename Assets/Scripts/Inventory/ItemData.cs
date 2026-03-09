using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea(1, 3)]
    public string description;
    public Sprite icon;
    public int maxStack = 99;
    public int buyPrice;
    public int sellPrice;
}
