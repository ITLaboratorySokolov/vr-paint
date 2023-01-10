using System;
using UnityEditor;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Editor.Drawers.Attributes
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            var helpBoxAttribute = (HelpBoxAttribute)attribute;
            EditorGUI.HelpBox(position, helpBoxAttribute.Text, this.GetMessageType(helpBoxAttribute.Type));
        }

        public override float GetHeight()
        {
            var helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
            if (helpBoxStyle == null) return base.GetHeight();

            var helpBoxAttribute = (HelpBoxAttribute)attribute;
            return helpBoxAttribute.Slim 
                ? helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.Text), EditorGUIUtility.currentViewWidth) + 2
                : Mathf.Max(40f, helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.Text), EditorGUIUtility.currentViewWidth) + 4);
        }

        public UnityEditor.MessageType GetMessageType(HelpBoxAttribute.MessageType messageType)
        {
            switch (messageType)
            {
                case HelpBoxAttribute.MessageType.None: return MessageType.None;
                case HelpBoxAttribute.MessageType.Info: return MessageType.Info;
                case HelpBoxAttribute.MessageType.Warning: return MessageType.Warning;
                case HelpBoxAttribute.MessageType.Error: return MessageType.Error;
                default: throw new NotSupportedException("Message type is not supported");
            }
        }
    }
}