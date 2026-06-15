using System.Collections.Generic;
using UnityEngine;

namespace YuvTales.UI.Core
{
    /// <summary>
    /// Singleton central pour la gestion de tous les panels UI.
    /// 
    ///   UIManager.Instance.ShowPanel(PanelType.Settings);
    ///   UIManager.Instance.HidePanel(PanelType.Dialogue);
    ///   UIManager.Instance.TogglePanel(PanelType.Inventory);
    /// </summary>
    

    public class UIManager : MonoBehaviour
    {
        // ─── Singleton ─────────────────────────────────────────────────────────────

        public static UIManager Instance { get; private set; }

        // ─── Événements ────────────────────────────────────────────────────────────

        public event System.Action<PanelType> OnPanelOpened;

        public event System.Action<PanelType> OnPanelClosed;

        // ─── Propriétés ────────────────────────────────────────────────────────────

        public bool IsAnyPanelOpen
        {
            get
            {
                foreach (var panel in _panels.Values)
                    if (panel.IsVisible) return true;
                return false;
            }
        }

        // ─── Sérialisation Inspector ───────────────────────────────────────────────

        [Header("Panels enregistrés")]
        [Tooltip("Assigner chaque UIPanel depuis l'Inspector.")]
        [SerializeField] private PanelEntry[] _panelEntries;

        // ─── Données privées ───────────────────────────────────────────────────────

        private readonly Dictionary<PanelType, UIPanel> _panels = new();

        // ─── Unity Lifecycle ───────────────────────────────────────────────────────

        private void Awake()
        { 
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[UIManager] Doublon détecté, destruction du second UIManager.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            RegisterPanels();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // ─── API publique ──────────────────────────────────────────────────────────


        public void ShowPanel(PanelType type)
        {
            if (!TryGetPanel(type, out var panel)) return;
            if (panel.IsVisible) return;

            panel.Show();
            OnPanelOpened?.Invoke(type);

            Debug.Log($"[UIManager] Panel ouvert : {type}");
        } 

        public void HidePanel(PanelType type)
        {
            if (!TryGetPanel(type, out var panel)) return;
            if (!panel.IsVisible) return;

            panel.Hide();
            OnPanelClosed?.Invoke(type);

            Debug.Log($"[UIManager] Panel fermé : {type}");
        }
         
        public void TogglePanel(PanelType type)
        {
            if (!TryGetPanel(type, out var panel)) return;

            if (panel.IsVisible) HidePanel(type);
            else ShowPanel(type);
        }


        public void HideAllPanels()
        {
            foreach (var kvp in _panels)
            {
                if (kvp.Value.IsVisible)
                {
                    kvp.Value.Hide();
                    OnPanelClosed?.Invoke(kvp.Key);
                }
            }
        }


        public bool IsPanelOpen(PanelType type)
        {
            return TryGetPanel(type, out var panel) && panel.IsVisible;
        }

        // ─── Méthodes privées ──────────────────────────────────────────────────────

        /// <summary>
        /// Parcourt les entrées sérialisées et les enregistre dans le dictionnaire.
        /// </summary>
        private void RegisterPanels()
        {
            _panels.Clear();

            if (_panelEntries == null || _panelEntries.Length == 0)
            {
                Debug.LogWarning("[UIManager] Aucun panel enregistré dans l'Inspector.");
                return;
            }

            foreach (var entry in _panelEntries)
            {
                if (entry.Panel == null)
                {
                    Debug.LogWarning($"[UIManager] Entrée nulle pour le type {entry.Type}, ignorée.");
                    continue;
                }

                if (_panels.ContainsKey(entry.Type))
                {
                    Debug.LogWarning($"[UIManager] Type {entry.Type} déjà enregistré, doublon ignoré.");
                    continue;
                }

                _panels.Add(entry.Type, entry.Panel);
            }

            Debug.Log($"[UIManager] {_panels.Count} panel(s) enregistré(s).");
        }
         
        private bool TryGetPanel(PanelType type, out UIPanel panel)
        {
            if (_panels.TryGetValue(type, out panel))
                return true;

            Debug.LogError($"[UIManager] Panel introuvable pour le type : {type}. " +
                           $"Vérifie qu'il est bien assigné dans l'Inspector.");
            return false;
        }

        // ─── Structures internes ───────────────────────────────────────────────────
         
        [System.Serializable]
        private struct PanelEntry
        {
            public PanelType Type;
            public UIPanel   Panel;
        }
    }
}
