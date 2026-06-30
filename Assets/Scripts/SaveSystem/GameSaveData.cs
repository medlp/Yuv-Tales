using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GameSaveData
{
    [Header("Meta")]
    public string saveDate; 
    [Header("Player")]
    public float playerPosX, playerPosY, playerPosZ;
    public float playerRotY;

    [Header("Health / Stamina")]
    public float currentHealth;
    public float currentStamina;

    [Header("Dialogue Flags")]
    public List<string> flagKeys = new();
    public List<bool> flagValues = new();

    [Header("Inventory")]
    public List<InventorySlotSaveData> inventorySlots = new();

    [Header("Dig Zones")]
    public List<string> dugZoneIDs = new();

    [Header("Ping Familiar")]
    public float pingPosX, pingPosY, pingPosZ;
}

[Serializable]
public class InventorySlotSaveData
{
    public string itemName; 
    public int quantity;

    public InventorySlotSaveData(string itemName, int quantity)
    {
        this.itemName = itemName;
        this.quantity = quantity;
    }
}