using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZCU.TechnologyLab.Common.Unity.UI
{
    public class MessageBoxUGUI : MessageBox
    {
        [SerializeField]
        private TMP_Text title;

        [SerializeField]
        private TMP_Text content;

        [SerializeField]
        private Image icon;

        [SerializeField]
        private Sprite errorIcon;

        [SerializeField]
        private Sprite warningIcon;

        [SerializeField]
        private Sprite infoIcon;

        private MessageBoxType type;

        public override string Title
        {
            get => this.title.text;
            set => this.title.text = value;
        }

        public override MessageBoxType Type
        {
            get => this.type;
            set
            {
                this.type = value;
                switch(this.type)
                {
                    case MessageBoxType.Error:
                        {
                            this.icon.sprite = this.errorIcon;
                        } break;
                    case MessageBoxType.Warning:
                        {
                            this.icon.sprite = this.warningIcon;
                        }
                        break;
                    case MessageBoxType.Information: 
                        { 
                            this.icon.sprite = this.infoIcon;
                        } break;
                }
            }
        }

        public override string Content
        {
            get => this.content.text;
            set => this.content.text = value;
        }
    }
}