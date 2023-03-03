using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Utility.Inventory
{
    /// <summary>
    /// Inventory that can save list of items in assets.
    /// </summary>
    /// <typeparam name="T">Type of saved items.</typeparam>
    public class Inventory<T> : ScriptableObject
    {
        [SerializeField]
        [FormerlySerializedAs("items")]
        private List<T> _items = new();

        /// <summary>
        /// Gets collection of items.
        /// </summary>
        public IReadOnlyList<T> Items => _items;

        /// <summary>
        /// Gets an item on a given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The item on the given index.</returns>
        public T this[int index] => _items[index];
    }
}
