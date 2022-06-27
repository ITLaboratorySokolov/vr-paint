using System.Collections.Generic;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.WorldObjects.Storage
{
    /// <summary>
    /// Abstract class that describes functionality of a storage of world objects.
    /// </summary>
    public abstract class WorldObjectStorage : MonoBehaviour
    {
        /// <summary>
        /// Deletes all stored objects.
        /// </summary>
        public abstract IEnumerable<GameObject> ClearStorage();

        /// <summary>
        /// Gets stored object with a provided name.
        /// </summary>
        /// <param name="name">The provided name.</param>
        /// <param name="worldObject">The requested object.</param>
        /// <returns>True if the object exists in the storage, false otherwise.</returns>
        public abstract bool Get(string name, out GameObject worldObject);

        /// <summary>
        /// Gets a prefab of a world object if its type is supported.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="prefab">Prefab of the type.</param>
        /// <returns>True if the type is supported, false otherwise.</returns>
        public abstract bool GetTypePrefab(string name, out GameObject prefab);

        /// <summary>
        /// Gets all stored objects.
        /// </summary>
        /// <returns>The stored objects.</returns>
        public abstract IEnumerable<GameObject> GetAll();

        /// <summary>
        /// Does the storage contain an object with a provided name?
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <returns>True if the storage contains the object, false otherwise.</returns>
        public abstract bool IsStored(string name);

        /// <summary>
        /// Is a type of an object supported?
        /// </summary>
        /// <param name="name">Name of the type.</param>
        /// <returns>True, if the type is supported, false otherwise.</returns>
        public abstract bool IsSupported(string name);

        /// <summary>
        /// Removes an object with a provided name.
        /// </summary>
        /// <param name="name">The provided name.</param>
        /// <param name="worldObject">The removed object.</param>
        /// <returns>True if the object exists in the storage, false otherwise.</returns>
        public abstract bool Remove(string name, out GameObject worldObject);

        /// <summary>
        /// Replaces an object with its new version. Names of both objects must be the same. Otherwise an exception is thrown.
        /// The method returns the old object.
        /// </summary>
        /// <param name="worldObject">The new object.</param>
        /// <param name="oldWorldObject">The old object.</param>
        /// <returns>True if the object exists in the storage, false otherwise.</returns>
        public abstract bool Replace(GameObject worldObject, out GameObject oldWorldObject);

        /// <summary>
        /// Replaces an object with its new version. Names of both objects must be the same. Otherwise an exception is thrown.
        /// </summary>
        /// <param name="worldObject">The new object.</param>
        /// <returns>True if the object exists in the storage, false otherwise.</returns>
        public abstract bool Replace(GameObject worldObject);

        /// <summary>
        /// Adds new object to the storage.
        /// </summary>
        /// <param name="worldObject">Added object.</param>
        /// <returns>True if the object was successfully added to the storage, false otherwise.</returns>
        public abstract bool Store(GameObject worldObject);
    }
}
