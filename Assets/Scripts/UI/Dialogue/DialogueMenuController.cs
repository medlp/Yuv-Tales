using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using YuvTales.UI.Core;

namespace YuvTales.UI.Dialogue
{
    public class DialogueMenuController : UIPanel
    {
        private Label _actorNameLabel;
        private Label _messageTextLabel;
        private VisualElement _choicesContainer;
        private VisualElement _backgroundBox;

        [Header("UI References (Resources)")]
        [SerializeField] private VisualTreeAsset _choiceButtonTemplate;

        protected override void InitializePanel()
        {
            var root = Root;

            _backgroundBox = root.Q<VisualElement>("DialogueBox");
            _actorNameLabel = root.Q<Label>("ActorNameLabel");
            _messageTextLabel = root.Q<Label>("MessageTextLabel");
            _choicesContainer = root.Q<VisualElement>("ChoicesContainer");

            // Hide on start
            if (_backgroundBox != null)
                _backgroundBox.style.scale = new StyleScale(new Scale(Vector3.zero));
        }

        protected override void OnShow()
        {
            base.OnShow();

            // Lock camera when dialogue opens
            var playerController = FindFirstObjectByType<PlayerControllerTPS>();
            if (playerController != null)
            {
                playerController.LockCamera(true);
            }

            // Animate In (Scale from 0 to 1)
            if (_backgroundBox != null)
            {
                var anim = _backgroundBox.experimental.animation.Start(0f, 1f, 500, (visualElement, value) =>
                {
                    visualElement.style.scale = new StyleScale(new Scale(new Vector3(value, value, value)));
                }).Ease(Easing.OutCubic);
            }
        }

        protected override void OnHide()
        {
            base.OnHide();

            // Animate Out (Scale from 1 to 0)
            if (_backgroundBox != null)
            {
                var anim = _backgroundBox.experimental.animation.Start(1f, 0f, 500, (visualElement, value) =>
                {
                    visualElement.style.scale = new StyleScale(new Scale(new Vector3(value, value, value)));
                }).Ease(Easing.InCubic);
            }

            // Unlock camera when dialogue closes
            var playerController = FindFirstObjectByType<PlayerControllerTPS>();
            if (playerController != null)
            {
                playerController.LockCamera(false);
            }
        }

        public void DisplayDialogue(DialogueData data)
        {
            if (_actorNameLabel != null)
                _actorNameLabel.text = data.ActorName;

            if (_messageTextLabel != null)
            {
                _messageTextLabel.text = data.MessageText;

                // Animate Text Alpha (0 to 1)
                _messageTextLabel.style.opacity = 0f;
                var anim = _messageTextLabel.experimental.animation.Start(0f, 1f, 500, (visualElement, value) =>
                {
                    visualElement.style.opacity = value;
                }).Ease(Easing.InOutCubic);
            }

            if (_choicesContainer != null)
            {
                _choicesContainer.Clear();

                if (data.Choices == null || data.Choices.Count == 0)
                {
                    _choicesContainer.style.display = DisplayStyle.None;
                }
                else
                {
                    _choicesContainer.style.display = DisplayStyle.Flex;

                    foreach (var choice in data.Choices)
                    {
                        Button btn;
                        if (_choiceButtonTemplate != null)
                        {
                            var templateContainer = _choiceButtonTemplate.Instantiate();
                            btn = templateContainer.Q<Button>();
                            if(btn == null)
                            {
                                btn = new Button();
                                templateContainer.Add(btn);
                            }
                            btn.AddToClassList("dialogue-choice-button");
                            _choicesContainer.Add(templateContainer);
                        }
                        else
                        {
                            btn = new Button();
                            btn.AddToClassList("dialogue-choice-button");
                            _choicesContainer.Add(btn);
                        }

                        btn.text = choice.Text;
                        btn.clicked += () => choice.OnSelected?.Invoke();
                    }
                }
            }
        }
    }
}
