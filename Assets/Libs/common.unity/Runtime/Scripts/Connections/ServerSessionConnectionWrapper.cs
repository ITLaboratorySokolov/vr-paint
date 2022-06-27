using UnityEngine;
using UnityEngine.Assertions;
using ZCU.TechnologyLab.Common.Connections;
using ZCU.TechnologyLab.Common.Unity.Attributes;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;

namespace ZCU.TechnologyLab.Common.Unity.Connections
{
    /// <summary>
    /// Wrapper over <see cref="ServerSessionConnection"/>. This enables usage of <see cref="ServerSessionConnection"/> in a scene.
    /// </summary>
    public class ServerSessionConnectionWrapper : MonoBehaviour
    {
        [HelpBox("Session Client has to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private SessionClientWrapper sessionClient;

        /// <summary>
        /// Gets session connection to a technology lab server.
        /// </summary>
        public ServerSessionConnection Session { get; private set; }

        private void OnValidate()
        {
            Assert.IsNotNull(sessionClient, "Session Client was null.");
        }

        private void Awake()
        {
            this.Session = new ServerSessionConnection(sessionClient);
        }
    }
}
