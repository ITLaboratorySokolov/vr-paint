using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage
{
    public class WorldObjectMemoryStorage : IWorldObjectStorage
    {
        public event Action<GameObject> WorldObjectStored;
        public event Action<GameObject> WorldObjectRemoved;
        public event Action StorageCleared;
        
        private readonly Dictionary<string, GameObject> _storedWorldObjects = new();

        public int Count => _storedWorldObjects.Count;
        
        /// <inheritdoc/>
        public IEnumerable<GameObject> ClearStorage()
        {
            var values = new List<GameObject>(_storedWorldObjects.Values);
            _storedWorldObjects.Clear();
            StorageCleared?.Invoke();
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
            var result = _storedWorldObjects.Remove(name, out worldObject);
            if (result)
            {
                WorldObjectRemoved?.Invoke(worldObject);
            }

            return result;
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
            var result = _storedWorldObjects.TryAdd(worldObject.name, worldObject);
            if (result)
            {
                WorldObjectStored?.Invoke(worldObject);
            }

            return result;
        }
    }
}