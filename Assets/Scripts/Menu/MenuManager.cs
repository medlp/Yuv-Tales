using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "CACA";

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject slotSelectionPanel;
    [SerializeField] private GameObject gameOverPanel;

    public static bool shouldShowGameOverOnLoad = false;

    [Header("Navigation Manette")]
    [SerializeField] private GameObject firstButtonMainMenu;    
    [SerializeField] private GameObject firstButtonSettings;
    [SerializeField] private GameObject firstButtonSlotSelection;


    private void Start()
    {
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

        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        if (shouldShowGameOverOnLoad)
        {
            ShowGameOver();
            shouldShowGameOverOnLoad = false; 
        }
        else
        {
            ShowMainMenu();
        }
    }


    // ── Bouton "Jouer" ───────────────────────────────────
    public void PlayGame()
    {
        OpenSlotSelection();
    }

    // ── Ouvre l'ecran de sélection de slot ──
    public void OpenSlotSelection()
    {
        mainMenuPanel.SetActive(false);
        slotSelectionPanel.SetActive(true);
        SelectFirstButton(firstButtonSlotSelection);
    }

    // ── Bouton "Retour" de l'ecran de selection de slot ──────────────────
    public void CloseSlotSelection()
    {
        slotSelectionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        SelectFirstButton(firstButtonMainMenu);
    }

    // ── Boutons "Slot 1 / Slot 2 / Slot 3" ────────────────────────────────

    public void SelectSlot(int slotIndex)
    {
        SaveManager.SelectedSlot = slotIndex;
        SceneManager.LoadScene(gameSceneName);
        GameManager.Instance.UpdateState(GameState.Gameplay);
    }

    // ── Bouton "Parametres" ──────────────────────────────
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        SelectFirstButton(firstButtonSettings);
    }

    // ── Bouton "Retour" Settings ───────────────────
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        SelectFirstButton(firstButtonMainMenu);
    }

    // ── Bouton "Quitter" ─────────────────────────────────
    public void QuitGame()
    {
        Application.Quit();


#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ShowMainMenu()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (slotSelectionPanel != null) slotSelectionPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        SelectFirstButton(firstButtonMainMenu);
    }

    public void ShowGameOver()
    {
        ShowMainMenu();
        gameOverPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void RespawnFromLastSave()
    {
        SceneManager.LoadScene(gameSceneName);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateState(GameState.Gameplay);
        }
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame(SaveManager.Instance.CurrentSlot);
            Debug.Log($"Current Slot : {SaveManager.Instance.CurrentSlot}");
        }

        ShowMainMenu();
    }
    private void SelectFirstButton(GameObject buttonToSelect)
    {
        if (buttonToSelect == null) return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }
}