using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MerchantUI : MonoBehaviour
{
    static MerchantUI instance;
    public static MerchantUI Instance => instance;

    [Header("UI")]
    [SerializeField] GameObject merchantPanel;
    [SerializeField] Transform slotsContainer;
    [SerializeField] GameObject merchantSlotPrefab;
    [SerializeField] Button buyTabButton;
    [SerializeField] Button sellTabButton;
    [SerializeField] Button closeButton;
    [SerializeField] Sprite goldSprite;
    [SerializeField] TextMeshProUGUI goldText;

    [Header("Références")]
    [SerializeField] Inventory playerInventory;

    MerchantData currentData;
    NPCMerchant currentMerchant;
    bool showingBuyTab = true;

    public bool IsOpen => merchantPanel.activeSelf;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        merchantPanel.SetActive(false);
    }

    void Start()
    {
        buyTabButton.onClick.AddListener(() => ShowTab(true));
        sellTabButton.onClick.AddListener(() => ShowTab(false));
        closeButton.onClick.AddListener(CloseShop);
        playerInventory.OnChanged += RefreshGold;
        RefreshGold();
    }

    void RefreshGold()
    {
        if (goldText != null)
            goldText.text = playerInventory.Gold.ToString();
    }

    void Update()
    {
        if (IsOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseShop();
    }

    public void OpenShop(NPCMerchant merchant, MerchantData data)
    {
        currentMerchant = merchant;
        currentData = data;
        merchantPanel.SetActive(true);
        ShowTab(true);
    }

    public void CloseShop()
    {
        merchantPanel.SetActive(false);
        ClearSlots();
        currentMerchant = null;
        currentData = null;
    }

    void ShowTab(bool buyTab)
    {
        showingBuyTab = buyTab;
        ClearSlots();

        if (buyTab) RefreshBuyTab();
        else RefreshSellTab();
    }

    void RefreshBuyTab()
    {
        for (int i = 0; i < currentData.forSale.Length; i++)
        {
            int index = i;
            MerchantOffer offer = currentData.forSale[i];
            int stock = currentMerchant.GetStock(i);
            int price = WorldEventManager.Instance != null
                ? WorldEventManager.Instance.GetModifiedPrice(offer.item, true)
                : offer.item.buyPrice;
            bool canDo = stock > 0 && playerInventory.Gold >= price;

            var slotObj = Instantiate(merchantSlotPrefab, slotsContainer);
            slotObj.GetComponent<MerchantSlotUI>().SetupBuy(
                offer.item, stock, price, goldSprite, canDo, () => OnBuy(index));
        }
    }

    void RefreshSellTab()
    {
        foreach (ItemData item in currentData.wantsToBuy)
        {
            int count = playerInventory.CountItem(item);
            int price = WorldEventManager.Instance != null
                ? WorldEventManager.Instance.GetModifiedPrice(item, false)
                : item.sellPrice;
            var slotObj = Instantiate(merchantSlotPrefab, slotsContainer);
            slotObj.GetComponent<MerchantSlotUI>().SetupSell(
                item, count, price, goldSprite, count > 0, () => OnSell(item));
        }
    }

    void OnBuy(int offerIndex)
    {
        MerchantOffer offer = currentData.forSale[offerIndex];
        if (currentMerchant.GetStock(offerIndex) <= 0) return;

        int price = WorldEventManager.Instance != null
            ? WorldEventManager.Instance.GetModifiedPrice(offer.item, true)
            : offer.item.buyPrice;

        if (!playerInventory.SpendGold(price)) return;

        int leftover = playerInventory.AddItem(offer.item, 1);
        if (leftover > 0)
        {
            // Inventaire plein, remboursement
            playerInventory.AddGold(price);
            return;
        }

        currentMerchant.DecrementStock(offerIndex);
        ShowTab(true);
    }

    void OnSell(ItemData item)
    {
        if (!playerInventory.RemoveItem(item, 1)) return;

        int price = WorldEventManager.Instance != null
            ? WorldEventManager.Instance.GetModifiedPrice(item, false)
            : item.sellPrice;

        playerInventory.AddGold(price);
        ShowTab(false);
    }

    void ClearSlots()
    {
        foreach (Transform child in slotsContainer)
            Destroy(child.gameObject);
    }
}
