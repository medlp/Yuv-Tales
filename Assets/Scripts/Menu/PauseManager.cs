using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetCursorLockState(CursorLockMode.None);
            CursorManager.Instance.SetCursorVisible(true);
            CursorManager.Instance.SetDefaultCursor();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetCursorLockState(CursorLockMode.Locked);
            CursorManager.Instance.SetCursorVisible(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;

        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

}
