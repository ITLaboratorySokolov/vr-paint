using System;
using UnityEditor;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Editor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeDrawer : DecoratorDrawer
    {
        private const string StyleName = "helpbox";

        public override void OnGUI(Rect position)
        {
            var helpBoxAttribute = (HelpBoxAttribute)attribute;
            EditorGUI.HelpBox(position, helpBoxAttribute.Text, GetMessageType(helpBoxAttribute.Type));
        }

        public override float GetHeight()
        {
            var helpBoxStyle = GUI.skin != null ? GUI.skin.GetStyle(StyleName) : null;
            if (helpBoxStyle == null) return base.GetHeight();

            var helpBoxAttribute = (HelpBoxAttribute)attribute;
            return helpBoxAttribute.Slim 
                ? CalculateSlimHeight(helpBoxStyle, helpBoxAttribute)
                : CalculateNormalHeight(helpBoxStyle, helpBoxAttribute);
        }

        private static float CalculateNormalHeight(GUIStyle helpBoxStyle, HelpBoxAttribute helpBoxAttribute)
        {
            return Mathf.Max(40f, CalculateSlimHeight(helpBoxStyle, helpBoxAttribute) + 2);
        }

        private static float CalculateSlimHeight(GUIStyle helpBoxStyle, HelpBoxAttribute helpBoxAttribute)
        {
            return helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.Text), EditorGUIUtility.currentViewWidth) + 2;
        }

        private static MessageType GetMessageType(HelpBoxAttribute.MessageType messageType)
        {
            return messageType switch
            {
                HelpBoxAttribute.MessageType.None => MessageType.None,
                HelpBoxAttribute.MessageType.Info => MessageType.Info,
                HelpBoxAttribute.MessageType.Warning => MessageType.Warning,
                HelpBoxAttribute.MessageType.Error => MessageType.Error,
                _ => throw new NotSupportedException("Message type is not supported")
            };
        }
    }
}