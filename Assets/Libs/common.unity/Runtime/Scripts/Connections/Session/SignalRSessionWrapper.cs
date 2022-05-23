using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Connections.Session;
using ZCU.TechnologyLab.Common.Unity.AssetVariables;

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
        [SerializeField]
        private StringVariable serverUrl;

        /// <summary>
        /// Target hub.
        /// </summary>
        [SerializeField]
        private StringVariable hub;

        protected override void Awake()
        {
            this.sessionClient = new SignalRSession(this.serverUrl.Value, this.hub.Value);
        }

        protected override void CreateSession()
        {
            
        }
    }
}
