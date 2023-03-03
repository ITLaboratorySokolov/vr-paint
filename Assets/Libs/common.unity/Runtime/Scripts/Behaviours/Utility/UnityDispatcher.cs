using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Utility
{
    public class UnityDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> ExecutionQueue = new();
        private static object s_lockObject = new();

        public static UnityDispatcher Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            /*
             * Locking whole while cycle helps with fluency of a program.
             * If I would lock only dequeue operation it would be possible
             * to add new actions infinitely and main thread would be
             * stuck on a single frame.
             */
            lock (s_lockObject)
            {
                while (ExecutionQueue.Count > 0)
                {
                    ExecutionQueue.Dequeue().Invoke();
                }
            }
        }

        public static void ExecuteOnUnityThread(Action action)
        {
            if (Instance == null)
            {
                Debug.LogWarning("UnityDispatcher is not in a scene");
            }

            lock (s_lockObject)
            {
                ExecutionQueue.Enqueue(action);
            }
        }
    }
}