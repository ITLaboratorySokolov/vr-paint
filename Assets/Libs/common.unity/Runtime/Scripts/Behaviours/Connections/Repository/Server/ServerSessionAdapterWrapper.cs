using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server
{
    /// <summary>
    /// Wrapper over <see cref="ServerSessionConnection"/>. This enables usage of <see cref="ServerSessionConnection"/> in a scene.
    /// </summary>
    public class ServerSessionAdapterWrapper : MonoBehaviour, IServerSessionAdapter
    {
        /// <inheritdoc/>
        public event Action<string> WorldObjectAdded
        {
            add => this.sessionAdapter.WorldObjectAdded += value;
            remove => this.sessionAdapter.WorldObjectAdded -= value;
        }
        
        /// <inheritdoc/>
        public event Action<string> WorldObjectPropertiesUpdated
        {
            add => this.sessionAdapter.WorldObjectPropertiesUpdated += value;
            remove => this.sessionAdapter.WorldObjectPropertiesUpdated -= value;
        }
        
        /// <inheritdoc/>
        public event Action<string> WorldObjectUpdated
        {
            add => this.sessionAdapter.WorldObjectUpdated += value;
            remove => this.sessionAdapter.WorldObjectUpdated -= value;
        }
        
        /// <inheritdoc/>
        public event Action<string> WorldObjectRemoved
        {
            add => this.sessionAdapter.WorldObjectRemoved += value;
            remove => this.sessionAdapter.WorldObjectRemoved -= value;
        }
        
        /// <inheritdoc/>
        public event Action<WorldObjectTransformDto> WorldObjectTransformed
        {
            add => this.sessionAdapter.WorldObjectTransformed += value;
            remove => this.sessionAdapter.WorldObjectTransformed -= value;
        }

        [HelpBox("Session Client has to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private SessionClientWrapper sessionClient;

        [Space]
        [SerializeField] 
        private UnityEvent<string> worldObjectAdded = new();

        [SerializeField] 
        private UnityEvent<string> worldObjectPropertiesUpdated = new();

        [SerializeField]
        private UnityEvent<string> worldObjectUpdated = new();

        [SerializeField]
        private UnityEvent<string> worldObjectRemoved = new();
        
        [SerializeField] 
        private UnityEvent<WorldObjectTransformDto> worldObjectTransformed = new();
        
        private ServerSessionAdapter sessionAdapter;

        private void OnValidate()
        {
            Assert.IsNotNull(this.sessionClient, "Session Client was null.");
        }

        private void Awake()
        {
            this.sessionAdapter = new ServerSessionAdapter(this.sessionClient);
        }

        private void Start()
        {
            if (this.worldObjectAdded.GetPersistentEventCount() > 0)
            {
                this.sessionAdapter.WorldObjectAdded += this.worldObjectAdded.Invoke;
            }
            
            if (this.worldObjectRemoved.GetPersistentEventCount() > 0)
            {
                this.sessionAdapter.WorldObjectRemoved += this.worldObjectRemoved.Invoke;
            }
            
            if (this.worldObjectTransformed.GetPersistentEventCount() > 0)
            {
                this.sessionAdapter.WorldObjectTransformed += this.worldObjectTransformed.Invoke;
            }
            
            if (this.worldObjectUpdated.GetPersistentEventCount() > 0)
            {
                this.sessionAdapter.WorldObjectUpdated += this.worldObjectUpdated.Invoke;
            }
            
            if (this.worldObjectPropertiesUpdated.GetPersistentEventCount() > 0)
            {
                this.sessionAdapter.WorldObjectPropertiesUpdated += this.worldObjectPropertiesUpdated.Invoke;
            }
        }

        private void OnDestroy()
        {
            if (this.worldObjectAdded.GetPersistentEventCount() > 0)
            {
                this.sessionAdapter.WorldObjectAdded -= this.worldObjectAdded.Invoke;
            }
            
            if (this.worldObjectRemoved.GetPersistentEventCount() > 0)
            {
                this.sessionAdapter.WorldObjectRemoved -= this.worldObjectRemoved.Invoke;
            }
            
            if (this.worldObjectTransformed.GetPersistentEventCount() > 0)
            {
                this.sessionAdapter.WorldObjectTransformed -= this.worldObjectTransformed.Invoke;
            }
            
            if (this.worldObjectUpdated.GetPersistentEventCount() > 0)
            {
                this.sessionAdapter.WorldObjectUpdated -= this.worldObjectUpdated.Invoke;
            }
            
            if (this.worldObjectPropertiesUpdated.GetPersistentEventCount() > 0)
            {
                this.sessionAdapter.WorldObjectPropertiesUpdated -= this.worldObjectPropertiesUpdated.Invoke;
            }
        }
        
        /// <inheritdoc/>
        public Task TransformWorldObjectAsync(string objectName, RemoteVectorDto position, RemoteVectorDto rotation,
            RemoteVectorDto scale)
        {
            return this.sessionAdapter.TransformWorldObjectAsync(objectName, position, rotation, scale);
        }

        /// <inheritdoc/>
        public Task TransformWorldObjectAsync(WorldObjectTransformDto worldObjectTransform)
        {
            return this.sessionAdapter.TransformWorldObjectAsync(worldObjectTransform);
        }
    }
}
