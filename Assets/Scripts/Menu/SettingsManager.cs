using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

    private CinemachineCamera vcamNormal;
    private CinemachineCamera vcamAim;

    [Header("Graphismes")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider shadowDistanceSlider;
    [SerializeField] private Slider farClipPlaneSlider;

    [Header("Shadow Distance Range")]
    [SerializeField] private float shadowDistanceMin = 150f;
    [SerializeField] private float shadowDistanceMax = 500f;

    [Header("Far Clip Plane Range")]
    [SerializeField] private float farClipPlaneMin = 200f;
    [SerializeField] private float farClipPlaneMax = 3000f;

    public enum SettingsTab
    {
        Global,
        Camera,
        Graphics
    }

    private SettingsTab currentTab = SettingsTab.Global;

    private const string KEY_VOLUME = "MasterVolume";
    private const string KEY_QUALITY = "GraphicsQuality";
    private const string KEY_FULLSCREEN = "Fullscreen";
    private const string KEY_MAIN_CAMERA_SENSITIVITY_X = "MainCamSensitivityX";
    private const string KEY_MAIN_CAMERA_SENSITIVITY_Y = "MainCamSensitivityY";
    private const string KEY_ZOOM_CAMERA_SENSITIVITY_X = "ZoomCamSensitivityX";
    private const string KEY_ZOOM_CAMERA_SENSITIVITY_Y = "ZoomCamSensitivityY";
    private const string KEY_SHADOW_DISTANCE = "ShadowDistance";
    private const string KEY_FAR_CLIP_PLANE = "FarClipPlane";
    public static float MainCamSensitivityX { get; private set; } = 1f;
    public static float MainCamSensitivityY { get; private set; } = 1f;
    public static float ZoomCamSensitivityX { get; private set; } = 1f;
    public static float ZoomCamSensitivityY { get; private set; } = 1f;


    private static SettingsManager instance;

    #region Unity Methods
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Find Camera
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCamerasInScene();
        ApplyFarClipPlaneToCameras(); 
    }

    private void FindCamerasInScene()
    {
        PlayerCameraController camController = FindFirstObjectByType<PlayerCameraController>();
        if (camController != null)
        {
            vcamNormal = camController.GetVcamNormal();
            vcamAim = camController.GetVcamAim();
        }
        else
        {
            vcamNormal = null;
            vcamAim = null;
        }
    }

    private void ApplyFarClipPlaneToCameras()
    {
        float farClipLevel = PlayerPrefs.GetFloat(KEY_FAR_CLIP_PLANE, 5f);
        float farClipRealDistance = Mathf.Lerp(farClipPlaneMin, farClipPlaneMax, farClipLevel / 10f);

        if (vcamNormal != null) vcamNormal.Lens.FarClipPlane = farClipRealDistance;
        if (vcamAim != null) vcamAim.Lens.FarClipPlane = farClipRealDistance;
    }
    #endregion

    #region Navigation Settings 
    public void SetCurrentTab(int tabIndex)
    {
        currentTab = (SettingsTab)tabIndex;
    }

    #endregion

    #region Global Settings
    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(KEY_VOLUME, value);
        Debug.Log($"[Settings] Volume changé à : {value}");
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(KEY_FULLSCREEN, isFullscreen ? 1 : 0);
        Debug.Log($"[Settings] Plein écran défini sur : {isFullscreen}");
    }
    #endregion

    #region Graphics Settings
    public void OnShadowDistanceChanged(float value)
    {
        float t = value / 10f;
        float realDistance = Mathf.Lerp(shadowDistanceMin, shadowDistanceMax, t);

        QualitySettings.shadowDistance = realDistance;
        PlayerPrefs.SetFloat(KEY_SHADOW_DISTANCE, value);

        Debug.Log($"[Settings] Shadow distance niveau {value}/10 → {realDistance}");
    }

    public void OnFarClipPlaneChanged(float value)
    {
        float t = value / 10f;
        float realDistance = Mathf.Lerp(farClipPlaneMin, farClipPlaneMax, t);

        if (vcamNormal != null) vcamNormal.Lens.FarClipPlane = realDistance;
        if (vcamAim != null) vcamAim.Lens.FarClipPlane = realDistance;

        PlayerPrefs.SetFloat(KEY_FAR_CLIP_PLANE, value);

        Debug.Log($"[Settings] Far clip plane niveau {value}/10 → {realDistance}");
    }
    public void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt(KEY_QUALITY, index);
        Debug.Log($"[Settings] Qualité graphique changée à l'index : {index}");
    }
    #endregion

    #region Camera Settings
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
    #endregion


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
        float shadowLevel = PlayerPrefs.GetFloat(KEY_SHADOW_DISTANCE, 5f); 
        float shadowRealDistance = Mathf.Lerp(shadowDistanceMin, shadowDistanceMax, shadowLevel / 10f);
        float farClipLevel = PlayerPrefs.GetFloat(KEY_FAR_CLIP_PLANE, 5f); 
        float farClipRealDistance = Mathf.Lerp(farClipPlaneMin, farClipPlaneMax, farClipLevel / 10f);

        AudioListener.volume = volume;
        QualitySettings.SetQualityLevel(quality);
        Screen.fullScreen = fullscreen;
        MainCamSensitivityX = mainCameraSensitivityX;
        MainCamSensitivityY = mainCameraSensitivityY;
        ZoomCamSensitivityX = zoomCameraSensitivityX;
        ZoomCamSensitivityY = zoomCameraSensitivityY;
        QualitySettings.shadowDistance = shadowRealDistance;

        if (vcamNormal != null) vcamNormal.Lens.FarClipPlane = farClipRealDistance;
        if (vcamAim != null) vcamAim.Lens.FarClipPlane = farClipRealDistance;

        if (masterVolumeSlider != null) masterVolumeSlider.SetValueWithoutNotify(volume);
        if (qualityDropdown != null) qualityDropdown.SetValueWithoutNotify(quality);
        if (fullscreenToggle != null) fullscreenToggle.SetIsOnWithoutNotify(fullscreen);
        if (mainCamSliderX != null) mainCamSliderX.SetValueWithoutNotify(mainCameraSensitivityX);
        if (mainCamSliderY != null) mainCamSliderY.SetValueWithoutNotify(mainCameraSensitivityY);
        if (zoomCamSliderX != null) zoomCamSliderX.SetValueWithoutNotify(zoomCameraSensitivityX);
        if (zoomCamSliderY != null) zoomCamSliderY.SetValueWithoutNotify(zoomCameraSensitivityY);
        if (shadowDistanceSlider != null) shadowDistanceSlider.SetValueWithoutNotify(shadowLevel);
        if (farClipPlaneSlider != null) farClipPlaneSlider.SetValueWithoutNotify(farClipLevel);

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
                PlayerPrefs.DeleteKey(KEY_FULLSCREEN);
                break;

            case SettingsTab.Graphics:
                PlayerPrefs.DeleteKey(KEY_QUALITY);
                PlayerPrefs.DeleteKey(KEY_SHADOW_DISTANCE);
                PlayerPrefs.DeleteKey(KEY_FAR_CLIP_PLANE);
                break;

            case SettingsTab.Camera:
                PlayerPrefs.DeleteKey(KEY_MAIN_CAMERA_SENSITIVITY_X);
                PlayerPrefs.DeleteKey(KEY_MAIN_CAMERA_SENSITIVITY_Y);
                PlayerPrefs.DeleteKey(KEY_ZOOM_CAMERA_SENSITIVITY_X);
                PlayerPrefs.DeleteKey(KEY_ZOOM_CAMERA_SENSITIVITY_Y);
                break;
        }

        LoadSettings();
        EventSystem.current.SetSelectedGameObject(null);
    }
}