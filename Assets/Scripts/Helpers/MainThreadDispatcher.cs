using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Class that allows for the execution of actions on the main thread from other threads.
    /// </summary>
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static MainThreadDispatcher _instance;

        private readonly Queue<Action> _actions = new();

        public static MainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("MainThreadDispatcher");
                    _instance = go.AddComponent<MainThreadDispatcher>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            lock (_actions)
            {
                while (_actions.Count > 0)
                {
                    Action action = _actions.Dequeue();
                    action?.Invoke();
                }
            }
        }

        public void Enqueue(Action action)
        {
            lock (_actions)
            {
                _actions.Enqueue(action);
            }
        }
    }
}