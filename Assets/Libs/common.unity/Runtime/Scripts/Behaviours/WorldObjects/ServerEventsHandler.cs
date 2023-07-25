using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Storage;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects
{
    /// <summary>
    /// Class reacts to events that can happen on a technology lab server. 
    /// </summary>
    public class ServerEventsHandler : MonoBehaviour
    {
        [Header("Networking")]
        [HelpBox("Server Session Connection and Server Data Connection have to be assigned.", HelpBoxAttribute.MessageType.Warning)]
        [SerializeField]
        [FormerlySerializedAs("serverSessionAdapter")]
        private ServerSessionAdapterWrapper _serverSessionAdapter;

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
        private bool _reactToAdd = true;
        
        [SerializeField] 
        private bool _reactToRemove = true;
        
        [SerializeField] 
        private bool _reactToUpdate = true;
        
        [SerializeField] 
        private bool _reactToPropertiesUpdate = true;
        
        [SerializeField] 
        private bool _reactToTransform = true;
        
        [SerializeField]
        [FormerlySerializedAs("worldObjectEventsHandler")]
        private WorldObjectEventsHandler _worldObjectEventsHandler;

        [Space]
        [SerializeField]
        [FormerlySerializedAs("worldObjectAdded")]
        private UnityEvent<GameObject> _worldObjectAdded = new();

        [SerializeField]
        [FormerlySerializedAs("worldObjectRemoved")]
        private UnityEvent<GameObject> _worldObjectRemoved = new();

        [SerializeField]
        [FormerlySerializedAs("worldObjectUpdated")]
        private UnityEvent<GameObject> _worldObjectUpdated = new();

        [SerializeField]
        [FormerlySerializedAs("worldObjectPropertiesUpdated")]
        private UnityEvent<GameObject> _worldObjectPropertiesUpdated = new();

        [SerializeField]
        [FormerlySerializedAs("worldObjectTransformed")]
        private UnityEvent<GameObject> _worldObjectTransformed = new();

        private void OnValidate()
        {
            Assert.IsNotNull(_serverSessionAdapter, "Server Session Connection was null.");
            Assert.IsNotNull(_serverDataAdapter, "Server Data Connection was null.");
            Assert.IsNotNull(_worldObjectStorage, "World Object Storage was null.");
        }

        private void Start()
        {
            AssignConnectionEvents();
        }
        
        private void OnDestroy()
        {
            _serverSessionAdapter.WorldObjectAdded -= SessionConnection_WorldObjectAdded;
            _serverSessionAdapter.WorldObjectRemoved -= SessionConnection_WorldObjectRemoved;
            _serverSessionAdapter.WorldObjectUpdated -= SessionConnection_WorldObjectUpdated;
            _serverSessionAdapter.WorldObjectPropertiesUpdated -= SessionConnection_WorldObjectPropertiesUpdated;
            _serverSessionAdapter.WorldObjectTransformed -= SessionConnection_WorldObjectTransformed;
        }

        public void AssignConnectionEvents()
        {
            _serverSessionAdapter.WorldObjectAdded += SessionConnection_WorldObjectAdded;
            _serverSessionAdapter.WorldObjectRemoved += SessionConnection_WorldObjectRemoved;
            _serverSessionAdapter.WorldObjectUpdated += SessionConnection_WorldObjectUpdated;
            _serverSessionAdapter.WorldObjectPropertiesUpdated += SessionConnection_WorldObjectPropertiesUpdated;
            _serverSessionAdapter.WorldObjectTransformed += SessionConnection_WorldObjectTransformed;
        }

        private async void SessionConnection_WorldObjectAdded(string worldObjectName)
        {
            try
            {
                Debug.Log("Add world object");
                Debug.Log($"Name: {worldObjectName}");

                if (!_reactToAdd)
                {
                    Debug.Log("Reaction to add is disabled.");
                    return;
                }
                
                if (_worldObjectStorage.IsStored(worldObjectName))
                {
                    Debug.Log("World object already exists locally");
                    return;
                }
                
                // Download the object from the server
                var addedObjectDto = await _serverDataAdapter.GetWorldObjectAsync(worldObjectName);
                var worldObject = WorldObjectUtils.AddObject(
                    _worldObjectEventsHandler, 
                    _prefabStorage, 
                    _worldObjectStorage, 
                    addedObjectDto);

                if (worldObject != null)
                {
                    _worldObjectAdded.Invoke(worldObject);
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

                if (!_reactToRemove)
                {
                    Debug.Log("Reaction to remove is disabled.");
                    return;
                }
                
                if(_worldObjectStorage.Remove(worldObjectName, out var worldObject))
                {
                    if(_worldObjectEventsHandler != null)
                    {
                        _worldObjectEventsHandler.RemoveEventHandlers(worldObject);
                    }
                    
                    _worldObjectRemoved.Invoke(worldObject);
                    Destroy(worldObject);
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

                if (!_reactToUpdate)
                {
                    Debug.Log("Reaction to update is disabled.");
                    return;
                }
                
                if (_worldObjectStorage.Get(worldObjectName, out var worldObject))
                {
                    var worldObjectDto = await _serverDataAdapter.GetWorldObjectAsync(worldObjectName);

                    if (_reactToTransform)
                    {
                        WorldObjectUtils.SetTransform(
                            worldObject, 
                            worldObjectDto.Position, 
                            worldObjectDto.Rotation, 
                            worldObjectDto.Scale);
                    }
                    else
                    {
                        Debug.Log("Reaction to transform is disabled.");
                    }
                    
                    var propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);
                    propertiesManager.SetProperties(worldObjectDto.Properties);

                    _worldObjectUpdated.Invoke(worldObject);
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

                if (!_reactToPropertiesUpdate)
                {
                    Debug.Log("Reaction to properties update is disabled.");
                    return;
                }
                
                if (_worldObjectStorage.Get(worldObjectName, out var worldObject))
                {
                    var propertiesDto = await _serverDataAdapter.GetWorldObjectPropertiesAsync(worldObjectName);

                    var propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);
                    propertiesManager.SetProperties(propertiesDto.Properties);
                    
                    _worldObjectPropertiesUpdated.Invoke(worldObject);
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

                if (!_reactToTransform)
                {
                    Debug.Log("Reaction to transform is disabled.");
                    return;
                }
                
                if (_worldObjectStorage.Get(worldObjectTransformDto.ObjectName, out var worldObject))
                {
                    WorldObjectUtils.SetTransform(
                        worldObject, 
                        worldObjectTransformDto.Position, 
                        worldObjectTransformDto.Rotation, 
                        worldObjectTransformDto.Scale);
                    
                    _worldObjectTransformed.Invoke(worldObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}