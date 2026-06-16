using UnityEngine;

namespace YuvTales.UI.Settings
{
    /// <summary>
    /// S'occupe uniquement de lire/écrire dans PlayerPrefs.
    /// Le controller ne touche jamais PlayerPrefs directement.
    /// </summary>
    public static class SettingsRepository
    {
        private const string KeyMasterVolume = "Settings_MasterVolume";
        private const string KeyMusicVolume = "Settings_MusicVolume";
        private const string KeySFXVolume = "Settings_SFXVolume";
        private const string KeySensitivity = "Settings_Sensitivity";
        private const string KeyLanguage = "Settings_Language";
        private const string KeyInvertY = "Settings_InvertY";

        public static SettingsData LoadSettings()
        {
            var data = new SettingsData();

            data.MasterVolume = PlayerPrefs.GetFloat(KeyMasterVolume, 1.0f);
            data.MusicVolume = PlayerPrefs.GetFloat(KeyMusicVolume, 1.0f);
            data.SFXVolume = PlayerPrefs.GetFloat(KeySFXVolume, 1.0f);
            data.Sensitivity = PlayerPrefs.GetFloat(KeySensitivity, 1.0f);
            data.Language = PlayerPrefs.GetString(KeyLanguage, "en");
            data.InvertY = PlayerPrefs.GetInt(KeyInvertY, 0) == 1;

            return data;
        }

        public static void SaveSettings(SettingsData data)
        {
            PlayerPrefs.SetFloat(KeyMasterVolume, data.MasterVolume);
            PlayerPrefs.SetFloat(KeyMusicVolume, data.MusicVolume);
            PlayerPrefs.SetFloat(KeySFXVolume, data.SFXVolume);
            PlayerPrefs.SetFloat(KeySensitivity, data.Sensitivity);
            PlayerPrefs.SetString(KeyLanguage, data.Language);
            PlayerPrefs.SetInt(KeyInvertY, data.InvertY ? 1 : 0);

            PlayerPrefs.Save();
        }
    }
}
