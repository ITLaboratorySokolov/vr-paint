using UnityEngine;
using UnityEngine.Assertions;
using ZCU.TechnologyLab.Common.Connections.Session;
using ZCU.TechnologyLab.Common.Unity.AssetVariables;
using ZCU.TechnologyLab.Common.Unity.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Connections.Session
{
    /// <summary>
    /// A wrapper of a SignalR session.
    /// The wrapper enables for the SignalR client to be managed from a Unity scene.
    /// </summary>
    public class SignalRSessionWrapper : SessionClientWrapper
    {
        /// <summary>
        /// Url of a server.
        /// </summary>
        [HelpBox("Server Url and Hub have to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private StringVariable serverUrl;

        /// <summary>
        /// Target hub.
        /// </summary>
        [SerializeField]
        private StringVariable hub;

        private void OnValidate()
        {
            Assert.IsNotNull(serverUrl, "Server Url was null.");
            Assert.IsNotNull(hub, "Hub was null.");
        }

        /// <inheritdoc/>
        public override void CreateSession()
        {
            this.sessionClient = new SignalRSession(this.serverUrl.Value, this.hub.Value);
            base.CreateSession();
        }

        /// <summary>
        /// Returns id of this connection.
        /// </summary>
        /// <returns>The connection id.</returns>
        public string GetConnectionId()
        {
            return ((SignalRSession)this.sessionClient).GetConnectionId();
        }
    }
}
