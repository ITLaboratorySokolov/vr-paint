using UnityEngine;
using UnityEngine.Assertions;
using ZCU.TechnologyLab.Common.Connections;
using ZCU.TechnologyLab.Common.Unity.Attributes;
using ZCU.TechnologyLab.Common.Unity.Connections.Data;

namespace ZCU.TechnologyLab.Common.Unity.Connections
{
    /// <summary>
    /// Wrapper over <see cref="SessionDataConnection"/>. This enables usage of <see cref="SessionDataConnection"/> in a scene.
    /// </summary>
    public class ServerDataConnectionWrapper : MonoBehaviour
    {
        /// <summary>
        /// Data client.
        /// </summary>
        [HelpBox("Data Client has to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [SerializeField]
        private DataClientWrapper dataClient;

        /// <summary>
        /// Gets data connection to a technology lab server.
        /// </summary>
        public ServerDataConnection Data { get; private set; }

        private void OnValidate()
        {
            Assert.IsNotNull(dataClient, "Data Client was null.");
        }

        private void Awake()
        {
            this.Data = new ServerDataConnection(dataClient);
        }
    }
}
