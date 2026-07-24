using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gère les données de l'inventaire du joueur. 
/// </summary>
public class InventorySystem : MonoBehaviour
{
    [Header("Capacité")]
    [SerializeField] private int capacity = 20;
     
    public event Action<InventorySlot, int> OnSlotChanged;    
    public event Action OnInventoryFull;

    private List<InventorySlot> slots = new();

    public IReadOnlyList<InventorySlot> Slots => slots;
    public int Capacity => capacity;

    void Awake()
    { 
        for (int i = 0; i < capacity; i++)
            slots.Add(new InventorySlot(null, 0));
    }

    /// <summary>
    /// Tente d'ajouter un item. Retourne la quantité qui n'a pas pu être ajoutée.
    /// </summary>
    public int TryAdd(ItemData item, int quantity = 1)
    {
        if (item == null) return quantity;

        int remaining = quantity;
         
        if (item.isStackable)
        {
            for (int i = 0; i < slots.Count && remaining > 0; i++)
            {
                if (slots[i].item == item && slots[i].quantity < item.maxStack)
                {
                    int space = item.maxStack - slots[i].quantity;
                    int added = Mathf.Min(space, remaining);

                    slots[i].quantity += added;
                    remaining -= added;

                    OnSlotChanged?.Invoke(slots[i], i);
                }
            }
        }

        while (remaining > 0)
        {
            int emptyIndex = slots.FindIndex(s => s.IsEmpty);

            if (emptyIndex >= 0)
            {
                int maxAllowedInSlot = item.isStackable ? item.maxStack : 1;
                int toPlace = Mathf.Min(remaining, maxAllowedInSlot);

                slots[emptyIndex] = new InventorySlot(item, toPlace);
                remaining -= toPlace;

                OnSlotChanged?.Invoke(slots[emptyIndex], emptyIndex);
            }
            else
            {
                OnInventoryFull?.Invoke();
                break; 
            }
        }

        return remaining; // 0 = tout ajouté
    }

    /// <summary>
    /// Retire une quantité d'un slot précis.
    /// </summary>
    public bool TryRemoveAt(int index, int quantity = 1)
    {
        if (index < 0 || index >= slots.Count) return false;
        if (slots[index].IsEmpty || slots[index].quantity < quantity) return false;

        slots[index].quantity -= quantity;

        if (slots[index].quantity <= 0)
            slots[index] = new InventorySlot(null, 0);

        OnSlotChanged?.Invoke(slots[index], index);
        return true;
    }

    /// <summary>
    /// Tente de retirer une quantité d'un item, peu importe le(s) slot(s) où il se trouve.
    /// Échoue sans rien modifier si la quantité totale n'est pas disponible.
    /// </summary>
    public bool TryRemove(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;
        if (!Has(item, quantity)) return false;

        int remaining = quantity;

        for (int i = 0; i < slots.Count && remaining > 0; i++)
        {
            if (slots[i].IsEmpty || slots[i].item != item) continue;

            int toRemove = Mathf.Min(slots[i].quantity, remaining);
            slots[i].quantity -= toRemove;
            remaining -= toRemove;

            if (slots[i].quantity <= 0)
                slots[i] = new InventorySlot(null, 0);

            OnSlotChanged?.Invoke(slots[i], i);
        }

        return true;
    }

    /// <summary>
    /// Vérifie si le joueur possède au moins N exemplaires d'un item.
    /// </summary>
    public bool Has(ItemData item, int quantity = 1)
    {
        int total = 0;
        foreach (var slot in slots)
            if (slot.item == item) total += slot.quantity;
        return total >= quantity;
    }

    // SAVE SYSTEM  

    public List<InventorySlotSaveData> ExtractSaveData()
    {
        List<InventorySlotSaveData> result = new List<InventorySlotSaveData>(slots.Count);

        foreach(var slot in slots)
        {
            if (slot.IsEmpty)
                result.Add(new InventorySlotSaveData("", 0));
            else
                result.Add(new InventorySlotSaveData(slot.item.name, slot.quantity));
        }

        return result;
    }

    public void ApplySaveData(List<InventorySlotSaveData> savedSlots)
    {
        if (savedSlots == null) return;

        slots.Clear();

        for(int i = 0; i < capacity; i++)
        {
            if (i < savedSlots.Count && !String.IsNullOrEmpty("Items/" + savedSlots[i].itemName))   
            {
                ItemData resolvedItem = Resources.Load<ItemData>("Items/" +  savedSlots[i].itemName);

                if(resolvedItem == null)
                {
                    slots.Add(new InventorySlot(null, 0));
                    continue;
                }

                InventorySlot newSlot = new InventorySlot(resolvedItem, savedSlots[i].quantity);
                slots.Add(newSlot);
                OnSlotChanged?.Invoke(newSlot, i);
            }
            else
            {
                InventorySlot emptySlot = new InventorySlot(null, 0);
                slots.Add(emptySlot);
                OnSlotChanged?.Invoke(emptySlot, i);
            }
        }
    }

    /// <summary>
    /// Vide entièrement l'inventaire (nouvelle partie / slot sans save existante).
    /// </summary>
    public void ResetToEmpty()
    {
        slots.Clear();

        for (int i = 0; i < capacity; i++)
        {
            InventorySlot emptySlot = new InventorySlot(null, 0);
            slots.Add(emptySlot);
            OnSlotChanged?.Invoke(emptySlot, i);
        }
    }
}