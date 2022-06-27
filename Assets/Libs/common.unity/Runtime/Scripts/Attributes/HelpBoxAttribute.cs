using System;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class HelpBoxAttribute : PropertyAttribute
    {
        public enum MessageType
        {
            None,
            Info,
            Warning,
            Error,
        }

        public readonly string Text;

        public readonly MessageType Type;

        public readonly bool Slim;

        public HelpBoxAttribute(string text, MessageType type, bool slim = false)
        {
            this.Text = text;
            this.Type = type;
            this.Slim = slim;
        }
    }
}
