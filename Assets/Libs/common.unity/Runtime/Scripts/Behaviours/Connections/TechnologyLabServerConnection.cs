using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Data;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Connections
{
    /// <summary>
    /// Controls both networking clients at the same time. It allows common events and reaction to changes in both clients at once.
    /// </summary>
    public class TechnologyLabServerConnection : MonoBehaviour
    {
        private const string ConnectionIdHeaderName = "ConnectionId";

        [HelpBox("Rest Data Client and SignalR Session have to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        [FormerlySerializedAs("restDataClient")]
        private RestDataClientWrapper _restDataClient;

        [SerializeField]
        [FormerlySerializedAs("signalRSession")]
        private SignalRSessionWrapper _signalRSession;

        [SerializeField]
        [Tooltip("When making changes to world objects, should the changes be reported back from a server to a client that caused them?")]
        [FormerlySerializedAs("reportChangesBackToSourceClient")]
        private bool _reportChangesBackToSourceClient;
        
        private void OnValidate()
        {
            Assert.IsNotNull(_restDataClient, "Rest Data Client was null.");
            Assert.IsNotNull(_signalRSession, "SignalR Session was null.");
        }

        private void Start()
        {
            if (!_reportChangesBackToSourceClient)
            {
                _restDataClient.AddHeader(ConnectionIdHeaderName, _signalRSession.ConnectionId);
            }
        }
    }
}