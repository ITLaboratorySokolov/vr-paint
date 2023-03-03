using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.UI.MessageBox
{
    /// <summary>
    /// Message box created to support UIToolkit (the new Unity UI system).
    /// </summary>
    public class MessageBoxUIToolkit : MessageBox
    {
        [HelpBox("All fields have to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        [Tooltip("Prepared UI document of a message box.")]
        [FormerlySerializedAs("document")]
        private UIDocument _document;

        [SerializeField]
        [Tooltip("Icon of an error type message box.")]
        [FormerlySerializedAs("errorIcon")]
        private Sprite _errorIcon;

        [SerializeField]
        [Tooltip("Icon of a warning type message box.")]
        [FormerlySerializedAs("warningIcon")]
        private Sprite _warningIcon;

        [SerializeField]
        [Tooltip("Icon of an info type message box.")]
        [FormerlySerializedAs("infoIcon")]
        private Sprite _infoIcon;

        private MessageBoxType _type;

        private Label _title;

        private VisualElement _icon;

        private Label _content;

        /// <inheritdoc/>
        public override string Title
        {
            get => _title.text;
            set => _title.text = value;
        }

        /// <inheritdoc/>
        public override MessageBoxType Type
        {
            get => _type;
            set
            {
                _type = value;
                _icon.style.backgroundImage = _type switch
                {
                    MessageBoxType.Error => new StyleBackground(_errorIcon),
                    MessageBoxType.Warning => new StyleBackground(_warningIcon),
                    MessageBoxType.Information => new StyleBackground(_infoIcon),
                    _ => _icon.style.backgroundImage
                };
            }
        }

        /// <inheritdoc/>
        public override string Content
        {
            get => _content.text;
            set => _content.text = value;
        }

        private void OnValidate()
        {
            Assert.IsNotNull(_document, "Document was null.");
            Assert.IsNotNull(_errorIcon, "Error Icon was null.");
            Assert.IsNotNull(_warningIcon, "Warning Icon was null.");
            Assert.IsNotNull(_infoIcon, "Info Icon was null.");
        }

        private void Awake()
        {
            var root = _document.rootVisualElement;
            _title = root.Q<Label>("title");
            _icon = root.Q<VisualElement>("icon");
            _content = root.Q<Label>("content");

            var submitButton = root.Q<Button>("submit");
            submitButton.clicked += SubmitButton_clicked;

            var exitButton = root.Q<Button>("exit");
            exitButton.clicked += ExitButton_clicked;
        }
    }
}