using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Connections;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Connections.Data;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;
using ZCU.TechnologyLab.Common.Unity.Utility.Events;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties;

namespace ZCU.TechnologyLab.Common.Unity.WorldObjects
{
    /// <summary>
    /// The WorldObjectManager manages objects placed on a server. It downaloads objects from the server when 
    /// <see cref="CopyServerContentAsync"/> is called. It does not download objects from the server automatically.
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
        /// <summary>
        /// Dictionary of all objects that are placed in this space.
        /// </summary>
        private readonly Dictionary<string, GameObject> worldObjects = new Dictionary<string, GameObject>();

        /// <summary>
        /// Supported world objects.
        /// </summary>
        private readonly Dictionary<string, Type> supportedTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Connection to a server via a session client.
        /// </summary>
        [SerializeField]
        private SessionClientWrapper sessionClient;

        /// <summary>
        /// Connection to a server via a data client.
        /// </summary>
        [SerializeField]
        private DataClientWrapper dataClient;

        /// <summary>
        /// Event that is called when a new object is received from a server.
        /// </summary>
        [SerializeField]
        private UnityEvent<GameObject> OnWorldObjectReceived = new UnityEvent<GameObject>();

        [SerializeField]
        private UnityEvent<GameObject> OnWorldObjectRemoved = new UnityEvent<GameObject>();

        [SerializeField]
        private UnityEvent OnClearLocalObjects = new UnityEvent();

        [SerializeField]
        private UnityEvent<GameObject, GameObject> OnReplaceObjects = new UnityEvent<GameObject, GameObject>();

        /// <summary>
        /// Session connection to a server.
        /// </summary>
        private ServerSessionConnection sessionConnection;

        /// <summary>
        /// Data connection to a server.
        /// </summary>
        private ServerDataConnection dataConnection;
        
        /// <summary>
        /// Inicialize private members.
        /// </summary>
        private void Awake()
        {
            this.supportedTypes.Add(MeshPropertiesManager.ManagedTypeDescription, typeof(MeshPropertiesManager));
            this.supportedTypes.Add(BitmapPropertiesManager.ManagedTypeDescription, typeof(BitmapPropertiesManager));
        }

        /// <summary>
        /// Assigns actions to events on start.
        /// </summary>
        private void Start()
        {
            this.sessionConnection = new ServerSessionConnection(sessionClient);
            this.dataConnection = new ServerDataConnection(dataClient);

            this.sessionConnection.WorldObjectAdded += SessionConnection_WorldObjectAdded;
            this.sessionConnection.WorldObjectRemoved += SessionConnection_WorldObjectRemoved;
            this.sessionConnection.WorldObjectUpdated += SessionConnection_WorldObjectUpdated;
            this.sessionConnection.WorldObjectPropertiesUpdated += SessionConnection_WorldObjectPropertiesUpdated;
            this.sessionConnection.WorldObjectTransformed += SessionConnection_WorldObjectTransformed;
        }

        /// <summary>
        /// Free all event handlers to avoid memory leaks.
        /// </summary>
        private void OnDestroy()
        {
            this.sessionConnection.WorldObjectAdded -= SessionConnection_WorldObjectAdded;
            this.sessionConnection.WorldObjectRemoved -= SessionConnection_WorldObjectRemoved;
            this.sessionConnection.WorldObjectUpdated -= SessionConnection_WorldObjectUpdated;
            this.sessionConnection.WorldObjectPropertiesUpdated -= SessionConnection_WorldObjectPropertiesUpdated;
            this.sessionConnection.WorldObjectTransformed -= SessionConnection_WorldObjectTransformed;

            this.FreeWorldObjectsEvents();
        }

        /// <summary>
        /// Adds an object to the manager and to a server.
        /// </summary>
        /// <param name="gameObject">Added game object.</param>
        /// <returns>A task.</returns>
        /// <exception cref="ArgumentException">Thrown when the object does not have <see cref="IPropertiesManager"/> component.</exception>
        public async Task AddObjectAsync(GameObject gameObject)
        {
            var propertiesManager = gameObject.GetComponent<IPropertiesManager>();
            if (propertiesManager == null)
            {
                throw new ArgumentException($"GameObject does not contain component of type {nameof(IPropertiesManager)}");
            }

            propertiesManager.PropertiesChanged += PropertiesManager_PropertiesChanged;

            var transformReport = gameObject.GetComponent<ReportTransformChange>();
            if(transformReport != null)
            {
                transformReport.TransformChanged += TransformReport_TransformChanged;
            }

            gameObject.transform.parent = this.transform;
            this.worldObjects.Add(gameObject.name, gameObject);

            var worldObjectDto = new WorldObjectDto
            {
                Name = gameObject.name,
                Position = new RemoteVectorDto
                {
                    X = gameObject.transform.position.x,
                    Y = gameObject.transform.position.y,
                    Z = gameObject.transform.position.z
                },
                Rotation = new RemoteVectorDto
                {
                    X = gameObject.transform.rotation.eulerAngles.x,
                    Y = gameObject.transform.rotation.eulerAngles.y,
                    Z = gameObject.transform.rotation.eulerAngles.z
                },
                Scale = new RemoteVectorDto
                {
                    X = gameObject.transform.localScale.x,
                    Y = gameObject.transform.localScale.y,
                    Z = gameObject.transform.localScale.z
                },
                Type = propertiesManager.ManagedType,
                Properties = propertiesManager.GetProperties()
            };

            await this.dataConnection.AddWorldObjectAsync(worldObjectDto);
        }

        /// <summary>
        /// Deletes an object from the manager and sends delete message to a server.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when object is not added to the manager, or when the object does not have <see cref="IPropertiesManager"/> component.</exception>
        public async Task RemoveObjectAsync(string objectName)
        {
            if (this.worldObjects.Remove(objectName, out GameObject worldObject))
            {
                var propertiesManager = worldObject.GetComponent<IPropertiesManager>();
                if (propertiesManager == null)
                {
                    throw new ArgumentException($"GameObject does not contain component of type {nameof(IPropertiesManager)}");
                }

                propertiesManager.PropertiesChanged -= PropertiesManager_PropertiesChanged;

                var transformReport = worldObject.GetComponent<ReportTransformChange>();
                if (transformReport != null) 
                {
                    transformReport.TransformChanged -= TransformReport_TransformChanged;
                }

                this.OnWorldObjectRemoved.Invoke(worldObject);

                Destroy(worldObject);

                await this.dataConnection.RemoveWorldObjectAsync(objectName);
            } else
            {
                throw new ArgumentException($"GameObject with name {objectName} is unknown");
            }
        }

        public async Task ReplaceObjectAsync(GameObject worldObject)
        {
            if (!this.worldObjects.TryGetValue(worldObject.name, out var oldWorldObject))
            {
                throw new ArgumentException($"GameObject with name {worldObject.name} is unknown");
            }

            this.worldObjects[worldObject.name] = worldObject;

            var propertiesManager = worldObject.GetComponent<IPropertiesManager>();
            if (propertiesManager == null)
            {
                throw new ArgumentException($"GameObject does not contain component of type {nameof(IPropertiesManager)}");
            }

            propertiesManager.PropertiesChanged += PropertiesManager_PropertiesChanged;

            var transformReport = worldObject.GetComponent<ReportTransformChange>();
            if (transformReport != null)
            {
                transformReport.TransformChanged += TransformReport_TransformChanged;
            }

            worldObject.transform.parent = this.transform;

            this.OnReplaceObjects.Invoke(oldWorldObject, worldObject);

            var oldPropertiesManager = oldWorldObject.GetComponent<IPropertiesManager>();
            if (oldPropertiesManager == null)
            {
                throw new ArgumentException($"GameObject does not contain component of type {nameof(IPropertiesManager)}");
            }

            oldPropertiesManager.PropertiesChanged -= PropertiesManager_PropertiesChanged;

            var oldTransformReport = oldWorldObject.GetComponent<ReportTransformChange>();
            if (oldTransformReport != null)
            {
                oldTransformReport.TransformChanged -= TransformReport_TransformChanged;
            }

            GameObject.Destroy(oldWorldObject);
            await this.UpdateObjectAsync(worldObject.name);
        }

        /// <summary>
        /// Updates an object in the manager and sends update to a server.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when object is not added to the manager, or when the object does not have <see cref="IPropertiesManager"/> component.</exception>
        public async Task UpdateObjectAsync(string objectName)
        {
            if (!this.worldObjects.TryGetValue(objectName, out var worldObject))
            {
                throw new ArgumentException($"GameObject with name {objectName} is unknown");
            }

            var propertiesManager = worldObject.GetComponent<IPropertiesManager>();
            if (propertiesManager == null)
            {
                throw new ArgumentException($"GameObject does not contain component of type {nameof(IPropertiesManager)}");
            }

            var worldObjectDto = new WorldObjectDto
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

            await this.dataConnection.UpdateWorldObjectAsync(worldObjectDto);
        }

        /// <summary>
        /// Downloads content of a server and creates a local copy of all objects.
        /// </summary>
        /// <returns>A task.</returns>
        public async Task CopyServerContentAsync()
        {
            this.ClearLocalCopy();

            IEnumerable<WorldObjectDto> allObjects = await this.dataConnection.GetAllWorldObjectsAsync();

            Debug.Log("Load world objects");

            foreach (var worldObject in allObjects)
            {
                this.AddObject(worldObject);
            }
        }

        /// <summary>
        /// Deletes all objects downloaded to the application.
        /// </summary>
        public void ClearLocalCopy()
        {
            this.OnClearLocalObjects.Invoke();
            this.FreeWorldObjectsEvents();
            this.worldObjects.Clear();

            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Frees events from all managed objects.
        /// </summary>
        private void FreeWorldObjectsEvents()
        {
            foreach (var worldObject in this.worldObjects.Values)
            {
                var propertiesManager = gameObject.GetComponent<IPropertiesManager>();
                if (propertiesManager != null)
                {
                    propertiesManager.PropertiesChanged -= PropertiesManager_PropertiesChanged;
                }

                var transformReport = worldObject.GetComponent<ReportTransformChange>();
                if (transformReport != null)
                {
                    transformReport.TransformChanged -= TransformReport_TransformChanged;
                }
            }
        }

        /// <summary>
        /// Sets transform to a game object.
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        private void SetTransform(GameObject gameObject, RemoteVectorDto position, RemoteVectorDto rotation, RemoteVectorDto scale)
        {
            gameObject.transform.SetPositionAndRotation(
                new Vector3(position.X, position.Y, position.Z),
                Quaternion.Euler(rotation.X, rotation.Y, rotation.Z));

            gameObject.transform.localScale = new Vector3(scale.X, scale.Y, scale.Z);
        }

        /// <summary>
        /// Adds 
        /// </summary>
        /// <param name="worldObjectDto"></param>
        private void AddObject(WorldObjectDto worldObjectDto)
        {
            Debug.Log($"Properties count: {worldObjectDto.Properties.Count}");
            foreach (var property in worldObjectDto.Properties)
            {
                Debug.Log($"Property key: {property.Key}; Property value: {property.Value}");
            }

            if (this.supportedTypes.TryGetValue(worldObjectDto.Type, out Type type))
            {
                var gameObject = new GameObject(worldObjectDto.Name);
                gameObject.transform.parent = this.transform;

                this.SetTransform(gameObject, worldObjectDto.Position, worldObjectDto.Rotation, worldObjectDto.Scale);

                IPropertiesManager worldObject = (IPropertiesManager)gameObject.AddComponent(type);
                worldObject.SetProperties(worldObjectDto.Properties);
                this.worldObjects.Add(worldObjectDto.Name, gameObject);

                var transformChanged = gameObject.AddComponent<ReportTransformChange>();
                transformChanged.TransformChanged += TransformReport_TransformChanged;

                this.OnWorldObjectReceived.Invoke(gameObject);
            }
        }


        #region Session handlers

        /// <summary>
        /// Downloads added object from a server and adds it to the manager.
        /// </summary>
        /// <param name="worldObjectName">World object that should be added.</param>
        private async void SessionConnection_WorldObjectAdded(string worldObjectName)
        {
            try
            {
                Debug.Log("Add world object");
                Debug.Log($"Name: {worldObjectName}");

                if(!this.worldObjects.ContainsKey(worldObjectName))
                {
                    // Download the object from the server
                    WorldObjectDto addedObject = await this.dataConnection.GetWorldObjectAsync(worldObjectName);
                    this.AddObject(addedObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Removes a world object when it was deleted from a server.
        /// </summary>
        /// <param name="worldObjectName">Name of the world object.</param>
        private void SessionConnection_WorldObjectRemoved(string worldObjectName)
        {
            try
            {
                Debug.Log("Remove world object");
                Debug.Log($"Name: {worldObjectName}");
                if (this.worldObjects.TryGetValue(worldObjectName, out var worldObject))
                {
                    this.OnWorldObjectRemoved.Invoke(worldObject);
                    GameObject.Destroy(worldObject);
                    this.worldObjects.Remove(worldObjectName);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Updates a world object when it was updated on a server.
        /// </summary>
        /// <param name="worldObjectName">Name of the world object.</param>
        private async void SessionConnection_WorldObjectUpdated(string worldObjectName)
        {
            try
            {
                Debug.Log("Update world object");
                Debug.Log($"Name: {worldObjectName}");
                if (this.worldObjects.TryGetValue(worldObjectName, out var worldObject))
                {
                    WorldObjectDto worldObjectDto = await this.dataConnection.GetWorldObjectAsync(worldObjectName);

                    this.SetTransform(worldObject, worldObjectDto.Position, worldObjectDto.Rotation, worldObjectDto.Scale);

                    var worldObjectType = worldObject.GetComponent<IPropertiesManager>();
                    if (worldObjectType != null)
                    {
                        worldObjectType.SetProperties(worldObjectDto.Properties);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Sets propeties of a world object.
        /// </summary>
        /// <param name="worldObjectName">Name of the world object.</param>
        private async void SessionConnection_WorldObjectPropertiesUpdated(string worldObjectName)
        {
            try
            {
                Debug.Log("Set world object properties");
                Debug.Log($"Name: {worldObjectName}");

                if (this.worldObjects.TryGetValue(worldObjectName, out var worldObject))
                {
                    WorldObjectPropertiesDto propertiesDto = await this.dataConnection.GetWorldObjectPropertiesAsync(worldObjectName);
                    var worldObjectType = worldObject.GetComponent<IPropertiesManager>();
                    if (worldObjectType != null)
                    {
                        worldObjectType.SetProperties(propertiesDto.Properties);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Transforms an object.
        /// </summary>
        /// <param name="worldObjectTransformDto">Transform of an object.</param>
        private void SessionConnection_WorldObjectTransformed(WorldObjectTransformDto worldObjectTransformDto)
        {
            try
            {
                Debug.Log("Transform world object");
                Debug.Log($"Name: {worldObjectTransformDto.ObjectName}");
                if (this.worldObjects.TryGetValue(worldObjectTransformDto.ObjectName, out var gameObject))
                {
                    this.SetTransform(gameObject, worldObjectTransformDto.Position, worldObjectTransformDto.Rotation, worldObjectTransformDto.Scale);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        #endregion


        #region Object properties handlers

        /// <summary>
        /// Reports transform changes to a server.
        /// </summary>
        /// <param name="sender">Object that was transformed.</param>
        /// <param name="e">Transform changed arguments.</param>
        private async void TransformReport_TransformChanged(object sender, TransformChangedEventArgs e)
        {
            try
            {
                Debug.Log($"Transform of {e.Transform.gameObject.name} changed. Update server");

                var transformDto = new WorldObjectTransformDto
                {
                    ObjectName = e.Transform.gameObject.name,
                    Position = new RemoteVectorDto
                    {
                        X = e.Transform.position.x,
                        Y = e.Transform.position.y,
                        Z = e.Transform.position.z
                    },
                    Rotation = new RemoteVectorDto
                    {
                        X = e.Transform.rotation.eulerAngles.x,
                        Y = e.Transform.rotation.eulerAngles.y,
                        Z = e.Transform.rotation.eulerAngles.z
                    },
                    Scale = new RemoteVectorDto
                    {
                        X = e.Transform.localScale.x,
                        Y = e.Transform.localScale.y,
                        Z = e.Transform.localScale.z
                    }
                };

                await this.sessionConnection.TransformWorldObjectAsync(transformDto);
            }
            catch (Exception ex)
            {
                Debug.Log("Unable to update transform on a server.", this);
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Reports changes to properties to a server.
        /// </summary>
        /// <param name="sender">Object, whose properties were updated.</param>
        /// <param name="e">Properties changed arguments.</param>
        private async void PropertiesManager_PropertiesChanged(object sender, PropertiesChangedEventArgs e)
        {
            try
            {
                Debug.Log($"Properties of {e.ObjectName} changed. Update server");

                var propertiesDto = new WorldObjectPropertiesDto
                {
                    Properties = e.Properties
                };

                await this.dataConnection.UpdateWorldObjectPropertiesAsync(e.ObjectName, propertiesDto);
            }
            catch (Exception ex)
            {
                Debug.Log("Unable to update property on a server.", this);
                Debug.LogException(ex);
            }
        }
        #endregion
    }
}