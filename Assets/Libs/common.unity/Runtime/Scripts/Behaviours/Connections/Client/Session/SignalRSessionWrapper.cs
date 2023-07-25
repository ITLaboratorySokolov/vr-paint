using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session
{
    /// <summary>
    /// A wrapper of a SignalR session.
    /// The wrapper allows the SignalR client to be managed from a Unity scene.
    /// </summary>
    public class SignalRSessionWrapper : SessionClientWrapper
    {
        [HelpBox("Server Url and Hub have to be assigned.", HelpBoxAttribute.MessageType.Warning)]
        [SerializeField]
        [FormerlySerializedAs("serverUrl")]
        private StringVariable _serverUrl;

        [SerializeField]
        [FormerlySerializedAs("hub")]
        private StringVariable _hub;

        private SignalRSession _signalRSession;
        
        /// <summary>
        /// Id which is used to map users to established connections.
        /// </summary>
        public string ConnectionId => _signalRSession.ConnectionId;

        private void Start()
        {
            _serverUrl.ValueChanged += OnServerUrlValueChanged;
        }

        private void OnServerUrlValueChanged(string newServerUrl)
        {
            InitializeSession();
        }

        private void OnValidate()
        {
            Assert.IsNotNull(_serverUrl, "Server Url was null.");
            Assert.IsNotNull(_hub, "Hub was null.");
        }

        protected override ISessionClient CreateClient()
        {
            return _signalRSession = new SignalRSession(_serverUrl.Value, _hub.Value);
        }
    }
}
