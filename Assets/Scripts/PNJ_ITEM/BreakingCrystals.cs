using DS;
using UnityEngine;

public class BreakingCrystals : MonoBehaviour
{
    [Header("Flag Settings")]
    [Tooltip("Nom du flag DS requis pour casser le cristal au lieu de lancer le dialogue")]
    public string requiredFlag = "Has_Pickaxe";


    public bool TryInterceptInteraction()
    {
        if (DSDialogueFlags.Get(requiredFlag))
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }

}
