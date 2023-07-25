using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
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
        [HelpBox("Server Url has to be assigned.", HelpBoxAttribute.MessageType.Warning)]
        [FormerlySerializedAs("serverUrl")]
        private StringVariable _serverUrl;

        private RestDataClient _restDataClient;

        private void Start()
        {
            _serverUrl.ValueChanged += OnServerUrlValueChanged;
        }

        private void OnServerUrlValueChanged(string newServerUrl)
        {
            _restDataClient.Url = newServerUrl;
        }

        private void OnValidate()
        {
            Assert.IsNotNull(_serverUrl, "Server Url was null.");
        }

        protected override IDataClient CreateClient()
        {
            return _restDataClient = new RestDataClient(_serverUrl.Value);
        }
    }
}