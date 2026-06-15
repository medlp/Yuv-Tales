namespace YuvTales.UI.Core
{
    /// <summary>
    /// Identifie chaque panel UI du jeu.
    /// Utilisé par l'UIManager pour ouvrir/fermer les panels sans couplage direct.
    /// 
    /// Ajouter une entrée ici lors de la création d'un nouveau panel.
    /// </summary>
    public enum PanelType
    {
        Settings,
        Dialogue,
        Inventory,
    }
}
