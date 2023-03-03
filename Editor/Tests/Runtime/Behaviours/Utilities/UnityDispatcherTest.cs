using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Utility;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Behaviours.Utilities
{
    [TestFixture]
    public class UnityDispatcherTest
    {
        [Test]
        public void Awake_InstanceIsNull_SetsInstance()
        {
            var gameObject = new GameObject("UnityDispatcher");
            var unityDispatcher = gameObject.AddComponent<UnityDispatcher>();
            
            Assert.That(UnityDispatcher.Instance, Is.EqualTo(unityDispatcher));
            
            Object.DestroyImmediate(gameObject);
        }
        
        [Test]
        public void OnDestroy_Instance_IsSetToNull()
        {
            var gameObject = new GameObject("UnityDispatcher");
            gameObject.AddComponent<UnityDispatcher>();
            Object.DestroyImmediate(gameObject);

            Assert.That(UnityDispatcher.Instance, Is.Null);
        }
        
        [UnityTest]
        public IEnumerator Awake_SecondInstance_DestroysItself()
        {
            var firstGameObject = new GameObject("FirstUnityDispatcher");
            firstGameObject.AddComponent<UnityDispatcher>();
            var secondGameObject = new GameObject("SecondUnityDispatcher");
            secondGameObject.AddComponent<UnityDispatcher>();
            
            yield return new WaitForEndOfFrame();
            
            Assert.That(secondGameObject.GetComponent<UnityDispatcher>(), Is.Null);
            
            Object.DestroyImmediate(firstGameObject);
            Object.DestroyImmediate(secondGameObject);
        }
        
        [Test]
        public void Awake_SecondInstance_InstanceFieldDoesNotChange()
        {
            var firstGameObject = new GameObject("FirstUnityDispatcher");
            var firstUnityDispatcher = firstGameObject.AddComponent<UnityDispatcher>();
            var secondGameObject = new GameObject("SecondUnityDispatcher");
            secondGameObject.AddComponent<UnityDispatcher>();
            
            Assert.That(UnityDispatcher.Instance, Is.EqualTo(firstUnityDispatcher));
            
            Object.DestroyImmediate(firstGameObject);
            Object.DestroyImmediate(secondGameObject);
        }
        
        [UnityTest]
        public IEnumerator Awake_SecondInstanceIsDestroyed_InstanceFieldIsStillSet()
        {
            var firstGameObject = new GameObject("FirstUnityDispatcher");
            var firstUnityDispatcher = firstGameObject.AddComponent<UnityDispatcher>();
            var secondGameObject = new GameObject("SecondUnityDispatcher");
            secondGameObject.AddComponent<UnityDispatcher>();

            yield return new WaitForEndOfFrame();
            
            Assert.That(UnityDispatcher.Instance, Is.EqualTo(firstUnityDispatcher));
            
            Object.DestroyImmediate(firstGameObject);
            Object.DestroyImmediate(secondGameObject);
        }

        [UnityTest]
        public IEnumerator Update_ActionFromQueue_IsInvoked()
        {
            var gameObject = new GameObject("UnityDispatcher");
            gameObject.AddComponent<UnityDispatcher>();

            var executed = false;
            UnityDispatcher.ExecuteOnUnityThread(() => executed = true);

            yield return null;
            
            Assert.That(executed, Is.True);
            
            Object.DestroyImmediate(gameObject);
        }

        [UnityTest]
        public IEnumerator ExecuteOnUnityThread_LazyInvocation_InvokeActionWhenObjectIsCreated()
        {
            var executed = false;
            UnityDispatcher.ExecuteOnUnityThread(() => executed = true);

            yield return null;
            
            Assert.That(executed, Is.False);
            
            var gameObject = new GameObject("UnityDispatcher");
            gameObject.AddComponent<UnityDispatcher>();
            
            yield return null;
            
            Assert.That(executed, Is.True);
            
            Object.DestroyImmediate(gameObject);
        }
    }
}