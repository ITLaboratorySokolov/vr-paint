using UnityEngine;
using UnityEngine.Assertions;
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
        private UIDocument document;

        [SerializeField]
        [Tooltip("Icon of an error type message box.")]
        private Sprite errorIcon;

        [SerializeField]
        [Tooltip("Icon of a warning type message box.")]
        private Sprite warningIcon;

        [SerializeField]
        [Tooltip("Icon of an info type message box.")]
        private Sprite infoIcon;

        private MessageBoxType type;

        private Label title;

        private VisualElement icon;

        private Label content;

        /// <inheritdoc/>
        public override string Title
        {
            get => this.title.text;
            set => this.title.text = value;
        }

        /// <inheritdoc/>
        public override MessageBoxType Type
        {
            get => this.type;
            set
            {
                this.type = value;
                this.icon.style.backgroundImage = this.type switch
                {
                    MessageBoxType.Error => new StyleBackground(this.errorIcon),
                    MessageBoxType.Warning => new StyleBackground(this.warningIcon),
                    MessageBoxType.Information => new StyleBackground(this.infoIcon),
                    _ => this.icon.style.backgroundImage
                };
            }
        }

        /// <inheritdoc/>
        public override string Content
        {
            get => this.content.text;
            set => this.content.text = value;
        }

        private void OnValidate()
        {
            Assert.IsNotNull(this.document, "Document was null.");
            Assert.IsNotNull(this.errorIcon, "Error Icon was null.");
            Assert.IsNotNull(this.warningIcon, "Warning Icon was null.");
            Assert.IsNotNull(this.infoIcon, "Info Icon was null.");
        }

        private void Awake()
        {
            var root = this.document.rootVisualElement;
            this.title = root.Q<Label>("title");
            this.icon = root.Q<VisualElement>("icon");
            this.content = root.Q<Label>("content");

            var submitButton = root.Q<Button>("submit");
            submitButton.clicked += this.SubmitButton_clicked;

            var exitButton = root.Q<Button>("exit");
            exitButton.clicked += this.ExitButton_clicked;
        }
    }
}