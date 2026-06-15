using UnityEngine;
using UnityEngine.UIElements;

namespace YuvTales.UI.Core
{
    /// <summary>
    /// Classe de base abstraite pour tous les panels UI.
    /// 
    /// Chaque panel (Settings, Dialogue, Inventaire) hérite de cette classe
    /// et implémente ses propres méthodes d'initialisation et de binding.
    ///  
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIPanel : MonoBehaviour
    {
        // ─── Événements ────────────────────────────────────────────────────────────
         
        public event System.Action OnPanelShown;
         
        public event System.Action OnPanelHidden;

        // ─── Propriétés ────────────────────────────────────────────────────────────
         
        public bool IsVisible { get; private set; }
         
        protected VisualElement Root { get; private set; }

        // ─── Références privées ────────────────────────────────────────────────────

        private UIDocument _document;

        // ─── Unity Lifecycle ───────────────────────────────────────────────────────

        protected virtual void Awake()
        {
            _document = GetComponent<UIDocument>();

            if (_document == null)
            {
                Debug.LogError($"[UIPanel] Aucun UIDocument trouvé sur {gameObject.name}.");
                return;
            }

            Root = _document.rootVisualElement;
             
            SetVisible(false);
             
            InitializePanel();
        }

        // ─── API publique ──────────────────────────────────────────────────────────
         
        public void Show()
        {
            if (IsVisible) return;

            SetVisible(true);
            OnShow();
            OnPanelShown?.Invoke();
        }
         
        public void Hide()
        {
            if (!IsVisible) return;

            OnHide();
            SetVisible(false);
            OnPanelHidden?.Invoke();
        }
         
        public void Toggle()
        {
            if (IsVisible) Hide();
            else Show();
        }

        // ─── Méthodes abstraites & virtuelles à surcharger ────────────────────────
         
        protected abstract void InitializePanel();
         
        protected virtual void OnShow() { }
         
        protected virtual void OnHide() { }

        // ─── Méthodes privées ──────────────────────────────────────────────────────

        private void SetVisible(bool visible)
        {
            IsVisible = visible;

            if (Root != null)
                Root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
