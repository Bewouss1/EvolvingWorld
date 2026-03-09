using UnityEngine;

// Script temporaire pour tester l'inventaire. À supprimer ensuite.
public class InventoryTest : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemData[] testItems;
    [SerializeField] private int[] testAmounts;

    void Start()
    {
        for (int i = 0; i < testItems.Length; i++)
        {
            int amount = (i < testAmounts.Length) ? testAmounts[i] : 1;
            inventory.AddItem(testItems[i], amount);
        }

        inventory.AddGold(150);
    }
}
