using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int slotCount = 16;

    InventorySlot[] slots;
    int gold;

    public int SlotCount => slotCount;
    public int Gold => gold;

    public event Action OnChanged;

    void Awake()
    {
        slots = new InventorySlot[slotCount];
        for (int i = 0; i < slotCount; i++)
            slots[i] = new InventorySlot();
    }

    public InventorySlot GetSlot(int index) => slots[index];

    // Ajouter un item. Retourne la quantité qui n'a pas pu être ajoutée (0 = tout ajouté)
    public int AddItem(ItemData item, int amount = 1)
    {
        // D'abord remplir les stacks existants
        for (int i = 0; i < slotCount && amount > 0; i++)
        {
            if (slots[i].item == item && slots[i].count < item.maxStack)
            {
                int space = item.maxStack - slots[i].count;
                int toAdd = Mathf.Min(space, amount);
                slots[i].count += toAdd;
                amount -= toAdd;
            }
        }

        // Ensuite utiliser les slots vides
        for (int i = 0; i < slotCount && amount > 0; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = item;
                int toAdd = Mathf.Min(item.maxStack, amount);
                slots[i].count = toAdd;
                amount -= toAdd;
            }
        }

        OnChanged?.Invoke();
        return amount;
    }

    // Retirer un item. Retourne true si la quantité a pu être retirée
    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (CountItem(item) < amount) return false;

        for (int i = slotCount - 1; i >= 0 && amount > 0; i--)
        {
            if (slots[i].item == item)
            {
                int toRemove = Mathf.Min(slots[i].count, amount);
                slots[i].count -= toRemove;
                amount -= toRemove;
                if (slots[i].count <= 0)
                {
                    slots[i].item = null;
                    slots[i].count = 0;
                }
            }
        }

        OnChanged?.Invoke();
        return true;
    }

    public int CountItem(ItemData item)
    {
        int total = 0;
        for (int i = 0; i < slotCount; i++)
            if (slots[i].item == item)
                total += slots[i].count;
        return total;
    }

    // Échanger deux slots (pour le drag & drop)
    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA == indexB) return;

        // Si c'est le même item, essayer de fusionner
        if (slots[indexA].item != null && slots[indexA].item == slots[indexB].item)
        {
            int space = slots[indexA].item.maxStack - slots[indexB].count;
            int toMove = Mathf.Min(space, slots[indexA].count);
            slots[indexB].count += toMove;
            slots[indexA].count -= toMove;
            if (slots[indexA].count <= 0)
            {
                slots[indexA].item = null;
                slots[indexA].count = 0;
            }
        }
        else
        {
            (slots[indexA], slots[indexB]) = (slots[indexB], slots[indexA]);
        }

        OnChanged?.Invoke();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        OnChanged?.Invoke();
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        OnChanged?.Invoke();
        return true;
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int count;
}
