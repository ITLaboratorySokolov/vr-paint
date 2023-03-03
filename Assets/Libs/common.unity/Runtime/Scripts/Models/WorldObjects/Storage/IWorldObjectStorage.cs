using System.Collections.Generic;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage
{
    public interface IWorldObjectStorage
    {
        /// <summary>
        /// Deletes all stored objects.
        /// </summary>
        public IEnumerable<GameObject> ClearStorage();
        
        /// <summary>
        /// Gets stored object with a provided name.
        /// </summary>
        /// <param name="name">The provided name.</param>
        /// <param name="worldObject">The requested object.</param>
        /// <returns>True if the object exists in the storage, false otherwise.</returns>
        public bool Get(string name, out GameObject worldObject);
        
        /// <summary>
        /// Gets all stored objects.
        /// </summary>
        /// <returns>The stored objects.</returns>
        public IEnumerable<GameObject> GetAll();
        
        /// <summary>
        /// Does the storage contain an object with a provided name?
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <returns>True if the storage contains the object, false otherwise.</returns>
        public bool IsStored(string name);
        
        /// <summary>
        /// Removes an object with a provided name.
        /// </summary>
        /// <param name="name">The provided name.</param>
        /// <param name="worldObject">The removed object.</param>
        /// <returns>True if the object exists in the storage, false otherwise.</returns>
        public bool Remove(string name, out GameObject worldObject);
        
        /// <summary>
        /// Replaces an object with its new version. Names of both objects must be the same. Otherwise an exception is thrown.
        /// The method returns the old object.
        /// </summary>
        /// <param name="worldObject">The new object.</param>
        /// <param name="oldWorldObject">The old object.</param>
        /// <returns>True if the object exists in the storage, false otherwise.</returns>
        public bool Replace(GameObject worldObject, out GameObject oldWorldObject);
        
        /// <summary>
        /// Replaces an object with its new version. Names of both objects must be the same. Otherwise an exception is thrown.
        /// </summary>
        /// <param name="worldObject">The new object.</param>
        /// <returns>True if the object exists in the storage, false otherwise.</returns>
        public bool Replace(GameObject worldObject);
        
        /// <summary>
        /// Adds new object to the storage.
        /// </summary>
        /// <param name="worldObject">Added object.</param>
        /// <returns>True if the object was successfully added to the storage, false otherwise.</returns>
        public bool Store(GameObject worldObject);
    }
}