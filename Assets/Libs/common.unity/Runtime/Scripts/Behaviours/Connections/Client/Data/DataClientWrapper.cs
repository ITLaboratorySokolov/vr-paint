using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Connections.Client.Data;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Data
{
    /// <summary>
    /// Abstract wrapper of a data client from ZCU.TechnologyLab.Common.Connections.Data.
    /// The wrapper allows the data client to be managed from a Unity scene.
    /// </summary>
    public abstract class DataClientWrapper : MonoBehaviour, IDataClient
    {
        private IDataClient _dataClient;

        private void Awake()
        {
            _dataClient = CreateClient();
        }

        protected abstract IDataClient CreateClient();

        /// <inheritdoc/>
        public void SetHeader(string name, string value)
        {
            _dataClient.SetHeader(name, value);
        }

        /// <inheritdoc/>
        public Task DeleteAsync(string route, CancellationToken cancellationToken = default)
        {
            return _dataClient.DeleteAsync(route, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> GetAsync(string route, CancellationToken cancellationToken = new CancellationToken())
        {
            return _dataClient.GetAsync(route, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<T> GetAsync<T>(string route, CancellationToken cancellationToken = default)
        {
            return _dataClient.GetAsync<T>(route, cancellationToken);
        }

        /// <inheritdoc/>
        public Task PostAsync<T>(string route, T data, CancellationToken cancellationToken = default)
        {
            return _dataClient.PostAsync(route, data, cancellationToken);
        }

        /// <inheritdoc/>
        public Task PutAsync<T>(string route, T data, CancellationToken cancellationToken = default)
        {
            return _dataClient.PutAsync(route, data, cancellationToken);
        }
    }
}