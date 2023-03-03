using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server
{
    /// <summary>
    /// Wrapper over <see cref="ServerSessionAdapter"/>. This enables usage of <see cref="ServerSessionAdapter"/> in a scene.
    /// </summary>
    public class ServerSessionAdapterWrapper : MonoBehaviour, IServerSessionAdapter
    {
        /// <inheritdoc/>
        public event Action<string> WorldObjectAdded;

        /// <inheritdoc/>
        public event Action<string> WorldObjectPropertiesUpdated;

        /// <inheritdoc/>
        public event Action<string> WorldObjectUpdated;

        /// <inheritdoc/>
        public event Action<string> WorldObjectRemoved;

        /// <inheritdoc/>
        public event Action<WorldObjectTransformDto> WorldObjectTransformed;

        [HelpBox("Session Client has to be assigned.", HelpBoxAttribute.MessageType.Warning, true)] 
        [SerializeField]
        [FormerlySerializedAs("sessionClient")]
        private SessionClientWrapper _sessionClient;

        [Space] 
        [SerializeField] 
        [Tooltip("Event called when world object is added.")]
        [FormerlySerializedAs("worldObjectAdded")]
        private UnityEvent<string> _worldObjectAdded = new();

        [SerializeField]
        [Tooltip("Event called when properties are updated.")]
        [FormerlySerializedAs("worldObjectPropertiesUpdated")]
        private UnityEvent<string> _worldObjectPropertiesUpdated = new();

        [SerializeField]
        [Tooltip("Event called when world object is updated.")]
        [FormerlySerializedAs("worldObjectUpdated")]
        private UnityEvent<string> _worldObjectUpdated = new();

        [SerializeField]
        [Tooltip("Event called when world object is removed.")]
        [FormerlySerializedAs("worldObjectRemoved")]
        private UnityEvent<string> _worldObjectRemoved = new();

        [SerializeField] 
        [Tooltip("Event called when world object is transformed.")]
        [FormerlySerializedAs("worldObjectTransformed")]
        private UnityEvent<WorldObjectTransformDto> _worldObjectTransformed = new();

        private ServerSessionAdapter _sessionAdapter;

        private void OnValidate()
        {
            Assert.IsNotNull(_sessionClient, "Session Client was null.");
        }

        private void Awake()
        {
            _sessionAdapter = new ServerSessionAdapter(_sessionClient);
        }

        private void Start()
        {
            /*
             * Event handlers cannot be assigned in Awake.
             * Session may not be initialized at that moment
             * if Awake of the ServerSessionAdapterWrapper
             * is called first.
             */
            _sessionAdapter.WorldObjectAdded += OnWorldObjectAdded;
            _sessionAdapter.WorldObjectRemoved += OnWorldObjectRemoved;
            _sessionAdapter.WorldObjectTransformed += OnWorldObjectTransformed;
            _sessionAdapter.WorldObjectUpdated += OnWorldObjectUpdated;
            _sessionAdapter.WorldObjectPropertiesUpdated += OnWorldObjectPropertiesUpdated;
        }

        private void OnDestroy()
        {
            _sessionAdapter.WorldObjectAdded -= OnWorldObjectAdded;
            _sessionAdapter.WorldObjectRemoved -= OnWorldObjectRemoved;
            _sessionAdapter.WorldObjectTransformed -= OnWorldObjectTransformed;
            _sessionAdapter.WorldObjectUpdated -= OnWorldObjectUpdated;
            _sessionAdapter.WorldObjectPropertiesUpdated -= OnWorldObjectPropertiesUpdated;
        }

        /// <inheritdoc/>
        public Task TransformWorldObjectAsync(string objectName, RemoteVectorDto position, RemoteVectorDto rotation,
            RemoteVectorDto scale)
        {
            return _sessionAdapter.TransformWorldObjectAsync(objectName, position, rotation, scale);
        }

        /// <inheritdoc/>
        public Task TransformWorldObjectAsync(WorldObjectTransformDto worldObjectTransform)
        {
            return _sessionAdapter.TransformWorldObjectAsync(worldObjectTransform);
        }

        private void OnWorldObjectPropertiesUpdated(string objectName)
        {
            WorldObjectPropertiesUpdated?.Invoke(objectName);
            _worldObjectPropertiesUpdated.Invoke(objectName);
        }

        private void OnWorldObjectUpdated(string objectName)
        {
            WorldObjectUpdated?.Invoke(objectName);
            _worldObjectUpdated.Invoke(objectName);
        }

        private void OnWorldObjectAdded(string objectName)
        {
            WorldObjectAdded?.Invoke(objectName);
            _worldObjectAdded.Invoke(objectName);
        }

        private void OnWorldObjectRemoved(string objectName)
        {
            WorldObjectRemoved?.Invoke(objectName);
            _worldObjectRemoved.Invoke(objectName);
        }

        private void OnWorldObjectTransformed(WorldObjectTransformDto transformDto)
        {
            WorldObjectTransformed?.Invoke(transformDto);
            _worldObjectTransformed.Invoke(transformDto);
        }
    }
}