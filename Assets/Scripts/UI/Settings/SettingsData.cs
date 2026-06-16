namespace YuvTales.UI.Settings
{
    /// <summary>
    /// Modèle de données (POCO) contenant les paramètres du jeu.
    /// Aucune logique liée à Unity (pas de MonoBehaviour).
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
