namespace YuvTales.UI.Settings
{
    /// <summary>
    /// Modèle de donnée contenant les paramètres du jeu.
    /// </summary>
    [System.Serializable]
    public class SettingsData
    {
        public float MasterVolume = 1.0f;
        public float MusicVolume = 1.0f;
        public float SFXVolume = 1.0f;
        public float Sensitivity = 1.0f;
        public string Language = "en";
        public bool InvertY = false;

        public SettingsData()
        {
        }
    }
}
