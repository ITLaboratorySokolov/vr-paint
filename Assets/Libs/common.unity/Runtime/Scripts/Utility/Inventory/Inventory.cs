using System.Collections.Generic;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Utility.Inventory
{
    /// <summary>
    /// Inventory that can save list of items in assets.
    /// </summary>
    /// <typeparam name="T">Type of saved items.</typeparam>
    public class Inventory<T> : ScriptableObject
    {
        [SerializeField]
        private List<T> items = new List<T>();

        /// <summary>
        /// Gets collection of items.
        /// </summary>
        public IList<T> Items
        {
            get => this.items;
        }

        /// <summary>
        /// Gets an item on a given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The item on the given index.</returns>
        public T this[int index] => this.items[index];

    }
}
