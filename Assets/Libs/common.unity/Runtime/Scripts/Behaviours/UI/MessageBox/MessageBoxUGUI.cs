using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("title")]
        private TMP_Text _title;

        [SerializeField]
        [FormerlySerializedAs("content")]
        private TMP_Text _content;

        [SerializeField]
        [FormerlySerializedAs("icon")]
        private Image _icon;

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
                _icon.sprite = _type switch
                {
                    MessageBoxType.Error => _errorIcon,
                    MessageBoxType.Warning => _warningIcon,
                    MessageBoxType.Information => _infoIcon,
                    _ => _icon.sprite
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
            Assert.IsNotNull(_title, "Title was null.");
            Assert.IsNotNull(_content, "Content was null.");
            Assert.IsNotNull(_icon, "Icon was null.");
            Assert.IsNotNull(_errorIcon, "Error Icon was null.");
            Assert.IsNotNull(_warningIcon, "Warning Icon was null.");
            Assert.IsNotNull(_infoIcon, "Info Icon was null.");
        }
    }
}