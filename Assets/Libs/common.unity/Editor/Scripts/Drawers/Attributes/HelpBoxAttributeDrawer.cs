using System;
using UnityEditor;
using UnityEngine.UIElements;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Editor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeDrawer : DecoratorDrawer
    {
        public VisualElement CreatePropertyGUI() // override 
        {
            var helpBoxAttribute = (HelpBoxAttribute)attribute;
            var helpBox = new HelpBox(helpBoxAttribute.Text, GetMessageType(helpBoxAttribute.Type));
            return helpBox;
        }

        private static HelpBoxMessageType GetMessageType(HelpBoxAttribute.MessageType messageType)
        {
            return messageType switch
            {
                HelpBoxAttribute.MessageType.None => HelpBoxMessageType.None,
                HelpBoxAttribute.MessageType.Info => HelpBoxMessageType.Info,
                HelpBoxAttribute.MessageType.Warning => HelpBoxMessageType.Warning,
                HelpBoxAttribute.MessageType.Error => HelpBoxMessageType.Error,
                _ => throw new NotSupportedException("Message type is not supported")
            };
        }
    }
}