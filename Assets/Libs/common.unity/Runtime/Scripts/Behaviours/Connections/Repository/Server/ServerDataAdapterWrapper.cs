using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using ZCU.TechnologyLab.Common.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Data;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server
{
    /// <summary>
    /// Wrapper over <see cref="SessionDataAdapter"/>. This enables usage of <see cref="SessionDataAdapter"/> in a scene.
    /// </summary>
    public class ServerDataAdapterWrapper : MonoBehaviour, IServerDataAdapter
    {
        [HelpBox("Data Client has to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private DataClientWrapper dataClient;

        private ServerDataAdapter data;

        private void OnValidate()
        {
            Assert.IsNotNull(this.dataClient, "Data Client was null.");
        }

        private void Awake()
        {
            this.data = new ServerDataAdapter(this.dataClient);
        }

        public Task AddWorldObjectAsync(WorldObjectDto worldObject)
        {
            return this.data.AddWorldObjectAsync(worldObject);
        }

        public Task RemoveWorldObjectAsync(string name)
        {
            return this.data.RemoveWorldObjectAsync(name);
        }

        public Task<IEnumerable<WorldObjectDto>> GetAllWorldObjectsAsync()
        {
            return this.data.GetAllWorldObjectsAsync();
        }

        public Task<WorldObjectDto> GetWorldObjectAsync(string name)
        {
            return this.data.GetWorldObjectAsync(name);
        }

        public Task<WorldObjectPropertiesDto> GetWorldObjectPropertiesAsync(string name)
        {
            return this.data.GetWorldObjectPropertiesAsync(name);
        }

        public Task UpdateWorldObjectAsync(string name, WorldObjectUpdateDto worldObjectUpdate)
        {
            return this.data.UpdateWorldObjectAsync(name, worldObjectUpdate);
        }

        public Task UpdateWorldObjectPropertiesAsync(string name, WorldObjectPropertiesDto properties)
        {
            return this.data.UpdateWorldObjectPropertiesAsync(name, properties);
        }
    }
}
