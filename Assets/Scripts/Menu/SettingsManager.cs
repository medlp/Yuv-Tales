using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;

    [Header("Graphismes")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private const string KEY_VOLUME = "MasterVolume";
    private const string KEY_QUALITY = "GraphicsQuality";
    private const string KEY_FULLSCREEN = "Fullscreen";

    private static SettingsManager instance;
    private bool isInitializing = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplyGlobalSettingsOnLoad()
    {
        float volume = PlayerPrefs.GetFloat(KEY_VOLUME, 1f);
        int quality = PlayerPrefs.GetInt(KEY_QUALITY, QualitySettings.GetQualityLevel());
        bool fullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;

        AudioListener.volume = volume;
        QualitySettings.SetQualityLevel(quality);
        Screen.fullScreen = fullscreen;

        Debug.Log($"[Settings] Paramètres appliqués au démarrage : Vol={volume}, Qualité={quality}, PleinEcran={fullscreen}");
    }

    private void Awake()
    {
        // SettingsManager manages UI elements specific to the current scene (sliders, toggles).
        // It shouldn't be a DontDestroyOnLoad Singleton because it needs to hook up to the local UI.
        // And it shares a GameObject with MenuManager / PauseManager.
        instance = this;
    }



    private void Start()
    {
        isInitializing = true;

        // Remplir le dropdown avec les noms réels des Quality Levels
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
        }

        LoadSettings();

        isInitializing = false;
    }

    public void OnVolumeChanged(float value)
    {
        if (isInitializing) return;

        AudioListener.volume = value;
        PlayerPrefs.SetFloat(KEY_VOLUME, value);
        PlayerPrefs.Save();
        Debug.Log($"[Settings] Volume changé à : {value}");
    }

    public void OnQualityChanged(int index)
    {
        if (isInitializing) return;

        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt(KEY_QUALITY, index);
        PlayerPrefs.Save();
        Debug.Log($"[Settings] Qualité graphique changée à l'index : {index}");
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        if (isInitializing) return;

        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(KEY_FULLSCREEN, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"[Settings] Plein écran défini sur : {isFullscreen}");
    }

    // ── Charger settings sauvegardés ─────────────────
    private void LoadSettings()
    {
        float volume = PlayerPrefs.GetFloat(KEY_VOLUME, 1f);
        int quality = PlayerPrefs.GetInt(KEY_QUALITY, QualitySettings.GetQualityLevel());
        bool fullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;

        AudioListener.volume = volume;
        QualitySettings.SetQualityLevel(quality);
        Screen.fullScreen = fullscreen;

        if (masterVolumeSlider != null) masterVolumeSlider.value = volume;
        if (qualityDropdown != null) qualityDropdown.value = quality;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;

        Debug.Log($"[Settings] Paramètres chargés : Vol={volume}, Qualité={quality}, PleinEcran={fullscreen}");
    }

    // ── Bouton "Réinitialiser" ─────────────────
    public void ResetToDefaults()
    {
        isInitializing = true;
        PlayerPrefs.DeleteKey(KEY_VOLUME);
        PlayerPrefs.DeleteKey(KEY_QUALITY);
        PlayerPrefs.DeleteKey(KEY_FULLSCREEN);
        PlayerPrefs.Save();
        Debug.Log("[Settings] Réinitialisation des paramètres par défaut.");
        LoadSettings();
        isInitializing = false;
    }
}