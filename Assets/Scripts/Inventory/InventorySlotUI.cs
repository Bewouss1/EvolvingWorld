using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    int slotIndex;
    InventoryUI inventoryUI;

    // Drag
    static InventorySlotUI draggedSlot;
    static GameObject dragIcon;
    Canvas rootCanvas;

    public void Init(int index, InventoryUI ui)
    {
        slotIndex = index;
        inventoryUI = ui;
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void UpdateSlot(InventorySlot slot)
    {
        if (slot.item != null)
        {
            iconImage.sprite = slot.item.icon;
            iconImage.color = Color.white;
            countText.text = slot.count > 1 ? slot.count.ToString() : "";
        }
        else
        {
            iconImage.sprite = null;
            iconImage.color = Color.white;
            countText.text = "";
        }
    }

    // === Drag & Drop ===

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (iconImage.sprite == null) return;

        draggedSlot = this;

        // Créer une icône qui suit la souris
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(rootCanvas.transform, false);
        var img = dragIcon.AddComponent<Image>();
        img.sprite = iconImage.sprite;
        img.raycastTarget = false;
        var rect = dragIcon.GetComponent<RectTransform>();
        rect.sizeDelta = iconImage.rectTransform.rect.size;

        // Rendre le slot semi-transparent
        iconImage.color = new Color(1, 1, 1, 0.3f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null) return;
        dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            Destroy(dragIcon);

        draggedSlot = null;

        // Rafraîchir pour restaurer l'affichage
        inventoryUI.RefreshUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggedSlot == null || draggedSlot == this) return;
        inventoryUI.OnSlotDropped(draggedSlot.slotIndex, slotIndex);
    }
}
