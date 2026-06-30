using DS;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Autosave")]
    [SerializeField] private bool enableAutosave = true;
    [SerializeField] private float autosaveInterval = 900f; // 15 min

    [Header("Debug")]
    [Tooltip("Active la touche de save rapide pendant les tests.")]
    [SerializeField] private bool enableDebugSaveKey = true;
    [Tooltip("Affiche le détail des données chargées dans la Console.")]
    [SerializeField] private bool logDetailedLoad = true;

    private const string SaveFileNamePattern = "save_slot{0}.json";
    private const string AutosaveFileName = "save_autosave.json";
    public const int SlotCount = 3;

    private Coroutine autosaveRoutine;

    #region Unity Methods
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (enableAutosave)
            autosaveRoutine = StartCoroutine(AutosaveLoop());
    }

    void Update()
    {
        if (enableDebugSaveKey && Keyboard.current != null)
        {
            bool shiftHeld = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                if (shiftHeld) LoadGame(0); else SaveGame(0);
            }

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                if (shiftHeld) LoadGame(1); else SaveGame(1);
            }

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                if (shiftHeld) LoadGame(2); else SaveGame(2);
            }

            if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                LoadAutosave();
                Debug.Log("LOAD");
            }
        }

    }
    

    private IEnumerator AutosaveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(autosaveInterval);
            SaveAutosave();
        }
    }
    #endregion

    #region Paths
    private string GetSlotPath(int slotIndex)
    {
        string fileName = string.Format(SaveFileNamePattern, slotIndex);
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    private string GetAutosavePath()
    {
        return Path.Combine(Application.persistentDataPath, AutosaveFileName);
    }

    public bool SlotExists(int slotIndex)
    {
        return File.Exists(GetSlotPath(slotIndex));
    }

    public bool AutosaveExists()
    {
        return File.Exists(GetAutosavePath());
    }
    #endregion

    #region Save System
    public void SaveGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotCount)
        {
            return;
        }

        GameSaveData data = BuildSaveData();
        WriteToDisk(data, GetSlotPath(slotIndex));

    }

    public void SaveAutosave()
    {
        GameSaveData data = BuildSaveData();
        WriteToDisk(data, GetAutosavePath());
    }

    private void WriteToDisk(GameSaveData data, string path)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    private GameSaveData BuildSaveData()
    {
        GameSaveData data = new GameSaveData
        {
            saveDate = DateTime.Now.ToString("o")
        };

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            return data;
        }

        GameObject mount = GameObject.FindWithTag("Mount");
        if (mount == null)
        {
            Debug.Log("CACA");
            return data;
        }

        // Position / rotation
        PlayerControllerTPS playerController = player.GetComponent<PlayerControllerTPS>();
        if (playerController != null)
        {
            Vector3 pos = playerController.GetPosition();
            data.playerPosX = pos.x;
            data.playerPosY = pos.y;
            data.playerPosZ = pos.z;
            data.playerRotY = playerController.GetRotationY();

        }

        // Health / Stamina
        HealthSystem health = player.GetComponent<HealthSystem>();
        if (health != null)
        {
            data.currentHealth = health.currentHealth;
        }

        StaminaSystem stamina = player.GetComponent<StaminaSystem>();
        if (stamina != null)
        {
            data.currentStamina = stamina.currentStamina;
        }

        // Inventaire
        InventorySystem inventory = player.GetComponent<InventorySystem>();
        if (inventory != null)
        {
            data.inventorySlots = inventory.ExtractSaveData();

            int nonEmptyCount = data.inventorySlots.FindAll(s => !string.IsNullOrEmpty(s.itemName)).Count;
        }

        // Flags de dialogue
        DSDialogueFlags.ExtractSaveData(out data.flagKeys, out data.flagValues);

        // DigZones
        DigZone[] digZones = FindObjectsByType<DigZone>(FindObjectsSortMode.None);
        foreach (DigZone zone in digZones)
        {
            if (zone.IsAlreadyDug)
                data.dugZoneIDs.Add(zone.ZoneID);
        }

        // Ping Familiar
        CompanionFollowing companion = mount.GetComponent<CompanionFollowing>();
        if (companion != null)
        {
            Vector3 pos = companion.pingPosition;
            data.pingPosX = pos.x;
            data.pingPosY = pos.y;
            data.pingPosZ = pos.z;
        }

        return data;
    }
    #endregion

    #region Load System
    public void LoadGame(int slotIndex)
    {
        if (!SlotExists(slotIndex))
        {
            return;
        }

        GameSaveData data = ReadFromDisk(GetSlotPath(slotIndex));
        ApplySaveData(data);

    }

    public void LoadAutosave()
    {
        if (!AutosaveExists())
        {
            return;
        }

        GameSaveData data = ReadFromDisk(GetAutosavePath());
        ApplySaveData(data);

    }

    private GameSaveData ReadFromDisk(string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    private void ApplySaveData(GameSaveData data)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            return;
        }

        GameObject mount = GameObject.FindWithTag("Mount");
        if (mount == null)
        {
            return;
        }

        // Flags de dialogue 
        DSDialogueFlags.ApplySaveData(data.flagKeys, data.flagValues);

        // Position / rotation
        PlayerControllerTPS playerController = player.GetComponent<PlayerControllerTPS>();
        if (playerController != null)
        {
            Vector3 pos = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);
            playerController.Warp(pos, data.playerRotY);
        }

        // Health / Stamina
        HealthSystem health = player.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.currentHealth = data.currentHealth;
        }

        StaminaSystem stamina = player.GetComponent<StaminaSystem>();
        if (stamina != null)
        {
            stamina.currentStamina = data.currentStamina;
        }

        // Inventaire
        InventorySystem inventory = player.GetComponent<InventorySystem>();
        if (inventory != null)
        {
            inventory.ApplySaveData(data.inventorySlots);

            if (logDetailedLoad)
            {
                int nonEmptyCount = data.inventorySlots.FindAll(s => !string.IsNullOrEmpty(s.itemName)).Count;
            }
        }

        // DigZones
        DigZone[] digZones = FindObjectsByType<DigZone>(FindObjectsSortMode.None);
        int restoredZoneCount = 0;
        foreach (DigZone zone in digZones)
        {
            if (data.dugZoneIDs.Contains(zone.ZoneID))
            {
                zone.RestoreDugState();
                restoredZoneCount++;
            }
        }

        // Ping Familiar
        CompanionFollowing companionFollowing = mount.GetComponent<CompanionFollowing>();
        if (companionFollowing != null)
        {
            Vector3 pos = new Vector3(data.pingPosX, data.pingPosY, data.pingPosZ);
            companionFollowing.pingPosition = pos;
        }
    }
    #endregion
}