using UnityEngine;

[CreateAssetMenu(fileName = "NewMerchant", menuName = "Merchant/MerchantData")]
public class MerchantData : ScriptableObject
{
    public MerchantOffer[] forSale;
    public ItemData[] wantsToBuy;
}

[System.Serializable]
public class MerchantOffer
{
    public ItemData item;
    public int stock;
}
