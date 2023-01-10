using System;
using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;
using ZCU.TechnologyLab.Common.Unity.Models.Utility.Events;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties.Managers;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers
{
    public class LeftHandPropertiesManager : MonoBehaviour, IPropertiesManager
    {
        /// <inheritdoc/>
        public event EventHandler<PropertiesChangedEventArgs> PropertiesChanged;

        /// <summary>
        /// Description of a type of this world object.
        /// </summary>
        private const string ManagedTypeDescription = "LeftHand";

        /// <inheritdoc/>
        public string ManagedType => ManagedTypeDescription;

        /// <inheritdoc/>
        public Dictionary<string, byte[]> GetProperties()
        {
            return new Dictionary<string, byte[]>();
        }

        /// <inheritdoc/>
        public void SetProperties(Dictionary<string, byte[]> properties)
        {
            // Hand does not contain any properties.
        }
    }
}
