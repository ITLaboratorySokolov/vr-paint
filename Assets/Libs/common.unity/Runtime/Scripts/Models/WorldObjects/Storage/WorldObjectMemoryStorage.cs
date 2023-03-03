using System.Collections.Generic;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage
{
    public class WorldObjectMemoryStorage : IWorldObjectStorage
    {
        private readonly Dictionary<string, GameObject> _storedWorldObjects = new();
        
        /// <inheritdoc/>
        public IEnumerable<GameObject> ClearStorage()
        {
            var values = new List<GameObject>(_storedWorldObjects.Values);
            _storedWorldObjects.Clear();
            return values;
        }

        /// <inheritdoc/>
        public bool Get(string name, out GameObject worldObject)
        {
            return _storedWorldObjects.TryGetValue(name, out worldObject);
        }

        /// <inheritdoc/>
        public IEnumerable<GameObject> GetAll()
        {
            return _storedWorldObjects.Values;
        }

        /// <inheritdoc/>
        public bool IsStored(string name)
        {
            return _storedWorldObjects.ContainsKey(name);
        }

        /// <inheritdoc/>
        public bool Remove(string name, out GameObject worldObject)
        {
            return _storedWorldObjects.Remove(name, out worldObject);
        }

        /// <inheritdoc/>
        public bool Replace(GameObject worldObject, out GameObject oldWorldObject)
        {
            var success = _storedWorldObjects.TryGetValue(worldObject.name, out oldWorldObject);
            if (success)
            {
                _storedWorldObjects[worldObject.name] = worldObject;
            }

            return success;
        }

        /// <inheritdoc/>
        public bool Replace(GameObject worldObject)
        {
            return Replace(worldObject, out _);
        }

        /// <inheritdoc/>
        public bool Store(GameObject worldObject)
        {
            return _storedWorldObjects.TryAdd(worldObject.name, worldObject);
        }
    }
}