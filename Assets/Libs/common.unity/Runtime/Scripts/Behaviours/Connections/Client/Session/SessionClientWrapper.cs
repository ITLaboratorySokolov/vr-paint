using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Connections.Client.Session;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session
{
    /// <summary>
    /// Abstract wrapper of a session client from ZCU.TechnologyLab.Common.Connections.Session.
    /// The wrapper allows the session client to be managed from a Unity scene.
    /// C# event handlers and Unity event are both called when an event occurs.
    /// </summary>
    public abstract class SessionClientWrapper : MonoBehaviour, ISessionClient
    {
        /// <inheritdoc/>
        public event EventHandler<Exception> Disconnected;

        /// <inheritdoc/>
        public event EventHandler<Exception> Reconnecting;

        /// <inheritdoc/>
        public event EventHandler Reconnected;

        [SerializeField]
        [Tooltip("Event called when session is started.")]
        private UnityEvent onStarted = new();

        [SerializeField]
        [Tooltip("Event called when session cannot be started.")]
        private UnityEvent onStartFailed = new();

        [SerializeField]
        [Tooltip("Event called when session disconnects.")]
        private UnityEvent onDisconnected = new();

        [SerializeField]
        [Tooltip("Event called when session is reconnecting.")]
        private UnityEvent onReconnecting = new();

        [SerializeField]
        [Tooltip("Event called when session reconnected.")]
        private UnityEvent onReconnected = new();

        protected ISessionClient sessionClient;

        /// <inheritdocs/>
        public SessionState State => this.sessionClient.State;

        protected virtual void Awake()
        {
            this.CreateClient();
            this.sessionClient.Disconnected += this.SessionClient_Disconnected;
            this.sessionClient.Reconnecting += this.SessionClient_Reconnecting;
            this.sessionClient.Reconnected += this.SessionClient_Reconnected;
        }
        
        // Remove event handlers.
        protected virtual void OnDestroy()
        {
            this.sessionClient.Disconnected -= this.SessionClient_Disconnected;
            this.sessionClient.Reconnecting -= this.SessionClient_Reconnecting;
            this.sessionClient.Reconnected -= this.SessionClient_Reconnected;
        }
        
        protected abstract void CreateClient();

        /// <summary>
        /// Starts a session.
        /// This method should be used only for events. It cannot be awaited.
        /// </summary>
        public async void StartSession()
        {
            try
            {                    
                await this.StartSessionAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot start session", this);
                Debug.LogException(ex, this);
                this.onStartFailed.Invoke();
            }
        }

        /// <summary>
        /// Stops a session.
        /// This method should be used only for events. It cannot be awaited.
        /// </summary>
        public async void StopSession()
        {
            try
            {
                await this.StopSessionAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot stop session", this);
                Debug.LogException(ex, this);
            }
        }

        /// <inheritdoc/>
        public async Task StartSessionAsync()
        {
            await this.sessionClient.StartSessionAsync();
            Debug.Log("Session started");
            this.onStarted.Invoke();
        }

        /// <inheritdoc/>
        public async Task StopSessionAsync()
        {
            await this.sessionClient.StopSessionAsync();
            Debug.Log("Session stopped");
        }

        /// <inheritdoc/>
        public void RegisterCallback<T>(string method, Action<T> callback)
        {
            this.sessionClient.RegisterCallback(method, callback);
        }

        /// <inheritdoc/>
        public void UnregisterCallback(string method)
        {
            this.sessionClient.UnregisterCallback(method);
        }

        /// <inheritdoc/>
        public Task SendMessageAsync<T>(string method, T parameter)
        {
            return this.sessionClient.SendMessageAsync(method, parameter);
        }
        
        private void SessionClient_Disconnected(object sender, Exception e)
        {
            Debug.Log("Session disconnected");
            this.Disconnected?.Invoke(sender, e);
            this.onDisconnected.Invoke();
        }

        private void SessionClient_Reconnecting(object sender, Exception e)
        {
            Debug.Log("Session reconnecting");
            this.Reconnecting?.Invoke(sender, e);
            this.onReconnecting.Invoke();
        }

        private void SessionClient_Reconnected(object sender, EventArgs e)
        {
            Debug.Log("Session reconnected");
            this.Reconnected?.Invoke(sender, e);
            this.onReconnected.Invoke();
        }
    }
}
