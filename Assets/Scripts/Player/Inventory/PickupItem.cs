using UnityEngine;
using DS;

/// <summary>
/// À placer sur un GameObject ramassable dans la scène.
/// Nécessite un Collider en mode Trigger.
/// </summary>
/// 
[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private ItemData item;
    [SerializeField] private int quantity = 1;

    [Header("Feedback")]
    [SerializeField] private GameObject pickupVFX;  

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        InventorySystem inventory = other.GetComponent<InventorySystem>();
        if (inventory == null) return;

        int leftover = inventory.TryAdd(item, quantity);

        if (leftover < quantity) 
        {
            string flagName = "Has_" + item.itemName;
            DSDialogueFlags.Set(flagName, true);

            if (pickupVFX != null)
                Instantiate(pickupVFX, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}