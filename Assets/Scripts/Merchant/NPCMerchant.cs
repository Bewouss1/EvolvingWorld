using UnityEngine;

public class NPCMerchant : MonoBehaviour, IInteractable
{
    [SerializeField] float interactionDistance = 3f;
    [SerializeField] MerchantData merchantData;

    public float InteractionDistance => interactionDistance;

    // Stock runtime : copie des données SO pour ne pas modifier l'asset
    int[] runtimeStock;

    void Awake()
    {
        if (merchantData == null) return;
        runtimeStock = new int[merchantData.forSale.Length];
        for (int i = 0; i < merchantData.forSale.Length; i++)
            runtimeStock[i] = merchantData.forSale[i].stock;
    }

    public int GetStock(int index)
    {
        if (runtimeStock == null || index >= runtimeStock.Length) return 0;
        return runtimeStock[index];
    }

    public void DecrementStock(int index)
    {
        if (runtimeStock == null || index >= runtimeStock.Length) return;
        runtimeStock[index] = Mathf.Max(0, runtimeStock[index] - 1);
    }

    public void Interact()
    {
        if (merchantData == null) return;
        MerchantUI.Instance.OpenShop(this, merchantData);
    }
}
