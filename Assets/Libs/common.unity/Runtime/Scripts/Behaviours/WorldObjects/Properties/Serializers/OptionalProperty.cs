using System.Collections.Generic;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Serializers
{
    /// <summary>
    /// Optional property that can be assigned to a properties manager. Manager then processes the optional property whenever an object is send or received from a server.
    /// </summary>
    public abstract class OptionalProperty : MonoBehaviour
    {
        /// <summary>
        /// The method should deserialize a property from the dictionary, if the dictionary contains it, and process the data however it needs. 
        /// </summary>
        /// <param name="properties">Dictionary that may contain the property.</param>
        public abstract void Process(Dictionary<string, byte[]> properties);

        /// <summary>
        /// Converts this property to a byte array.
        /// </summary>
        /// <returns>The byte array.</returns>
        public abstract byte[] Serialize();

        /// <summary>
        /// Gets a name of this property.
        /// </summary>
        /// <returns>The name of this property.</returns>
        public abstract string GetPropertyName();
    }
}
