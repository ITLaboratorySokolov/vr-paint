﻿using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Connections.Session;

namespace ZCU.TechnologyLab.Common.Unity.Connections.Session
{
    /// <summary>
    /// Abstract wrapper of a session client from ZCU.TechnologyLab.Common.Connections.Session.
    /// The wrapper enables for the session client to be managed from a Unity scene.
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

        /// <summary>
        /// Event called when session is started.
        /// </summary>
        [SerializeField]
        private UnityEvent OnStarted = new UnityEvent();

        /// <summary>
        /// Event called when session cannot be started.
        /// </summary>
        [SerializeField]
        private UnityEvent OnStartFailed = new UnityEvent();

        /// <summary>
        /// Event called when session disconnects.
        /// </summary>
        [SerializeField]
        private UnityEvent OnDisconnected = new UnityEvent();

        /// <summary>
        /// Event called when session is reconnecting.
        /// </summary>
        [SerializeField]
        private UnityEvent OnReconnecting = new UnityEvent();

        /// <summary>
        /// Event called when session reconnected.
        /// </summary>
        [SerializeField]
        private UnityEvent OnReconnected = new UnityEvent();

        /// <summary>
        /// Session client.
        /// </summary>
        protected ISessionClient sessionClient;

        /// <inheritdocs/>
        public SessionState SessionState => this.sessionClient.SessionState;

        /// <summary>
        /// Awake method is called when game object is created.
        /// It initializes the session client.
        /// </summary>
        protected abstract void Awake();

        /// <summary>
        /// Adds event handlers on start.
        /// </summary>
        protected virtual void Start()
        {
            this.sessionClient.Disconnected += SessionClient_Disconnected;
            this.sessionClient.Reconnecting += SessionClient_Reconnecting;
            this.sessionClient.Reconnected += SessionClient_Reconnected;
        }

        /// <summary>
        /// Removes event handlers.
        /// </summary>
        protected virtual void OnDestroy()
        {
            this.sessionClient.Disconnected -= SessionClient_Disconnected;
            this.sessionClient.Reconnecting -= SessionClient_Reconnecting;
            this.sessionClient.Reconnected -= SessionClient_Reconnected;
        }

        protected abstract void CreateSession();

        /// <summary>
        /// Starts a session.
        /// This method should be used only for events. It cannot be awaited.
        /// </summary>
        public async void StartSession()
        {
            try
            {
                if(this.sessionClient == null)
                {
                    this.CreateSession();
                }

                await this.StartSessionAsync();
                Debug.Log("Session started");
                this.OnStarted.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot start session", this);
                Debug.LogException(ex, this);
                this.OnStartFailed.Invoke();
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
                Debug.Log("Session stopped");
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot stop session", this);
                Debug.LogException(ex, this);
            }
        }

        /// <inheritdoc/>
        public Task StartSessionAsync()
        {
            return this.sessionClient.StartSessionAsync();
        }

        /// <inheritdoc/>
        public Task StopSessionAsync()
        {
            return this.sessionClient.StopSessionAsync();
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

        /// <summary>
        /// Calls Unity event when the session disconnects.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Exception that caused the disconnection.</param>
        private void SessionClient_Disconnected(object sender, Exception e)
        {
            Debug.Log("Session disconnected");
            this.Disconnected?.Invoke(this, e);
            this.OnDisconnected.Invoke();
        }

        /// <summary>
        /// Calls Unity event when the session reconnects.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Exception that caused the reconnection.</param>
        private void SessionClient_Reconnecting(object sender, Exception e)
        {
            Debug.Log("Session disconnected");
            this.Reconnecting?.Invoke(this, e);
            this.OnReconnecting.Invoke();
        }

        /// <summary>
        /// Calls Unity event when the session reconnected.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        private void SessionClient_Reconnected(object sender, EventArgs e)
        {
            Debug.Log("Session disconnected");
            this.Reconnected?.Invoke(this, e);
            this.OnReconnected.Invoke();
        }
    }
}
