using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider mouseSlider;

    [Header("Graphismes")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private const string KEY_VOLUME = "MasterVolume";
    private const string KEY_QUALITY = "GraphicsQuality";
    private const string KEY_FULLSCREEN = "Fullscreen";
    private const string KEY_MOUSE_SENSITIVITY = "MouseSensitivity";

    public static float MouseSensitivity { get; private set; } = 1f;

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
        // Remplir le dropdown avec les noms réels des Quality Levels
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
        }

        LoadSettings();
    }
    public void OnMouseSensitivityChanged(float value)
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat(KEY_MOUSE_SENSITIVITY, value);
        Debug.Log($"[Settings] Sensibilité souris changée à : {value}");
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
        float mouseSensitivity = PlayerPrefs.GetFloat(KEY_MOUSE_SENSITIVITY, 1f);

        AudioListener.volume = volume;
        QualitySettings.SetQualityLevel(quality);
        Screen.fullScreen = fullscreen;
        MouseSensitivity = mouseSensitivity;

        if (masterVolumeSlider != null) masterVolumeSlider.value = volume;
        if (qualityDropdown != null) qualityDropdown.value = quality;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;
        if (mouseSlider != null) mouseSlider.value = mouseSensitivity;

        Debug.Log($"[Settings] Paramètres chargés : Vol={volume}, Qualité={quality}, PleinEcran={fullscreen}, Sensi={mouseSensitivity}");
    }

    // ── Bouton "Réinitialiser" ─────────────────
    public void ResetToDefaults()
    {
        PlayerPrefs.DeleteKey(KEY_VOLUME);
        PlayerPrefs.DeleteKey(KEY_QUALITY);
        PlayerPrefs.DeleteKey(KEY_FULLSCREEN);
        PlayerPrefs.DeleteKey(KEY_MOUSE_SENSITIVITY);
        Debug.Log("[Settings] Réinitialisation des paramètres par défaut.");
        LoadSettings();
    }
}