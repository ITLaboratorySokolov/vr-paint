using System;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Library
{
    public class SignalRSessionWrapperEventsHandler : MonoBehaviour
    {
        public bool StartedCalled { get; private set; }
        public bool StartFailedCalled { get; private set; }
        public bool DisconnectedCalled { get; private set; }
        public bool ReconnectingCalled { get; private set; }
        public bool ReconnectedCalled { get; private set; }

        public void OnStarted()
        {
            try
            {
                Debug.Log("Started is called");
                var tempObject = new GameObject();
                Destroy(tempObject);
                StartedCalled = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public void OnStartFailed()
        {
            try
            {
                Debug.Log("StartFailed is called");
                var tempObject = new GameObject();
                Destroy(tempObject);
                StartFailedCalled = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public void OnDisconnected()
        {
            try
            {
                Debug.Log("Disconnected is called");
                var tempObject = new GameObject();
                Destroy(tempObject);
                DisconnectedCalled = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public void OnReconnecting()
        {
            try
            {
                Debug.Log("Reconnecting is called");
                var tempObject = new GameObject();
                Destroy(tempObject);
                ReconnectingCalled = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    
        public void OnReconnected()
        {
            try
            {
                Debug.Log("Reconnected is called");
                var tempObject = new GameObject();
                Destroy(tempObject);
                ReconnectedCalled = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}