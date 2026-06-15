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
         
        if (remaining > 0)
        {
            int emptyIndex = slots.FindIndex(s => s.IsEmpty);

            if (emptyIndex >= 0)
            {
                int toPlace = Mathf.Min(remaining, item.maxStack);
                slots[emptyIndex] = new InventorySlot(item, toPlace);
                remaining -= toPlace;

                OnSlotChanged?.Invoke(slots[emptyIndex], emptyIndex);
            }
            else
            {
                OnInventoryFull?.Invoke();
                Debug.LogWarning("[Inventory] Inventaire plein !");
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
    /// Vérifie si le joueur possède au moins N exemplaires d'un item.
    /// </summary>
    public bool Has(ItemData item, int quantity = 1)
    {
        int total = 0;
        foreach (var slot in slots)
            if (slot.item == item) total += slot.quantity;
        return total >= quantity;
    }
}