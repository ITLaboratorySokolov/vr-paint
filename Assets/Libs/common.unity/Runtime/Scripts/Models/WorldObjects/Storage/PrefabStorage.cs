using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects;

namespace ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage
{
    public class PrefabStorage : IPrefabStorage
    {
        /// <summary>
        /// Dictionary of objects that are supported in this application and can be sent to a server.
        /// </summary>
        private readonly Dictionary<string, GameObject> _supportedTypes = new();

        public PrefabStorage(IEnumerable<GameObject> prefabs)
        {
            foreach (var prefab in prefabs)
            {
                var propertiesManager = WorldObjectUtils.GetPropertiesManager(prefab);
                _supportedTypes.Add(propertiesManager.ManagedType, prefab);
            }
        }
        
        /// <inheritdoc/>
        public bool GetTypePrefab(string name, out GameObject prefab)
        {
            return _supportedTypes.TryGetValue(name, out prefab);
        }
        
        /// <inheritdoc/>
        public bool IsSupported(string name)
        {
            return _supportedTypes.ContainsKey(name);
        }
    }
}