using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
 
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text quantityText;

    [SerializeField] private GameObject emptyOverlay;
    [SerializeField] private Button clickButton;

    private InventorySlot currentSlot;
    public event Action<InventorySlot> OnSlotClicked;
    private void Awake()
    {
        if (clickButton != null)
        {
            clickButton.onClick.AddListener(() => OnSlotClicked?.Invoke(currentSlot));
        }
    }

    public void Refresh(InventorySlot slot)
    {
        currentSlot = slot;
        bool hasItem = slot != null && !slot.IsEmpty;

        iconImage.enabled = hasItem;
        quantityText.enabled = hasItem;
        if (emptyOverlay != null) emptyOverlay.SetActive(!hasItem);

        if (clickButton != null) clickButton.interactable = hasItem;

        if (!hasItem) return;

        iconImage.sprite = slot.item.icon;
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
    }
}