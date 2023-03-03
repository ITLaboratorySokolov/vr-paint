using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage
{
    public interface IPrefabStorage
    {
        /// <summary>
        /// Gets a prefab of a world object if its type is supported.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        /// <param name="prefab">Prefab of the type.</param>
        /// <returns>True if the type is supported, false otherwise.</returns>
        public bool GetTypePrefab(string name, out GameObject prefab);

        /// <summary>
        /// Is a type of an object supported?
        /// </summary>
        /// <param name="name">Name of the type.</param>
        /// <returns>True, if the type is supported, false otherwise.</returns>
        public bool IsSupported(string name);
    }
}