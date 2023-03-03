using System;
using System.Collections;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Utility;
using ZCU.TechnologyLab.Common.Unity.Tests.Library;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Behaviours.Connections.Client.Session
{
    [TestFixture]
    public class SignalRSessionWrapperTest
    {
        private GameObject _gameObject;
        private SessionClientWrapperMock _sessionWrapperMock;
        private SignalRSessionWrapperEventsHandler _eventsHandler;
        private GameObject _unityDispatcher;

        [SetUp]
        public void SetUp()
        {
            _gameObject = UnityEngine.Object.Instantiate(Resources.Load("SessionClientWrapperTest")) as GameObject;
            _sessionWrapperMock = _gameObject.GetComponent<SessionClientWrapperMock>();
            _eventsHandler = _gameObject.GetComponent<SignalRSessionWrapperEventsHandler>();

            _unityDispatcher = new GameObject();
            _unityDispatcher.AddComponent<UnityDispatcher>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
            UnityEngine.Object.DestroyImmediate(_unityDispatcher);
        }

        [Test]
        public async Task StartSessionAsync_StartedEvent_IsTriggered()
        {
            var eventTriggered = false;
            _sessionWrapperMock.Started += () => eventTriggered = true;
            
            await _sessionWrapperMock.StartSessionAsync();
            
            Assert.That(eventTriggered, Is.True);
        }
        
        [Test]
        public async Task StartSessionAsync_StartedUnityEvent_IsTriggered()
        {
            await _sessionWrapperMock.StartSessionAsync();
            
            Assert.That(_eventsHandler.StartedCalled, Is.True);
        }

        [Test]
        public void StartSession_StartFailedEvent_IsTriggered()
        {
            LogAssert.ignoreFailingMessages = true;
            
            var eventTriggered = false;
            _sessionWrapperMock.StartFailed += () => eventTriggered = true;
            MockStartSessionAsyncToThrow();

            _sessionWrapperMock.StartSession();
            
            Assert.That(eventTriggered, Is.True);
        }

        [Test]
        public void StartSession_StartFailedUnityEvent_IsTriggered()
        {
            LogAssert.ignoreFailingMessages = true;
            
            MockStartSessionAsyncToThrow();

            _sessionWrapperMock.StartSession();
            
            Assert.That(_eventsHandler.StartFailedCalled, Is.True);
        }
        
        private void MockStartSessionAsyncToThrow()
        {
            _sessionWrapperMock.MockedClient.StartSessionAsync().Returns(_ => throw new Exception());
        }
        
        [UnityTest]
        public IEnumerator Disconnected_Event_IsTriggered()
        {
            LogAssert.ignoreFailingMessages = true;

            var eventTriggered = false;
            _sessionWrapperMock.Disconnected += _ => eventTriggered = true;
            
            RaiseDisconnectedEvent();

            yield return null;
            
            Assert.That(eventTriggered, Is.True);
        }

        [UnityTest]
        public IEnumerator Disconnected_UnityEvent_IsTriggered()
        {
            LogAssert.ignoreFailingMessages = true;

            RaiseDisconnectedEvent();
            
            yield return null;

            Assert.That(_eventsHandler.DisconnectedCalled, Is.True);
        }
        
        private void RaiseDisconnectedEvent()
        {
            _sessionWrapperMock.MockedClient.Disconnected += Raise.Event<Action<Exception>>(new Exception());
        }
        
        [UnityTest]
        public IEnumerator Reconnecting_Event_IsTriggered()
        {
            LogAssert.ignoreFailingMessages = true;

            var eventTriggered = false;
            _sessionWrapperMock.Reconnecting += _ => eventTriggered = true;

            RaiseReconnectingEvent();
            
            yield return null;

            Assert.That(eventTriggered, Is.True);
        }

        [UnityTest]
        public IEnumerator Reconnecting_UnityEvent_IsTriggered()
        {
            LogAssert.ignoreFailingMessages = true;

            RaiseReconnectingEvent();
            
            yield return null;

            Assert.That(_eventsHandler.ReconnectingCalled, Is.True);
        }
        
        private void RaiseReconnectingEvent()
        {
            _sessionWrapperMock.MockedClient.Reconnecting += Raise.Event<Action<Exception>>(new Exception());
        }
        
        [UnityTest]
        public IEnumerator Reconnected_Event_IsTriggered()
        {
            LogAssert.ignoreFailingMessages = true;

            var eventTriggered = false;
            _sessionWrapperMock.Reconnected += () => eventTriggered = true;

            RaiseReconnectedEvent();
            
            yield return null;

            Assert.That(eventTriggered, Is.True);
        }

        [UnityTest]
        public IEnumerator Reconnected_UnityEvent_IsTriggered()
        {
            LogAssert.ignoreFailingMessages = true;

            RaiseReconnectedEvent();
            
            yield return null;

            Assert.That(_eventsHandler.ReconnectedCalled, Is.True);
        }
        
        private void RaiseReconnectedEvent()
        {
            _sessionWrapperMock.MockedClient.Reconnected += Raise.Event<Action>();
        }
    }
}