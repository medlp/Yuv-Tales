using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "ZeLand";

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject slotSelectionPanel;

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    [Header("Navigation Manette")]
    [SerializeField] private GameObject firstButtonMainMenu;    
    [SerializeField] private GameObject firstButtonSettings;
    [SerializeField] private GameObject firstButtonSlotSelection;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, cursorHotspot, cursorMode);
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
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        if (slotSelectionPanel != null) slotSelectionPanel.SetActive(false);
        SelectFirstButton(firstButtonMainMenu);
    }

    private void SelectFirstButton(GameObject buttonToSelect)
    {
        if (buttonToSelect == null) return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }
}