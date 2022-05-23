using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.UI
{
    public abstract class MessageBox : MonoBehaviour
    {
        public enum MessageBoxType
        {
            Error,
            Warning,
            Information
        }

        public abstract string Title { get; set; }

        public abstract MessageBoxType Type { get; set; }

        public abstract string Content { get; set; }

        public void Initialize(string title, string content, MessageBoxType type)
        {
            this.Title = title;
            this.Content = content;
            this.Type = type;
        }

        public void SubmitButton_clicked()
        {
            GameObject.Destroy(this.gameObject);
        }

        public void ExitButton_clicked()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}

