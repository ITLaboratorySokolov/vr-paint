using UnityEngine;
using UnityEngine.UIElements;

namespace ZCU.TechnologyLab.Common.Unity.UI
{
    public class MessageBoxUIToolkit : MessageBox
    {
        [SerializeField]
        private UIDocument document;

        [SerializeField]
        private Sprite errorIcon;

        [SerializeField]
        private Sprite warningIcon;

        [SerializeField]
        private Sprite infoIcon;

        private MessageBoxType type;

        private Label title;

        private VisualElement icon;

        private Label content;

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
                switch (this.type)
                {
                    case MessageBoxType.Error:
                        {
                            this.icon.style.backgroundImage = new StyleBackground(this.errorIcon);
                        }
                        break;
                    case MessageBoxType.Warning:
                        {
                            this.icon.style.backgroundImage = new StyleBackground(this.warningIcon);
                        }
                        break;
                    case MessageBoxType.Information:
                        {
                            this.icon.style.backgroundImage = new StyleBackground(this.infoIcon);
                        }
                        break;
                }
            }
        }

        public override string Content
        {
            get => this.content.text;
            set => this.content.text = value;
        }

        private void Awake()
        {
            var root = this.document.rootVisualElement;
            this.title = root.Q<Label>("title");
            this.icon = root.Q<VisualElement>("icon");
            this.content = root.Q<Label>("content");

            var submitButton = root.Q<Button>("submit");
            submitButton.clicked += SubmitButton_clicked;

            var exitButton = root.Q<Button>("exit");
            exitButton.clicked += ExitButton_clicked;
        }
    }
}