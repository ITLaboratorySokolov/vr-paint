using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Storage
{
    /// <summary>
    /// Abstract class that describes functionality of a storage of world objects.
    /// </summary>
    public abstract class WorldObjectStorageWrapper : MonoBehaviour, IWorldObjectStorage
    {
        public event Action<GameObject> WorldObjectStored;
        public event Action<GameObject> WorldObjectRemoved;
        public event Action StorageCleared;

        [SerializeField] private UnityEvent<GameObject> _worldObjectStored = new();
        [SerializeField] private UnityEvent<GameObject> _worldObjectRemoved = new();
        [SerializeField] private UnityEvent _storageCleared = new();
        
        private IWorldObjectStorage _worldObjectStorage;

        public int Count => _worldObjectStorage.Count;
        
        private void Awake()
        {
            _worldObjectStorage = CreateStorage();
            _worldObjectStorage.WorldObjectStored += WorldObjectStorage_OnWorldObjectStored;
            _worldObjectStorage.WorldObjectRemoved += WorldObjectStorage_OnWorldObjectRemoved;
            _worldObjectStorage.StorageCleared += WorldObjectStorage_OnStorageCleared;
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
        
        private void WorldObjectStorage_OnStorageCleared()
        {
            StorageCleared?.Invoke();
            _storageCleared?.Invoke();
        }

        private void WorldObjectStorage_OnWorldObjectRemoved(GameObject obj)
        {
            WorldObjectRemoved?.Invoke(obj);
            _worldObjectRemoved?.Invoke(obj);
        }

        private void WorldObjectStorage_OnWorldObjectStored(GameObject obj)
        {
            WorldObjectStored?.Invoke(obj);
            _worldObjectStored?.Invoke(obj);
        }
    }
}