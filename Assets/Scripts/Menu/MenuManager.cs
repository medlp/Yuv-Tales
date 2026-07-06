using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "CACA";

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject slotSelectionPanel;


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

        ShowMainMenu();
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
    }

    // ── Bouton "Retour" de l'ecran de selection de slot ──────────────────
    public void CloseSlotSelection()
    {
        slotSelectionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // ── Boutons "Slot 1 / Slot 2 / Slot 3" ────────────────────────────────

    public void SelectSlot(int slotIndex)
    {
        SaveManager.SelectedSlot = slotIndex;
        SceneManager.LoadScene(gameSceneName);
    }

    // ── Bouton "Parametres" ──────────────────────────────
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // ── Bouton "Retour" Settings ───────────────────
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
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
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        if (slotSelectionPanel != null) slotSelectionPanel.SetActive(false);
    }
}