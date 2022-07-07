using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Attributes;
using ZCU.TechnologyLab.Common.Unity.Connections;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Managers;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Storage;

namespace ZCU.TechnologyLab.Common.Unity.WorldObjects
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
        [HelpBox("Server Data Connection has to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private ServerDataConnectionWrapper serverDataConnection;

        [Header("Storage")]
        [HelpBox("World Object Storage has to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private WorldObjectStorage worldObjectStorage;

        [Header("Events")]
        [SerializeField]
        private WorldObjectEventsHandler worldObjectEventsHandler;

        private void OnValidate()
        {
            Assert.IsNotNull(this.serverDataConnection, "Server Data Connection was null.");
            Assert.IsNotNull(this.worldObjectStorage, "World Object Storage was null.");
        }

        /// <summary>
        /// Adds an object to the manager and to a server.
        /// </summary>
        /// <param name="gameObject">Added game object.</param>
        /// <returns>A task.</returns>
        public async Task AddObjectAsync(GameObject worldObject)
        {
            var propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);

            var worldObjectDto = CreateWorldObjectDto(worldObject, propertiesManager);
            await this.serverDataConnection.Data.AddWorldObjectAsync(worldObjectDto);

            if(this.worldObjectEventsHandler != null)
            {
                this.worldObjectEventsHandler.AssignEventHandlers(worldObject);
            }
            

            if (!this.worldObjectStorage.Store(worldObject))
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
            await this.serverDataConnection.Data.RemoveWorldObjectAsync(objectName);

            if (this.worldObjectStorage.Remove(objectName, out GameObject worldObject))
            {
                if(this.worldObjectEventsHandler != null)
                {
                    this.worldObjectEventsHandler.RemoveEventHandlers(worldObject);
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
            if (!this.worldObjectStorage.Get(worldObject.name, out var oldWorldObject))
            {
                throw new ArgumentException($"GameObject with name {worldObject.name} is unknown");
            }

            var propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);

            // Send update that replaces old object to the new one on a server
            var worldObjectDto = this.CreateWorldObjectDto(worldObject, propertiesManager);
            await this.serverDataConnection.Data.UpdateWorldObjectAsync(worldObjectDto);

            if(this.worldObjectEventsHandler != null)
            {
                this.worldObjectEventsHandler.RemoveEventHandlers(oldWorldObject);
                this.worldObjectEventsHandler.AssignEventHandlers(worldObject);
            }
            
            this.worldObjectStorage.Replace(worldObject);

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
            if (!this.worldObjectStorage.Get(objectName, out var worldObject))
            {
                throw new ArgumentException($"GameObject with name {objectName} is unknown");
            }

            var propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);

            // Update object on a server
            var worldObjectDto = this.CreateWorldObjectDto(worldObject, propertiesManager);
            await this.serverDataConnection.Data.UpdateWorldObjectAsync(worldObjectDto);
        }

        /// <summary>
        /// Downloads content of a server and creates a local copy of all objects.
        /// </summary>
        /// <returns>A task.</returns>
        public async Task<IEnumerable<GameObject>> LoadServerContentAsync()
        {
            ClearLocalContent();

            IEnumerable<WorldObjectDto> allObjects = await this.serverDataConnection.Data.GetAllWorldObjectsAsync();

            Debug.Log("Load world objects");

            var worldObjects = new List<GameObject>();
            foreach (var worldObjectDto in allObjects)
            {
                GameObject worldObject = WorldObjectUtils.AddObject(this.worldObjectEventsHandler, this.worldObjectStorage, worldObjectDto);
                worldObjects.Add(worldObject);
            }

            return worldObjects;
        }

        /// <summary>
        /// Deletes all objects downloaded to the application.
        /// </summary>
        public void ClearLocalContent()
        {
            IEnumerable<GameObject> removedObjects = this.worldObjectStorage.ClearStorage();
            foreach (var removedObject in removedObjects)
            {
                if(this.worldObjectEventsHandler != null)
                {
                    this.worldObjectEventsHandler.RemoveEventHandlers(removedObject);
                }
                
                GameObject.Destroy(removedObject);
            }
        }

        /// <summary>
        /// Creates a data transfer object from a world object.
        /// </summary>
        /// <param name="worldObject">The world object.</param>
        /// <param name="propertiesManager">Properties manager of the world object.</param>
        /// <returns>The data transfer object.</returns>
        private WorldObjectDto CreateWorldObjectDto(GameObject worldObject, IPropertiesManager propertiesManager)
        {
            return new WorldObjectDto
            {
                Name = worldObject.name,
                Position = new RemoteVectorDto
                {
                    X = worldObject.transform.position.x,
                    Y = worldObject.transform.position.y,
                    Z = worldObject.transform.position.z
                },
                Rotation = new RemoteVectorDto
                {
                    X = worldObject.transform.rotation.eulerAngles.x,
                    Y = worldObject.transform.rotation.eulerAngles.y,
                    Z = worldObject.transform.rotation.eulerAngles.z
                },
                Scale = new RemoteVectorDto
                {
                    X = worldObject.transform.localScale.x,
                    Y = worldObject.transform.localScale.y,
                    Z = worldObject.transform.localScale.z
                },
                Type = propertiesManager.ManagedType,
                Properties = propertiesManager.GetProperties()
            };
        }
    }
}