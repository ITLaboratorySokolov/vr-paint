using UnityEngine;
using UnityEngine.Assertions;
using ZCU.TechnologyLab.Common.Connections.Data;
using ZCU.TechnologyLab.Common.Unity.AssetVariables;
using ZCU.TechnologyLab.Common.Unity.Attributes;

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
        [HelpBox("Server Url has to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        private StringVariable serverUrl;

        private void OnValidate()
        {
            Assert.IsNotNull(serverUrl, "Server Url was null.");
        }

        /// <inheritdoc/>
        public override void CreateClient()
        {
            this.dataClient = new RestDataClient(this.serverUrl.Value);
        }
    }
}
