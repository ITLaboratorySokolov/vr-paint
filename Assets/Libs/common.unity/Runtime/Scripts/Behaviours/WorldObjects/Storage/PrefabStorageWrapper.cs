using UnityEngine;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Utility.Inventory;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Storage
{
    public class PrefabStorageWrapper : MonoBehaviour, IPrefabStorage
    {
        /// <summary>
        /// Prefabs of objects that are supported in this application. All prefabs must have IPropertiesManager component attached.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefabs of objects that are supported in this application")]
        [FormerlySerializedAs("supportedTypePrefabs")]
        private GameObjectInventory _supportedTypePrefabs;

        private PrefabStorage _prefabStorage;
        
        private void Awake()
        {
            _prefabStorage = new PrefabStorage(_supportedTypePrefabs.Items);
        }

        public bool GetTypePrefab(string name, out GameObject prefab)
        {
            return _prefabStorage.GetTypePrefab(name, out prefab);
        }

        public bool IsSupported(string name)
        {
            return _prefabStorage.IsSupported(name);
        }
    }
}