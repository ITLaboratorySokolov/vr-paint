using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Unity.Attributes;
using ZCU.TechnologyLab.Common.Unity.Connections.Data;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;

namespace ZCU.TechnologyLab.Common.Unity.Connections
{
    /// <summary>
    /// Controls both networking clients at the same time. It allows common events and reaction to changes in both clients at once.
    /// </summary>
    public class TechnologyLabServerConnection : MonoBehaviour
    {
        private const string ConnectionIdHeaderName = "ConnectionId";

        [HelpBox("Rest Data Client and SignalR Session have to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private RestDataClientWrapper restDataClient;

        [SerializeField]
        private SignalRSessionWrapper signalRSession;

        [Space]
        [SerializeField]
        private UnityEvent Initialized = new UnityEvent();

        private void OnValidate()
        {
            Assert.IsNotNull(restDataClient, "Rest Data Client was null.");
            Assert.IsNotNull(signalRSession, "SignalR Session was null.");
        }

        /// <summary>
        /// Creates data and session clients.
        /// </summary>
        public void InitializeConnections()
        {
            this.restDataClient.CreateClient();
            this.signalRSession.CreateSession();
            this.Initialized.Invoke();
        }

        /// <summary>
        /// Assigns connection id to the header of a data client .
        /// </summary>
        public void AsignConnectionId()
        {
            this.restDataClient.AddHeader(ConnectionIdHeaderName, this.signalRSession.GetConnectionId());
        }
    }
}
