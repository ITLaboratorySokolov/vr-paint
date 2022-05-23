using UnityEngine;
using ZCU.TechnologyLab.Common.Connections.Data;
using ZCU.TechnologyLab.Common.Unity.AssetVariables;

namespace ZCU.TechnologyLab.Common.Unity.Connections.Data
{
    /// <summary>
    /// A wrapper of a REST data client.
    /// The wrapper enables for the REST client to be managed from a Unity scene.
    /// </summary>
    public class RestDataClientWrapper : DataClientWrapper
    {
        /// <summary>
        /// Url of a server.
        /// </summary>
        [SerializeField]
        private StringVariable serverUrl;

        /// <inheritdoc/>
        protected override void Awake()
        {
            this.dataClient = new RestDataClient(this.serverUrl.Value);
        }
    }
}
