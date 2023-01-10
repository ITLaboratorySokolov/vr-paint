using UnityEngine;
using UnityEngine.Assertions;
using ZCU.TechnologyLab.Common.Connections.Client.Data;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Data
{
    /// <summary>
    /// A wrapper of a REST data client.
    /// The wrapper allows the REST client to be managed from a Unity scene.
    /// </summary>
    public class RestDataClientWrapper : DataClientWrapper
    {
        [SerializeField]
        [HelpBox("Server Url has to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        private StringVariable serverUrl;

        private void OnValidate()
        {
            Assert.IsNotNull(this.serverUrl, "Server Url was null.");
        }

        protected override void CreateClient()
        {
            this.dataClient = new RestDataClient(this.serverUrl.Value);
        }
    }
}
