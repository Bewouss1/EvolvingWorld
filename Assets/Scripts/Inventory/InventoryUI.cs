using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Inventaire")]
    [SerializeField] private Inventory inventory;

    InventorySlotUI[] slotUIs;
    CustomActions input;
    System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> onInventoryPerformed;

    public bool IsOpen => inventoryPanel.activeSelf;

    void Awake()
    {
        input = new CustomActions();
        onInventoryPerformed = ctx => ToggleInventory();
        inventoryPanel.SetActive(false);
    }

    void OnEnable()
    {
        input.Enable();
        input.Main.Inventory.performed += onInventoryPerformed;

        if (inventory != null)
            inventory.OnChanged += RefreshUI;
    }

    void OnDisable()
    {
        input.Main.Inventory.performed -= onInventoryPerformed;
        input.Disable();

        if (inventory != null)
            inventory.OnChanged -= RefreshUI;
    }

    void Start()
    {
        InitSlots();
    }

    void InitSlots()
    {
        slotUIs = new InventorySlotUI[inventory.SlotCount];
        for (int i = 0; i < inventory.SlotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
            slotUIs[i] = slotObj.GetComponent<InventorySlotUI>();
            slotUIs[i].Init(i, this);
        }
        RefreshUI();
    }

    void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (IsOpen)
            RefreshUI();
    }

    public void RefreshUI()
    {
        if (slotUIs == null) return;
        for (int i = 0; i < slotUIs.Length; i++)
        {
            InventorySlot slot = inventory.GetSlot(i);
            slotUIs[i].UpdateSlot(slot);
        }
        goldText.text = inventory.Gold.ToString();
    }

    public void OnSlotDropped(int fromIndex, int toIndex)
    {
        inventory.SwapSlots(fromIndex, toIndex);
    }
}
