using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Storage;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties.Managers;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects
{
    /// <summary>
    /// The WorldObjectManager manages objects placed on a server. It downloads objects from the server when 
    /// <see cref="LoadServerContentAsync"/> is called. It does not download objects from the server automatically.
    /// 
    /// If you want to add a new object, you have to call <see cref="AddObjectAsync"/>. It will send object to the server
    /// and assign events to local updates from Unity app. These updates would be then automatically redirected to the server.
    /// If you change properties of the object via other means than <see cref="IPropertiesManager"/> the events will not be
    /// triggered and you have to update the object manually with <see cref="UpdateObjectAsync"/>.
    /// 
    /// If some other client sends an update to a server, objects either downloaded from the server or added to 
    /// the manager via <see cref="AddObjectAsync"/> will be updated automatically.
    /// </summary>
    public class WorldObjectManager : MonoBehaviour
    {
        [Header("Networking")]
        [HelpBox("Server Data Connection has to be assigned.", HelpBoxAttribute.MessageType.Warning)]
        [SerializeField]
        [FormerlySerializedAs("serverDataAdapter")]
        private ServerDataAdapterWrapper _serverDataAdapter;

        [Header("Storage")]
        [HelpBox("World Object Storage has to be assigned.", HelpBoxAttribute.MessageType.Warning)]
        [SerializeField]
        [FormerlySerializedAs("worldObjectStorage")]
        private WorldObjectStorageWrapper _worldObjectStorage;

        [HelpBox("Prefab Storage has to be assigned.", HelpBoxAttribute.MessageType.Warning)]
        [SerializeField]
        [FormerlySerializedAs("prefabStorage")]
        private PrefabStorageWrapper _prefabStorage;
        
        [Header("Events")]
        [SerializeField]
        [FormerlySerializedAs("worldObjectEventsHandler")]
        private WorldObjectEventsHandler _worldObjectEventsHandler;

        private void OnValidate()
        {
            Assert.IsNotNull(_serverDataAdapter, "Server Data Connection was null.");
            Assert.IsNotNull(_worldObjectStorage, "World Object Storage was null.");
        }

        /// <summary>
        /// Adds an object to the manager and to a server.
        /// </summary>
        /// <param name="worldObject">Added game object.</param>
        /// <returns>A task.</returns>
        public async Task AddObjectAsync(GameObject worldObject)
        {
            var propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);

            var worldObjectDto = CreateWorldObjectDto(worldObject, propertiesManager);
            await _serverDataAdapter.AddWorldObjectAsync(worldObjectDto);

            if(_worldObjectEventsHandler != null)
            {
                _worldObjectEventsHandler.AssignEventHandlers(worldObject);
            }
            
            if (!_worldObjectStorage.Store(worldObject))
            {
                throw new ArgumentException($"GameObject with name {worldObject.name} cannot be added");
            }
        }

        /// <summary>
        /// Deletes an object from the manager and sends delete message to a server.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns>The removed object.</returns>
        /// <exception cref="ArgumentException">Thrown when object is not added to the manager, or when the object does not have <see cref="IPropertiesManager"/> component.</exception>
        public async Task<GameObject> RemoveObjectAsync(string objectName)
        {
            await _serverDataAdapter.RemoveWorldObjectAsync(objectName);

            if (_worldObjectStorage.Remove(objectName, out GameObject worldObject))
            {
                if(_worldObjectEventsHandler != null)
                {
                    _worldObjectEventsHandler.RemoveEventHandlers(worldObject);
                }
            }
            else
            {
                throw new ArgumentException($"GameObject with name {objectName} is unknown");
            }

            return worldObject;
        }

        /// <summary>
        /// Replaces an object in the manager. Objects must exist when it is replaced.
        /// When objects are replaced old GameObject is returned.
        /// </summary>
        /// <param name="worldObject">New object.</param>
        /// <returns>Old object that was swapped.</returns>
        /// <exception cref="ArgumentException">Thrown when an object with a same name doesn't exist.</exception>
        public async Task<GameObject> UpdateObjectAsync(GameObject worldObject)
        {
            if (!_worldObjectStorage.Get(worldObject.name, out var oldWorldObject))
            {
                throw new ArgumentException($"GameObject with name {worldObject.name} is unknown");
            }

            var propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);

            // Send update that replaces old object to the new one on a server
            var worldObjectUpdateDto = CreateWorldObjectUpdateDto(worldObject, propertiesManager);
            await _serverDataAdapter.UpdateWorldObjectAsync(worldObject.name, worldObjectUpdateDto);

            if(_worldObjectEventsHandler != null)
            {
                _worldObjectEventsHandler.RemoveEventHandlers(oldWorldObject);
                _worldObjectEventsHandler.AssignEventHandlers(worldObject);
            }
            
            _worldObjectStorage.Replace(worldObject);

            return oldWorldObject;
        }

        /// <summary>
        /// Sends that an object has been updated to a server.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when object is not added to the manager, or when the object does not have <see cref="IPropertiesManager"/> component.</exception>
        public async Task UpdateObjectAsync(string objectName)
        {
            if (!_worldObjectStorage.Get(objectName, out var worldObject))
            {
                throw new ArgumentException($"GameObject with name {objectName} is unknown");
            }

            var propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);

            // Update object on a server
            var worldObjectUpdateDto = CreateWorldObjectUpdateDto(worldObject, propertiesManager);
            await _serverDataAdapter.UpdateWorldObjectAsync(worldObject.name, worldObjectUpdateDto);
        }

        /// <summary>
        /// Downloads content of a server and creates a local copy of all objects.
        /// </summary>
        /// <returns>A task.</returns>
        public async Task<IEnumerable<GameObject>> LoadServerContentAsync()
        {
            ClearLocalContent();

            var allObjects = await _serverDataAdapter.GetAllWorldObjectsAsync();

            Debug.Log("Load world objects");

            var worldObjects = new List<GameObject>();
            foreach (var worldObjectDto in allObjects)
            {
                try
                {
                    var worldObject = WorldObjectUtils.AddObject(_worldObjectEventsHandler,
                        _prefabStorage, _worldObjectStorage, worldObjectDto);

                    if (worldObject != null)
                    {
                        worldObjects.Add(worldObject);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message, this);
                    Debug.LogException(e, this);
                }
            }

            return worldObjects;
        }

        /// <summary>
        /// Deletes all objects downloaded to the application.
        /// </summary>
        public void ClearLocalContent()
        {
            var removedObjects = _worldObjectStorage.ClearStorage();
            foreach (var removedObject in removedObjects)
            {
                if(_worldObjectEventsHandler != null)
                {
                    _worldObjectEventsHandler.RemoveEventHandlers(removedObject);
                }
                
                Destroy(removedObject);
            }
        }

        /// <summary>
        /// Creates a data transfer object from a world object.
        /// </summary>
        /// <param name="worldObject">The world object.</param>
        /// <param name="propertiesManager">Properties manager of the world object.</param>
        /// <returns>The data transfer object.</returns>
        private static WorldObjectDto CreateWorldObjectDto(GameObject worldObject, IPropertiesManager propertiesManager)
        {
            var position = worldObject.transform.position;
            var rotation = worldObject.transform.rotation;
            var localScale = worldObject.transform.localScale;
            
            return new WorldObjectDto
            {
                Name = worldObject.name,
                Position = new RemoteVectorDto
                {
                    X = position.x,
                    Y = position.y,
                    Z = position.z
                },
                Rotation = new RemoteVectorDto
                {
                    X = rotation.eulerAngles.x,
                    Y = rotation.eulerAngles.y,
                    Z = rotation.eulerAngles.z
                },
                Scale = new RemoteVectorDto
                {
                    X = localScale.x,
                    Y = localScale.y,
                    Z = localScale.z
                },
                Type = propertiesManager.ManagedType,
                Properties = propertiesManager.GetProperties()
            };
        }
        
        /// <summary>
        /// Creates a data transfer object from a world object.
        /// </summary>
        /// <param name="worldObject">The world object.</param>
        /// <param name="propertiesManager">Properties manager of the world object.</param>
        /// <returns>The data transfer object.</returns>
        private static WorldObjectUpdateDto CreateWorldObjectUpdateDto(GameObject worldObject, IPropertiesManager propertiesManager)
        {
            var position = worldObject.transform.position;
            var rotation = worldObject.transform.rotation;
            var localScale = worldObject.transform.localScale;
            
            return new WorldObjectUpdateDto
            {
                Position = new RemoteVectorDto
                {
                    X = position.x,
                    Y = position.y,
                    Z = position.z
                },
                Rotation = new RemoteVectorDto
                {
                    X = rotation.eulerAngles.x,
                    Y = rotation.eulerAngles.y,
                    Z = rotation.eulerAngles.z
                },
                Scale = new RemoteVectorDto
                {
                    X = localScale.x,
                    Y = localScale.y,
                    Z = localScale.z
                },
                Type = propertiesManager.ManagedType,
                Properties = propertiesManager.GetProperties()
            };
        }
    }
}