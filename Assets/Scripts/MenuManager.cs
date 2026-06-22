using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Z_Test_Scene";

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMainMenu();
    }

    // ── Bouton "Jouer" ───────────────────────────────────
    public void PlayGame()
    {
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

        // Ligne utile pour tester dans editeur :
        UnityEditor.EditorApplication.isPlaying = false;
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
}