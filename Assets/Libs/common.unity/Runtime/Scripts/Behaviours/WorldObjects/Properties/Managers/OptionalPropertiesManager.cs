using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Serializers;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers
{
    public class OptionalPropertiesManager : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("optionalProperties")]
        private List<OptionalProperty> _optionalProperties = new();

        /// <summary>
        /// Gets list of optional properties.
        /// </summary>
        public IList<OptionalProperty> OptionalProperties => _optionalProperties;

        /// <param name="properties"></param>
        public void AddProperties(Dictionary<string, byte[]> properties)
        {
            foreach(var optionalProperty in _optionalProperties)
            {
                try
                {
                    properties.Add(optionalProperty.GetPropertyName(), optionalProperty.Serialize());
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Unable to serialize optional property: {ex.Message}");
                }
            }
        }
        
        public void SetProperties(Dictionary<string, byte[]> properties)
        {
            foreach(var optionalProperty in _optionalProperties)
            {
                try
                { 
                    optionalProperty.Process(properties);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Unable to process optional property: {ex.Message}");
                }
            }
        }
    }
}