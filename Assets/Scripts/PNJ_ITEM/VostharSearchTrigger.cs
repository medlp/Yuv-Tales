using UnityEngine;
using DS;

public class VostharSearchTrigger : MonoBehaviour
{
    [Tooltip("Assigner l'asset ItemData de \"Master's Work\"")]
    [SerializeField] private ItemData mastersWorkItem;

    [SerializeField] private string flagToSet = "SearchVosthar";

    private InventoryUI inventoryUI;

    private void Start()
    {
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        if (inventoryUI == null)
        {
            Debug.LogError("[VostharSearchTrigger] InventoryUI introuvable dans la scene.");
            return;
        }

        inventoryUI.OnItemClicked += HandleItemClicked;
    }

    private void OnDestroy()
    {
        if (inventoryUI != null)
            inventoryUI.OnItemClicked -= HandleItemClicked;
    }

    private void HandleItemClicked(ItemData item)
    {
        if (item == mastersWorkItem)
            DSDialogueFlags.Set(flagToSet, true);
    }
}