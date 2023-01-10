using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.UI.MessageBox
{
    /// <summary>
    /// Abstract class of a message box that can be shown on screen.
    /// </summary>
    public abstract class MessageBox : MonoBehaviour
    {
        /// <summary>
        /// Type of a message box.
        /// </summary>
        public enum MessageBoxType
        {
            Error,
            Warning,
            Information
        }

        /// <summary>
        /// Title of the message box.
        /// </summary>
        public abstract string Title { get; set; }

        /// <summary>
        /// Type of the message box.
        /// </summary>
        public abstract MessageBoxType Type { get; set; }

        /// <summary>
        /// Text content of the message box.
        /// </summary>
        public abstract string Content { get; set; }

        /// <summary>
        /// Initializes all properties of the message box.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="content">Content text.</param>
        /// <param name="type">Type.</param>
        public void Initialize(string title, string content, MessageBoxType type)
        {
            this.Title = title;
            this.Content = content;
            this.Type = type;
        }

        /// <summary>
        /// Submit button action that closes the message box.
        /// </summary>
        public void SubmitButton_clicked()
        {
            Destroy(this.gameObject);
        }

        /// <summary>
        /// Exit button action that closes the message box.
        /// </summary>
        public void ExitButton_clicked()
        {
            Destroy(this.gameObject);
        }
    }
}

