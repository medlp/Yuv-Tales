using TMPro;
using UnityEngine;
using UnityEngine.UI;
 
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private GameObject emptyOverlay; 

    public void Refresh(InventorySlot slot)
    {
        bool hasItem = slot != null && !slot.IsEmpty;

        iconImage.enabled = hasItem;
        quantityText.enabled = hasItem;
        if (emptyOverlay != null) emptyOverlay.SetActive(!hasItem);

        if (!hasItem) return;

        iconImage.sprite = slot.item.icon;
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
    }
}