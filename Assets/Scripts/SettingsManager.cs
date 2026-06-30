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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadSettings();
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(KEY_VOLUME, value);
        Debug.Log($"[Settings] Volume changé à : {value}");
    }

    public void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt(KEY_QUALITY, index);
        Debug.Log($"[Settings] Qualité graphique changée à l'index : {index}");
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(KEY_FULLSCREEN, isFullscreen ? 1 : 0);
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
        PlayerPrefs.DeleteKey(KEY_VOLUME);
        PlayerPrefs.DeleteKey(KEY_QUALITY);
        PlayerPrefs.DeleteKey(KEY_FULLSCREEN);
        Debug.Log("[Settings] Réinitialisation des paramètres par défaut.");
        LoadSettings();
    }
}