using UnityEngine;

public class NpcIdentity : MonoBehaviour
{
    [Tooltip("Rôles du PNJ (sélection dans la liste)")]
    [SerializeField] NpcRole[] roles;

    public bool HasRole(NpcRole role)
    {
        foreach (NpcRole r in roles)
        {
            if (r == role)
                return true;
        }
        return false;
    }
}
