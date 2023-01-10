using UnityEngine;
using UnityEngine.Assertions;
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
        [HelpBox("Server Url and Hub have to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private StringVariable serverUrl;

        [SerializeField]
        private StringVariable hub;

        /// <summary>
        /// Id which is used to map users to established connections.
        /// </summary>
        public string ConnectionId => ((SignalRSession)this.sessionClient).ConnectionId;
        
        private void OnValidate()
        {
            Assert.IsNotNull(this.serverUrl, "Server Url was null.");
            Assert.IsNotNull(this.hub, "Hub was null.");
        }

        protected override void CreateClient()
        {
            this.sessionClient = new SignalRSession(this.serverUrl.Value, this.hub.Value);
        }
    }
}
