using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Utility;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session
{
    /// <summary>
    /// Abstract wrapper of a session client from ZCU.TechnologyLab.Common.Connections.Session.
    /// The wrapper allows the session client to be managed from a Unity scene.
    /// C# event handlers and Unity event are both called when an event occurs.
    /// </summary>
    public abstract class SessionClientWrapper : MonoBehaviour, ISessionClient
    {
        public event Action Started;
        public event Action<Exception> StartFailed;
        public event Action<Exception> StopFailed;
        public event Action<Exception> Disconnected;
        public event Action<Exception> Reconnecting;
        public event Action Reconnected;

        [SerializeField]
        [Tooltip("Event called when session is started.")]
        [FormerlySerializedAs("started")]
        private UnityEvent _started = new();

        [SerializeField]
        [Tooltip("Event called when session cannot be started.")]
        [FormerlySerializedAs("startFailed")]
        private UnityEvent<Exception> _startFailed = new();
        
        [SerializeField]
        [Tooltip("Event called when session cannot be stopped.")]
        private UnityEvent<Exception> _stopFailed = new();

        [SerializeField]
        [Tooltip("Event called when session disconnects.")]
        [FormerlySerializedAs("disconnected")]
        private UnityEvent<Exception> _disconnected = new();

        [SerializeField]
        [Tooltip("Event called when session is reconnecting.")]
        [FormerlySerializedAs("reconnecting")]
        private UnityEvent<Exception> _reconnecting = new();

        [SerializeField]
        [Tooltip("Event called when session reconnected.")]
        [FormerlySerializedAs("reconnected")]
        private UnityEvent _reconnected = new();

        private ISessionClient _sessionClient;

        /// <inheritdocs/>
        public SessionState State => _sessionClient.State;

        protected virtual void Awake()
        {
            InitializeSession();
        }
        
        // Remove event handlers.
        protected virtual void OnDestroy()
        {
            RemoveEventHandlers();
        }

        protected void InitializeSession()
        {
            if (_sessionClient != null)
            {
                RemoveEventHandlers();
            }

            _sessionClient = CreateClient();
            AssignEventHandlers();
        }
        
        protected abstract ISessionClient CreateClient();

        private void AssignEventHandlers()
        {
            _sessionClient.Disconnected += SessionClient_Disconnected;
            _sessionClient.Reconnecting += SessionClient_Reconnecting;
            _sessionClient.Reconnected += SessionClient_Reconnected;
        }

        private void RemoveEventHandlers()
        {
            _sessionClient.Disconnected -= SessionClient_Disconnected;
            _sessionClient.Reconnecting -= SessionClient_Reconnecting;
            _sessionClient.Reconnected -= SessionClient_Reconnected;
        }

        /// <inheritdoc/>
        public Task InitializeAsync()
        {
            try
            {
                return _sessionClient.InitializeAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot initialize session", this);
                Debug.LogException(ex, this);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task StartSessionAsync()
        {
            try
            {
                await _sessionClient.StartSessionAsync();
                Debug.Log("Session started");
                Started?.Invoke();
                _started.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot start session", this);
                Debug.LogException(ex, this);
                StartFailed?.Invoke(ex);
                _startFailed.Invoke(ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task StopSessionAsync()
        {
            try
            {
                await _sessionClient.StopSessionAsync();
                Debug.Log("Session stopped");
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot stop session", this);
                Debug.LogException(ex, this);
                StopFailed?.Invoke(ex);
                _stopFailed?.Invoke(ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public void RegisterCallback<T>(string method, Action<T> callback)
        {
            _sessionClient.RegisterCallback(method, callback);
        }

        /// <inheritdoc/>
        public void RemoveCallbacks(string method)
        {
            _sessionClient.RemoveCallbacks(method);
        }

        /// <inheritdoc/>
        public Task SendMessageAsync<T>(string method, T parameter)
        {
            return _sessionClient.SendMessageAsync(method, parameter);
        }

        #region Event Handlers

        private void SessionClient_Disconnected(Exception cause)
        {
            UnityDispatcher.ExecuteOnUnityThread(() =>
            {
                Debug.Log("Session disconnected");
                Disconnected?.Invoke(cause);
                _disconnected.Invoke(cause);
            });
        }

        private void SessionClient_Reconnecting(Exception cause)
        {
            UnityDispatcher.ExecuteOnUnityThread(() =>
            {
                Debug.Log("Session reconnecting");
                Reconnecting?.Invoke(cause);
                _reconnecting.Invoke(cause);
            });
        }

        private void SessionClient_Reconnected()
        {
            UnityDispatcher.ExecuteOnUnityThread(() =>
            {
                Debug.Log("Session reconnected");
                Reconnected?.Invoke();
                _reconnected.Invoke();
            });
        }

        #endregion
    }
}