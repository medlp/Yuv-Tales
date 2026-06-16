using UnityEngine.UIElements;
using YuvTales.UI.Core;

namespace YuvTales.UI.Settings
{
    /// <summary>
    /// Branche les sliders/toggles/langue sur le SettingsData,
    /// appelle le SettingsRepository pour sauvegarder.
    /// </summary>
    public class SettingsMenuController : UIPanel
    {
        private SettingsData _currentData;

        // Visual Elements
        private Slider _masterVolumeSlider;
        private Slider _musicVolumeSlider;
        private Slider _sfxVolumeSlider;
        private Slider _sensitivitySlider;
        private DropdownField _languageDropdown;
        private Toggle _invertYToggle;
        private Button _closeButton;

        protected override void InitializePanel()
        {
            // Trouver les éléments UI
            _masterVolumeSlider = Root.Q<Slider>("MasterVolumeSlider");
            _musicVolumeSlider = Root.Q<Slider>("MusicVolumeSlider");
            _sfxVolumeSlider = Root.Q<Slider>("SFXVolumeSlider");
            _sensitivitySlider = Root.Q<Slider>("SensitivitySlider");
            _languageDropdown = Root.Q<DropdownField>("LanguageDropdown");
            _invertYToggle = Root.Q<Toggle>("InvertYToggle");
            _closeButton = Root.Q<Button>("CloseButton");

            // Charger les données depuis le repository
            _currentData = SettingsRepository.LoadSettings();

            // Mettre à jour l'UI avec les données chargées
            UpdateUIFromData();

            // Enregistrer les événements de changement
            RegisterCallbacks();
        }

        private void UpdateUIFromData()
        {
            if (_masterVolumeSlider != null) _masterVolumeSlider.value = _currentData.MasterVolume;
            if (_musicVolumeSlider != null) _musicVolumeSlider.value = _currentData.MusicVolume;
            if (_sfxVolumeSlider != null) _sfxVolumeSlider.value = _currentData.SFXVolume;
            if (_sensitivitySlider != null) _sensitivitySlider.value = _currentData.Sensitivity;
            if (_languageDropdown != null) _languageDropdown.value = _currentData.Language;
            if (_invertYToggle != null) _invertYToggle.value = _currentData.InvertY;
        }

        private void RegisterCallbacks()
        {
            if (_masterVolumeSlider != null)
                _masterVolumeSlider.RegisterValueChangedCallback(evt => { _currentData.MasterVolume = evt.newValue; SaveSettings(); });

            if (_musicVolumeSlider != null)
                _musicVolumeSlider.RegisterValueChangedCallback(evt => { _currentData.MusicVolume = evt.newValue; SaveSettings(); });

            if (_sfxVolumeSlider != null)
                _sfxVolumeSlider.RegisterValueChangedCallback(evt => { _currentData.SFXVolume = evt.newValue; SaveSettings(); });

            if (_sensitivitySlider != null)
                _sensitivitySlider.RegisterValueChangedCallback(evt => { _currentData.Sensitivity = evt.newValue; SaveSettings(); });

            if (_languageDropdown != null)
                _languageDropdown.RegisterValueChangedCallback(evt => { _currentData.Language = evt.newValue; SaveSettings(); });

            if (_invertYToggle != null)
                _invertYToggle.RegisterValueChangedCallback(evt => { _currentData.InvertY = evt.newValue; SaveSettings(); });

            if (_closeButton != null)
                _closeButton.clicked += Hide;
        }

        private void SaveSettings()
        {
            SettingsRepository.SaveSettings(_currentData);
            // La notification au reste du jeu via événements se fera plus tard
        }

        protected override void OnShow()
        {
            // On peut recharger les données à chaque ouverture si besoin,
            // mais ici on garde l'état en mémoire qui est synchronisé
            base.OnShow();
        }
    }
}
