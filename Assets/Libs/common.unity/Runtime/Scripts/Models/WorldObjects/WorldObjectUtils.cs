using UnityEngine;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties.Managers;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage;

namespace ZCU.TechnologyLab.Common.Unity.Models.WorldObjects
{
    /// <summary>
    /// Utility function for world objects.
    /// </summary>
    public static class WorldObjectUtils
    {
        /// <summary>
        /// Sets transform to a game object.
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        public static void SetTransform(GameObject gameObject, RemoteVectorDto position, RemoteVectorDto rotation, RemoteVectorDto scale)
        {
            gameObject.transform.SetPositionAndRotation(
                new Vector3(position.X, position.Y, position.Z),
                Quaternion.Euler(rotation.X, rotation.Y, rotation.Z));

            gameObject.transform.localScale = new Vector3(scale.X, scale.Y, scale.Z);

            gameObject.transform.hasChanged = false;
        }

        /// <summary>
        /// Gets properties manager from a game object.
        /// </summary>
        /// <param name="worldObject">The game object.</param>
        /// <returns>The properties manager.</returns>
        /// <exception cref="MissingComponentException">Thrown if the object does not have the properties manager asigned.</exception>
        public static IPropertiesManager GetPropertiesManager(GameObject worldObject)
        {
            var propertiesManager = worldObject.GetComponent<IPropertiesManager>();
            if (propertiesManager == null)
            {
                throw new MissingComponentException($"GameObject does not contain component of type {nameof(IPropertiesManager)}");
            }

            return propertiesManager;
        }

        /// <summary>
        /// Instantiates object from a stored prefab and adds new object to the managed storage.
        /// </summary>
        /// <param name="worldObjectDto"></param>
        public static GameObject AddObject(WorldObjectEventsHandler worldObjectEventsHandler, IPrefabStorage prefabStorage, IWorldObjectStorage storage, WorldObjectDto worldObjectDto)
        {
            Debug.Log($"Properties count: {worldObjectDto.Properties.Count}");
            foreach (var property in worldObjectDto.Properties)
            {
                Debug.Log($"Property key: {property.Key}; Property value: {property.Value}");
            }

            if (!prefabStorage.GetTypePrefab(worldObjectDto.Type, out var prefab))
            {
                Debug.Log("Unsupported object type.");
                return null;
            }

            var worldObject = Object.Instantiate(prefab);
            worldObject.name = worldObjectDto.Name;

            SetTransform(worldObject, worldObjectDto.Position, worldObjectDto.Rotation, worldObjectDto.Scale);

            var propertiesManager = GetPropertiesManager(worldObject);
            propertiesManager.SetProperties(worldObjectDto.Properties);
            storage.Store(worldObject);

            if (worldObjectEventsHandler != null)
            {
                worldObjectEventsHandler.AssignEventHandlers(worldObject);
            }

            return worldObject;
        }
    }
}