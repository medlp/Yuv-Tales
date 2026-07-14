using UnityEngine;
using System;

public enum GameState
{
    Gameplay,
    Dialogue,
    Pause,
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
        UpdateState(GameState.Gameplay);
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
            CursorManager.Instance.SetCursorLockState(CursorLockMode.None);
            CursorManager.Instance.SetCursorVisible(true);
            CursorManager.Instance.SetDefaultCursor();
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
}