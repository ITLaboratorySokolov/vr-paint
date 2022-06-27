using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Connections.Data;

namespace ZCU.TechnologyLab.Common.Unity.Connections.Data
{
    /// <summary>
    /// Abstract wrapper of a data client from ZCU.TechnologyLab.Common.Connections.Data.
    /// The wrapper enables for the data client to be managed from a Unity scene.
    /// </summary>
    public abstract class DataClientWrapper : MonoBehaviour, IDataClient
    {
        /// <summary>
        /// Data client.
        /// </summary>
        protected IDataClient dataClient;

        /// <summary>
        /// Initializes the data client.
        /// </summary>
        public abstract void CreateClient();

        /// <inheritdoc/>
        public void AddHeader(string name, string value)
        {
            this.dataClient.AddHeader(name, value);
        }
             
        /// <inheritdoc/>
        public Task DeleteAsync(string route, CancellationToken cancellationToken = default)
        {
            return this.dataClient.DeleteAsync(route, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<T> GetAsync<T>(string route, CancellationToken cancellationToken = default)
        {
            return this.dataClient.GetAsync<T>(route, cancellationToken);
        }

        /// <inheritdoc/>
        public Task PostAsync<T>(string route, T data, CancellationToken cancellationToken = default)
        {
            return this.dataClient.PostAsync<T>(route, data, cancellationToken);
        }

        /// <inheritdoc/>
        public Task PutAsync<T>(string route, T data, CancellationToken cancellationToken = default)
        {
            return this.dataClient.PutAsync<T>(route, data, cancellationToken);
        }
    }
}
