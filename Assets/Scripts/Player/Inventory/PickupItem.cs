using UnityEngine;
using DS;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] private ItemData item;
    [SerializeField] private int quantity = 1;

    [Header("Feedback")]
    [SerializeField] private GameObject pickupVFX;

    private string pickupID;
    public string PickupID => pickupID;

    public string InteractionPrompt => $"Take {item.itemName}";

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        pickupID = gameObject.name + "_" + transform.position.ToString("F2");
    }

    public bool Interact(InventorySystem inventory)
    {
        if (inventory == null) return false;

        int leftover = inventory.TryAdd(item, quantity);

        if (leftover < quantity)
        {
            string flagName = "Has_" + item.itemName;
            DSDialogueFlags.Set(flagName, true);

            string pickupFlag = "PickedUp_" + pickupID;
            DSDialogueFlags.Set(pickupFlag, true);

            if (pickupVFX != null)
                Instantiate(pickupVFX, transform.position, Quaternion.identity);

            Destroy(gameObject);
            return true;
        }

        return false;
    }
}