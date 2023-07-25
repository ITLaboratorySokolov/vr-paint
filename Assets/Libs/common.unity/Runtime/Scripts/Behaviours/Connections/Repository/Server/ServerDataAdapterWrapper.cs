using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Data;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server
{
    /// <summary>
    /// Wrapper over <see cref="ServerDataAdapter"/>. This enables usage of <see cref="ServerDataAdapter"/> in a scene.
    /// </summary>
    public class ServerDataAdapterWrapper : MonoBehaviour, IServerDataAdapter
    {
        [HelpBox("Data Client has to be assigned.", HelpBoxAttribute.MessageType.Warning)]
        [SerializeField]
        [FormerlySerializedAs("dataClient")]
        private DataClientWrapper _dataClient;

        private ServerDataAdapter _dataAdapter;

        private void OnValidate()
        {
            Assert.IsNotNull(_dataClient, "Data Client was null.");
        }

        private void Awake()
        {
            _dataAdapter = new ServerDataAdapter(_dataClient);
        }

        public Task AddWorldObjectAsync(WorldObjectDto worldObject)
        {
            return _dataAdapter.AddWorldObjectAsync(worldObject);
        }

        public Task RemoveWorldObjectAsync(string name)
        {
            return _dataAdapter.RemoveWorldObjectAsync(name);
        }

        public Task<IEnumerable<WorldObjectDto>> GetAllWorldObjectsAsync()
        {
            return _dataAdapter.GetAllWorldObjectsAsync();
        }

        public Task<WorldObjectDto> GetWorldObjectAsync(string name)
        {
            return _dataAdapter.GetWorldObjectAsync(name);
        }

        public Task<WorldObjectPropertiesDto> GetWorldObjectPropertiesAsync(string name)
        {
            return _dataAdapter.GetWorldObjectPropertiesAsync(name);
        }

        public Task<bool> ContainsWorldObjectAsync(string name)
        {
            return _dataAdapter.ContainsWorldObjectAsync(name);
        }

        public Task UpdateWorldObjectAsync(string name, WorldObjectUpdateDto worldObjectUpdate)
        {
            return _dataAdapter.UpdateWorldObjectAsync(name, worldObjectUpdate);
        }

        public Task UpdateWorldObjectPropertiesAsync(string name, WorldObjectPropertiesDto properties)
        {
            return _dataAdapter.UpdateWorldObjectPropertiesAsync(name, properties);
        }
    }
}