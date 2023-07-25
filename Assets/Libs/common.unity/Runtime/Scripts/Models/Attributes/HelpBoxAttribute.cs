using System;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HelpBoxAttribute : PropertyAttribute
    {
        public enum MessageType
        {
            None,
            Info,
            Warning,
            Error,
        }

        public string Text { get; }

        public MessageType Type { get; }

        public HelpBoxAttribute(string text, MessageType type)
        {
            Text = text;
            Type = type;
        }
    }
}
