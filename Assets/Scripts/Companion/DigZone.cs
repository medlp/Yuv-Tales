using UnityEngine;

public class DigZone : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Material dugMaterial; 

    [Header("Reward Settings")]
    [SerializeField] private ItemData itemToGive;   
    [SerializeField] private int quantity = 1;

    [Header("Save Settings")]
    [Tooltip("Identifiant unique pour la sauvegarde. Par défaut, le nom du GameObject.")]
    [SerializeField] private string zoneID;

    private bool isAlreadyDug = false;
    public bool IsAlreadyDug => isAlreadyDug;
    public string ZoneID => string.IsNullOrEmpty(zoneID) ? gameObject.name : zoneID;

    private void Awake()
    {
        if(string.IsNullOrEmpty(zoneID))
            zoneID = gameObject.name;
    }

    public void OnDigComplete()
    {
        if (isAlreadyDug) return;

        isAlreadyDug = true;

        ApplyDugVisual();

        GiveRewardToPlayer();
    }

    public void RestoreDugState()
    {
        if (isAlreadyDug) return;

        isAlreadyDug = true;
        ApplyDugVisual();
    }

    private void ApplyDugVisual()
    {
        if (GetComponent<Renderer>() != null && dugMaterial != null)
        {
            GetComponent<Renderer>().material = dugMaterial;
        }
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
        }
    }
}

