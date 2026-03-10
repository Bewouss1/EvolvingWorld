using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class MerchantSlotUI : MonoBehaviour
{
    [SerializeField] Image leftIcon;
    [SerializeField] TextMeshProUGUI leftLabel;
    [SerializeField] Image rightIcon;
    [SerializeField] TextMeshProUGUI rightLabel;
    [SerializeField] Button actionButton;
    [SerializeField] CanvasGroup canvasGroup;

    // Achat : joueur donne de l'or, reçoit l'item
    public void SetupBuy(ItemData item, int stock, int price, Sprite goldSprite, bool canDo, UnityAction onClick)
    {
        leftIcon.sprite = goldSprite;
        leftLabel.text = price.ToString();

        rightIcon.sprite = item.icon;
        rightLabel.text = stock > 0 ? $"{item.itemName}  x{stock}" : $"{item.itemName}  (Épuisé)";

        canvasGroup.alpha = canDo ? 1f : 0.5f;
        canvasGroup.blocksRaycasts = canDo;
        actionButton.onClick.AddListener(onClick);
    }

    // Vente : joueur donne l'item, reçoit de l'or
    public void SetupSell(ItemData item, int count, int price, Sprite goldSprite, bool playerHas, UnityAction onClick)
    {
        leftIcon.sprite = item.icon;
        leftLabel.text = count > 0 ? $"{item.itemName}  x{count}" : item.itemName;

        rightIcon.sprite = goldSprite;
        rightLabel.text = price.ToString();

        canvasGroup.alpha = playerHas ? 1f : 0.5f;
        canvasGroup.blocksRaycasts = playerHas;
        actionButton.onClick.AddListener(onClick);
    }
}
