using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gère l'affichage de l'inventaire. 
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotsParent;

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
    }

    void OnDestroy()
    {
        if (inventorySystem == null) return;
        inventorySystem.OnSlotChanged -= RefreshSlot;
        inventorySystem.OnInventoryFull -= OnFull;
    }
     
    public void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        isOpen = !isOpen;
        panel.SetActive(isOpen);
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
            slotUIs.Add(slotUI);
        }
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