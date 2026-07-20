using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum GameState
{
    Gameplay,
    Dialogue,
    Pause,
    Inventory,
    MainMenu
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Current State")]
    [SerializeField] private GameState currentState = GameState.Gameplay;
    public GameState CurrentState => currentState;

    public static event Action<GameState> OnStateChanged;

    [Header("References")]
    [SerializeField] private PlayerControllerTPS playerController;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public PlayerControllerTPS GetPlayerControllerTPS() { return playerController; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerControllerTPS>();
        }
    }

    private void Start()
    {
        UpdateState(GameState.MainMenu);
    }

    private void Update()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerControllerTPS>();
        }
    }

    public void UpdateState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case GameState.Gameplay:
                HandleGameplayState();
                break;
            case GameState.Dialogue:
                HandleDialogueState();
                break;
            case GameState.Pause:
                HandlePauseState();
                break;
            case GameState.Inventory:
                HandleInventoryState();
                break;
            case GameState.MainMenu:
                HandleMainMenuState();
                break;

        }

        OnStateChanged?.Invoke(newState);
    }

    private void HandleGameplayState()
    {
        Time.timeScale = 1f;

        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetCursorLockState(CursorLockMode.Locked);
            CursorManager.Instance.SetCursorVisible(false);
            CursorManager.Instance.SetDefaultCursor();
        }

        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerControllerTPS>();
        }

        if (playerController != null)
        {
            playerController.LockCamera(false);
            playerController.SetMovementLocked(false);
        }
    }

    private void HandleDialogueState()
    {
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetCursorLockState(CursorLockMode.None);
            CursorManager.Instance.SetCursorVisible(true);
            CursorManager.Instance.SetDefaultCursor();
        }


        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerControllerTPS>();
        }

        if (playerController != null)
        {
            playerController.LockCamera(true);
            playerController.SetMovementLocked(true);
        }
    }

    private void HandleInventoryState()
    {
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetCursorLockState(CursorLockMode.None);
            CursorManager.Instance.SetCursorVisible(true);
            CursorManager.Instance.SetDefaultCursor();
        }


        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerControllerTPS>();
        }

        if (playerController != null)
        {
            playerController.LockCamera(true);
            playerController.SetMovementLocked(true);
        }
    }

    private void HandlePauseState()
    {
        Time.timeScale = 0f;

        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetCursorLockState(CursorLockMode.None);
            CursorManager.Instance.SetCursorVisible(true);
            CursorManager.Instance.SetDefaultCursor();
        }
    }

    private void HandleMainMenuState()
    {
        Time.timeScale = 1f; 

        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetCursorLockState(CursorLockMode.None);
            CursorManager.Instance.SetCursorVisible(true);
            CursorManager.Instance.SetDefaultCursor();
        }
    }

    public void PlayerDied()
    {
        Time.timeScale = 1f;

        MenuManager.shouldShowGameOverOnLoad = true;

        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        UpdateState(GameState.MainMenu);

        SceneManager.LoadScene(mainMenuSceneName);
    }
}