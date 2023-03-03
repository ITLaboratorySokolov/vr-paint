using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Storage
{
    /// <summary>
    /// Abstract class that describes functionality of a storage of world objects.
    /// </summary>
    public abstract class WorldObjectStorageWrapper : MonoBehaviour, IWorldObjectStorage
    {
        private IWorldObjectStorage _worldObjectStorage;
        
        private void Awake()
        {
            _worldObjectStorage = CreateStorage();
        }
        
        protected abstract IWorldObjectStorage CreateStorage();

        public IEnumerable<GameObject> ClearStorage()
        {
            return _worldObjectStorage.ClearStorage();
        }

        public bool Get(string name, out GameObject worldObject)
        {
            return _worldObjectStorage.Get(name, out worldObject);
        }

        public IEnumerable<GameObject> GetAll()
        {
            return _worldObjectStorage.GetAll();
        }

        public bool IsStored(string name)
        {
            return _worldObjectStorage.IsStored(name);
        }

        public bool Remove(string name, out GameObject worldObject)
        {
            return _worldObjectStorage.Remove(name, out worldObject);
        }

        public bool Replace(GameObject worldObject, out GameObject oldWorldObject)
        {
            return _worldObjectStorage.Replace(worldObject, out oldWorldObject);
        }

        public bool Replace(GameObject worldObject)
        {
            return _worldObjectStorage.Replace(worldObject);
        }

        public bool Store(GameObject worldObject)
        {
            return _worldObjectStorage.Store(worldObject);
        }
    }
}