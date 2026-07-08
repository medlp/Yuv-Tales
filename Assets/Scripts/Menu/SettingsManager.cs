using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;

    [Header("Camera")]
    [SerializeField] private Slider mainCamSliderX;
    [SerializeField] private Slider mainCamSliderY;
    [SerializeField] private Slider zoomCamSliderX;
    [SerializeField] private Slider zoomCamSliderY;

    [Header("Graphismes")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    public enum SettingsTab
    {
        Global,
        Camera
    }

    private SettingsTab currentTab = SettingsTab.Global;

    private const string KEY_VOLUME = "MasterVolume";
    private const string KEY_QUALITY = "GraphicsQuality";
    private const string KEY_FULLSCREEN = "Fullscreen";
    private const string KEY_MAIN_CAMERA_SENSITIVITY_X = "MainCamSensitivityX";
    private const string KEY_MAIN_CAMERA_SENSITIVITY_Y = "MainCamSensitivityY";
    private const string KEY_ZOOM_CAMERA_SENSITIVITY_X = "ZoomCamSensitivityX";
    private const string KEY_ZOOM_CAMERA_SENSITIVITY_Y = "ZoomCamSensitivityY";

    public static float MainCamSensitivityX { get; private set; } = 1f;
    public static float MainCamSensitivityY { get; private set; } = 1f;
    public static float ZoomCamSensitivityX { get; private set; } = 1f;
    public static float ZoomCamSensitivityY { get; private set; } = 1f;


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

    public void SetCurrentTab(int tabIndex)
    {
        currentTab = (SettingsTab)tabIndex;
    }
    public void OnMainCamXSensitivityChanged(float value)
    {
        MainCamSensitivityX = value;
        PlayerPrefs.SetFloat(KEY_MAIN_CAMERA_SENSITIVITY_X, value);
        Debug.Log($"[Settings] Sensibilité main cam X changée à : {value}");
    }
    public void OnMainCamYSensitivityChanged(float value)
    {
        MainCamSensitivityY = value;
        PlayerPrefs.SetFloat(KEY_MAIN_CAMERA_SENSITIVITY_Y, value);
        Debug.Log($"[Settings] Sensibilité main cam Y changée à : {value}");
    }

    public void OnZoomCamXSensitivityChanged(float value)
    {
        ZoomCamSensitivityX = value;
        PlayerPrefs.SetFloat(KEY_ZOOM_CAMERA_SENSITIVITY_X, value);
        Debug.Log($"[Settings] Sensibilité zoom cam X changée à : {value}");
    }
    public void OnZoomCamYSensitivityChanged(float value)
    {
        ZoomCamSensitivityY = value;
        PlayerPrefs.SetFloat(KEY_ZOOM_CAMERA_SENSITIVITY_Y, value);
        Debug.Log($"[Settings] Sensibilité zoom cam Y changée à : {value}");
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
        float mainCameraSensitivityX = PlayerPrefs.GetFloat(KEY_MAIN_CAMERA_SENSITIVITY_X, 1f);
        float mainCameraSensitivityY = PlayerPrefs.GetFloat(KEY_MAIN_CAMERA_SENSITIVITY_Y, 1f);
        float zoomCameraSensitivityX = PlayerPrefs.GetFloat(KEY_ZOOM_CAMERA_SENSITIVITY_X, 1f);
        float zoomCameraSensitivityY = PlayerPrefs.GetFloat(KEY_ZOOM_CAMERA_SENSITIVITY_Y, 1f);

        AudioListener.volume = volume;
        QualitySettings.SetQualityLevel(quality);
        Screen.fullScreen = fullscreen;
        MainCamSensitivityX = mainCameraSensitivityX;
        MainCamSensitivityY = mainCameraSensitivityY;
        ZoomCamSensitivityX = zoomCameraSensitivityX;
        ZoomCamSensitivityY = zoomCameraSensitivityY;

        if (masterVolumeSlider != null) masterVolumeSlider.value = volume;
        if (qualityDropdown != null) qualityDropdown.value = quality;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;
        if (mainCamSliderX != null) mainCamSliderX.value = mainCameraSensitivityX;
        if (mainCamSliderY != null) mainCamSliderY.value = mainCameraSensitivityY;
        if (zoomCamSliderX != null) zoomCamSliderX.value = zoomCameraSensitivityX;
        if (zoomCamSliderY != null) zoomCamSliderY.value = zoomCameraSensitivityY;

        Debug.Log($"[Settings] Paramètres chargés : Vol={volume}, Qualité={quality}, PleinEcran={fullscreen}" +
            $", SensiMainX={mainCameraSensitivityX}, SensiMainY={mainCameraSensitivityY}, SensiZoomX={zoomCameraSensitivityX}, SensiZoomY={zoomCameraSensitivityY}");
    }

    // ── Bouton "Réinitialiser" ─────────────────
    public void ResetToDefaults()
    {
        switch (currentTab)
        {
            case SettingsTab.Global:
                PlayerPrefs.DeleteKey(KEY_VOLUME);
                PlayerPrefs.DeleteKey(KEY_QUALITY);
                PlayerPrefs.DeleteKey(KEY_FULLSCREEN);
                Debug.Log("[Settings] Réinitialisation de l'onglet Global.");
                break;

            case SettingsTab.Camera:
                PlayerPrefs.DeleteKey(KEY_MAIN_CAMERA_SENSITIVITY_X);
                PlayerPrefs.DeleteKey(KEY_MAIN_CAMERA_SENSITIVITY_Y);
                PlayerPrefs.DeleteKey(KEY_ZOOM_CAMERA_SENSITIVITY_X);
                PlayerPrefs.DeleteKey(KEY_ZOOM_CAMERA_SENSITIVITY_Y);
                Debug.Log("[Settings] Réinitialisation de l'onglet Caméra.");
                break;
        }

        LoadSettings();
        EventSystem.current.SetSelectedGameObject(null);
    }
}