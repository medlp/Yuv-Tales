using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
/// <summary>
/// Gère l'affichage de l'inventaire. 
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotsParent;

    [Header("Description UI")]
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;

    [Header("Parchemin UI")]
    [SerializeField] private Image parchmentImage;

    private InventorySystem inventorySystem;
    private List<InventorySlotUI> slotUIs = new();
    private bool isOpen = false;



    void Start()
    { 
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) { Debug.LogError("[InventoryUI] Player introuvable."); return; }

        inventorySystem = player.GetComponent<InventorySystem>();
        if (inventorySystem == null) { Debug.LogError("[InventoryUI] InventorySystem introuvable sur Player."); return; }

        BuildGrid();
         
        inventorySystem.OnSlotChanged += RefreshSlot;
        inventorySystem.OnInventoryFull += OnFull;

        panel.SetActive(false);
        ClearDescription();
    }

    void OnDestroy()
    {
        if (inventorySystem == null) return;
        inventorySystem.OnSlotChanged -= RefreshSlot;
        inventorySystem.OnInventoryFull -= OnFull;

        foreach (var slotUI in slotUIs)
        {
            slotUI.OnSlotClicked -= UpdateDescriptionPanel;
        }
    }
     
    public void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        isOpen = !isOpen;
        panel.SetActive(isOpen);

        if (!isOpen) ClearDescription();

        GameManager gm = GameManager.Instance;

        if (gm.CurrentState == GameState.Gameplay)
            gm.UpdateState(GameState.Inventory);
        else if (gm.CurrentState == GameState.Inventory)
            gm.UpdateState(GameState.Gameplay);
    }

    private void BuildGrid()
    {
        foreach (Transform child in slotsParent)
            Destroy(child.gameObject);

        slotUIs.Clear();

        for (int i = 0; i < inventorySystem.Capacity; i++)
        {
            InventorySlotUI slotUI = Instantiate(slotPrefab, slotsParent);
            slotUI.Refresh(inventorySystem.Slots[i]);

            slotUI.OnSlotClicked += UpdateDescriptionPanel;

            slotUIs.Add(slotUI);
        }
    }

    private void UpdateDescriptionPanel(InventorySlot slot)
    {
        if (slot == null) return;

        if(slot.IsEmpty)
        {
            ClearDescription();
            return;
        }

        if (image != null) image.enabled = true;
        if (itemNameText != null) itemNameText.text = slot.item.itemName; 
        if (itemDescriptionText != null) itemDescriptionText.text = slot.item.description;


        ParchmentItemData parchmentItemData = slot.item as ParchmentItemData;
        bool parchment = false;
        if (parchmentItemData != null)
        {
            parchment = parchmentItemData.parchmentImage != null;
        
            parchmentImage.enabled = parchment;
            if (parchment) 
                parchmentImage.sprite = parchmentItemData.parchmentImage;
        }

    }
    private void ClearDescription()
    {
        if (itemNameText != null) itemNameText.text = "";
        if (itemDescriptionText != null) itemDescriptionText.text = "";
        if (parchmentImage != null) parchmentImage.enabled = false;
        if (image != null) image.enabled = false;
    }

    private void RefreshSlot(InventorySlot slot, int index)
    {
        if (index >= 0 && index < slotUIs.Count)
            slotUIs[index].Refresh(slot);
    }

    private void OnFull()
    {
        Debug.Log("[UI] Inventaire plein — afficher un feedback ici."); 
    }
}