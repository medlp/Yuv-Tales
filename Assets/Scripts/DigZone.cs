using UnityEngine;

public class DigZone : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Material dugMaterial; 

    [Header("Reward Settings")]
    [SerializeField] private ItemData itemToGive;   
    [SerializeField] private int quantity = 1;

    private bool isAlreadyDug = false;
    public bool IsAlreadyDug => isAlreadyDug;

    public void OnDigComplete()
    {
        if (isAlreadyDug) return;

        isAlreadyDug = true;

        if (GetComponent<Renderer>() != null && dugMaterial != null)
        {
            GetComponent<Renderer>().material = dugMaterial;
        }

        GiveRewardToPlayer();

        Debug.Log("La zone a été fouillée avec succès !");
    }

    private void GiveRewardToPlayer()
    {
        if (itemToGive == null) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            InventorySystem inventory = player.GetComponent<InventorySystem>();
            if (inventory != null)
            {
                inventory.TryAdd(itemToGive, quantity);
            }
            else
            {
                Debug.LogWarning("[DigZone] L'inventaire (InventorySystem) est introuvable sur le Player.");
            }
        }
        else
        {
            Debug.LogWarning("[DigZone] Impossible de donner l'item : aucun GameObject avec le Tag 'Player' n'a été trouvé.");
        }
    }
}

