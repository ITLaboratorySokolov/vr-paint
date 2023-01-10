using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.UI.MessageBox
{
    /// <summary>
    /// Message box created to support UGUI (the old Unity UI system).
    /// </summary>
    public class MessageBoxUGUI : MessageBox
    {
        [HelpBox("All fields have to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private TMP_Text title;

        [SerializeField]
        private TMP_Text content;

        [SerializeField]
        private Image icon;

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
                this.icon.sprite = this.type switch
                {
                    MessageBoxType.Error => this.errorIcon,
                    MessageBoxType.Warning => this.warningIcon,
                    MessageBoxType.Information => this.infoIcon,
                    _ => this.icon.sprite
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
            Assert.IsNotNull(this.title, "Title was null.");
            Assert.IsNotNull(this.content, "Content was null.");
            Assert.IsNotNull(this.icon, "Icon was null.");
            Assert.IsNotNull(this.errorIcon, "Error Icon was null.");
            Assert.IsNotNull(this.warningIcon, "Warning Icon was null.");
            Assert.IsNotNull(this.infoIcon, "Info Icon was null.");
        }
    }
}