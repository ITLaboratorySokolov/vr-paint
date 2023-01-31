using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Storage;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties.Managers;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects
{
    /// <summary>
    /// Class reacts to events that can happen on a technology lab server. 
    /// </summary>
    public class ServerEventsHandler : MonoBehaviour
    {
        [Header("Networking")]
        [HelpBox("Server Session Connection and Server Data Connection have to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private ServerSessionAdapterWrapper serverSessionAdapter;

        [SerializeField]
        private ServerDataAdapterWrapper serverDataAdapter;

        [Header("Storage")]
        [HelpBox("World Object Storage has to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private WorldObjectStorage worldObjectStorage;

        [Header("Events")]
        [SerializeField]
        private WorldObjectEventsHandler worldObjectEventsHandler;

        [Space]
        [SerializeField]
        private UnityEvent<GameObject> worldObjectAdded = new UnityEvent<GameObject>();

        [SerializeField]
        private UnityEvent<GameObject> worldObjectRemoved = new UnityEvent<GameObject>();

        [SerializeField]
        private UnityEvent<GameObject> worldObjectUpdated = new UnityEvent<GameObject>();

        [SerializeField]
        private UnityEvent<GameObject> worldObjectPropertiesUpdated = new UnityEvent<GameObject>();

        [SerializeField]
        private UnityEvent<GameObject> worldObjectTransformed = new UnityEvent<GameObject>();

        private void OnValidate()
        {
            Assert.IsNotNull(this.serverSessionAdapter, "Server Session Connection was null.");
            Assert.IsNotNull(this.serverDataAdapter, "Server Data Connection was null.");
            Assert.IsNotNull(this.worldObjectStorage, "World Object Storage was null.");
        }

        private void Start()
        {
            this.AssignConnectionEvents();
        }

        private void OnDestroy()
        {
            if (this.serverSessionAdapter != null)
            {
                this.serverSessionAdapter.WorldObjectAdded -= this.SessionConnection_WorldObjectAdded;
                this.serverSessionAdapter.WorldObjectRemoved -= this.SessionConnection_WorldObjectRemoved;
                this.serverSessionAdapter.WorldObjectUpdated -= this.SessionConnection_WorldObjectUpdated;
                this.serverSessionAdapter.WorldObjectPropertiesUpdated -= this.SessionConnection_WorldObjectPropertiesUpdated;
                this.serverSessionAdapter.WorldObjectTransformed -= this.SessionConnection_WorldObjectTransformed;
            }
        }

        /// <inheritdoc/>
        public void AssignConnectionEvents()
        {
            this.serverSessionAdapter.WorldObjectAdded += this.SessionConnection_WorldObjectAdded;
            this.serverSessionAdapter.WorldObjectRemoved += this.SessionConnection_WorldObjectRemoved;
            this.serverSessionAdapter.WorldObjectUpdated += this.SessionConnection_WorldObjectUpdated;
            this.serverSessionAdapter.WorldObjectPropertiesUpdated += this.SessionConnection_WorldObjectPropertiesUpdated;
            this.serverSessionAdapter.WorldObjectTransformed += this.SessionConnection_WorldObjectTransformed;
        }

        private async void SessionConnection_WorldObjectAdded(string worldObjectName)
        {
            try
            {
                Debug.Log("Add world object");
                Debug.Log($"Name: {worldObjectName}");

                if (!this.worldObjectStorage.IsStored(worldObjectName))
                {
                    // Download the object from the server
                    WorldObjectDto addedObjectDto = await this.serverDataAdapter.GetWorldObjectAsync(worldObjectName);
                    GameObject worldObject = WorldObjectUtils.AddObject(this.worldObjectEventsHandler, this.worldObjectStorage, addedObjectDto);
                    this.worldObjectAdded.Invoke(worldObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void SessionConnection_WorldObjectRemoved(string worldObjectName)
        {
            try
            {
                Debug.Log("Remove world object");
                Debug.Log($"Name: {worldObjectName}");

                if (this.worldObjectStorage.Remove(worldObjectName, out GameObject worldObject))
                {
                    if (this.worldObjectEventsHandler != null)
                    {
                        this.worldObjectEventsHandler.RemoveEventHandlers(worldObject);
                    }

                    this.worldObjectRemoved.Invoke(worldObject);
                    GameObject.Destroy(worldObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private async void SessionConnection_WorldObjectUpdated(string worldObjectName)
        {
            try
            {
                Debug.Log("Update world object");
                Debug.Log($"Name: {worldObjectName}");

                if (this.worldObjectStorage.Get(worldObjectName, out var worldObject))
                {
                    WorldObjectDto worldObjectDto = await this.serverDataAdapter.GetWorldObjectAsync(worldObjectName);

                    WorldObjectUtils.SetTransform(worldObject, worldObjectDto.Position, worldObjectDto.Rotation, worldObjectDto.Scale);

                    IPropertiesManager propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);
                    propertiesManager.SetProperties(worldObjectDto.Properties);

                    this.worldObjectUpdated.Invoke(worldObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private async void SessionConnection_WorldObjectPropertiesUpdated(string worldObjectName)
        {
            try
            {
                Debug.Log("Set world object properties");
                Debug.Log($"Name: {worldObjectName}");

                if (this.worldObjectStorage.Get(worldObjectName, out var worldObject))
                {
                    WorldObjectPropertiesDto propertiesDto = await this.serverDataAdapter.GetWorldObjectPropertiesAsync(worldObjectName);

                    IPropertiesManager propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);
                    propertiesManager.SetProperties(propertiesDto.Properties);

                    this.worldObjectPropertiesUpdated.Invoke(worldObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void SessionConnection_WorldObjectTransformed(WorldObjectTransformDto worldObjectTransformDto)
        {
            try
            {
                Debug.Log("Transform world object");
                Debug.Log($"Name: {worldObjectTransformDto.ObjectName}");

                if (this.worldObjectStorage.Get(worldObjectTransformDto.ObjectName, out var worldObject))
                {
                    WorldObjectUtils.SetTransform(worldObject, worldObjectTransformDto.Position, worldObjectTransformDto.Rotation, worldObjectTransformDto.Scale);
                    this.worldObjectTransformed.Invoke(worldObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}