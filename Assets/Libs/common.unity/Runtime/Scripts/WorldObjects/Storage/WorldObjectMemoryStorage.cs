using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Utility.Inventory;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Managers;

namespace ZCU.TechnologyLab.Common.Unity.WorldObjects.Storage
{
    /// <summary>
    /// Storage of world objects. World objects are saved in a dictionary.
    /// </summary>
    public class WorldObjectMemoryStorage : WorldObjectStorage
    {
        private readonly Dictionary<string, GameObject> storedWorldObjects = new Dictionary<string, GameObject>();

        /// <summary>
        /// Dictionary of objects that are supported in this application and can be sent to a server.
        /// </summary>
        private readonly Dictionary<string, GameObject> supportedTypes = new Dictionary<string, GameObject>();

        /// <summary>
        /// Prefabs of objects that are supported in this application. All prefabs must have IPropertiesManager component attached.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefabs of objects that are supported in this application")]
        private GameObjectInventory supportedTypePrefabs;

        private void Awake()
        {
            if(this.supportedTypePrefabs != null)
            {
                foreach (var prefab in this.supportedTypePrefabs.Items)
                {
                    IPropertiesManager propertiesManager = WorldObjectUtils.GetPropertiesManager(prefab);
                    this.supportedTypes.Add(propertiesManager.ManagedType, prefab);
                }
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<GameObject> ClearStorage()
        {
            var values = new List<GameObject>(this.storedWorldObjects.Values);
            this.storedWorldObjects.Clear();
            return values;
        }

        /// <inheritdoc/>
        public override bool Get(string name, out GameObject worldObject)
        {
            return this.storedWorldObjects.TryGetValue(name, out worldObject);
        }

        /// <inheritdoc/>
        public override bool GetTypePrefab(string name, out GameObject prefab)
        {
            return this.supportedTypes.TryGetValue(name, out prefab);
        }

        /// <inheritdoc/>
        public override IEnumerable<GameObject> GetAll()
        {
            return this.storedWorldObjects.Values;
        }

        /// <inheritdoc/>
        public override bool IsStored(string name)
        {
            return this.storedWorldObjects.ContainsKey(name);
        }

        /// <inheritdoc/>
        public override bool IsSupported(string name)
        {
            return this.supportedTypes.ContainsKey(name);
        }

        /// <inheritdoc/>
        public override bool Remove(string name, out GameObject worldObject)
        {
            return this.storedWorldObjects.Remove(name, out worldObject);
        }

        /// <inheritdoc/>
        public override bool Replace(GameObject worldObject, out GameObject oldWorldObject)
        {
            var success = this.storedWorldObjects.TryGetValue(worldObject.name, out oldWorldObject);
            if (success) this.storedWorldObjects[worldObject.name] = worldObject;
            return success;
        }

        /// <inheritdoc/>
        public override bool Replace(GameObject worldObject)
        {
            return this.Replace(worldObject, out _);
        }

        /// <inheritdoc/>
        public override bool Store(GameObject worldObject)
        {
            var success = this.storedWorldObjects.TryAdd(worldObject.name, worldObject);
            if (success) worldObject.transform.parent = this.transform;
            return success;
        }
    }
}
