using DS;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

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
            Debug.Log($"Rota : " + playerController.GetRotationY());
            Debug.Log($"Rota : " + data.playerRotY);

        }

        // Health / Stamina
        HealthSystem health = player.GetComponent<HealthSystem>();
        if (health != null)
        {
            data.currentHealth = health.maxHealth;
        }

        StaminaSystem stamina = player.GetComponent<StaminaSystem>();
        if (stamina != null)
        {
            data.currentStamina = stamina.maxStamina;
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

        // Flags de dialogue 
        DSDialogueFlags.ApplySaveData(data.flagKeys, data.flagValues);

        // Clean Item
        PickupItem[] scenePickups = FindObjectsByType<PickupItem>(FindObjectsSortMode.None);
        foreach (PickupItem pickup in scenePickups)
        {
            string flagCheck = "PickedUp_" + pickup.PickupID;
            if (DSDialogueFlags.Get(flagCheck))
            {
                Destroy(pickup.gameObject);
            }
        }

        // Position / rotation
        PlayerControllerTPS playerController = player.GetComponent<PlayerControllerTPS>();
        Vector3 playerPos = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);
        if (playerController != null)
        {
            //Debug.Log($"Rota : " + data.playerRotY);
            playerController.Warp(playerPos, data.playerRotY);
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

        // Companion
        GameObject mount = GameObject.FindWithTag("Mount");
        if (mount != null)
        {
            Vector3 playerForward = Quaternion.Euler(0, data.playerRotY, 0) * Vector3.forward;
            Vector3 spawnBehindPos = playerPos - (playerForward * 2f);

            UnityEngine.AI.NavMeshAgent agent = mount.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(spawnBehindPos);
            }
            else
            {
                mount.transform.position = spawnBehindPos;
            }
            mount.transform.rotation = Quaternion.Euler(0, data.playerRotY, 0);
        }
    }
    #endregion
}