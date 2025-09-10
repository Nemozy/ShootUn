using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class ApplicationTickUpdater : MonoBehaviour
    {
        public event Action<float> OnTick;
        public event Action OnPause;
        public event Action OnResume;

        private static readonly Queue<Action> _mainThreadCallbackQueue = new ();
        private static readonly List<Action> _receivedMainThreadCallbackList = new ();
        private void Update()
        {
            OnTick?.Invoke(Time.deltaTime);
            RunMainThreadCallbacks();
        }

        private static void RunMainThreadCallbacks()
        {
            void DeadlockSafeCopyQueueToList()
            {
                lock (_mainThreadCallbackQueue)
                {
                    _receivedMainThreadCallbackList.AddRange(_mainThreadCallbackQueue);
                    _mainThreadCallbackQueue.Clear();
                }
            }

            DeadlockSafeCopyQueueToList();

            foreach (var notification in _receivedMainThreadCallbackList)
            {
                notification.Invoke();
            }

            _receivedMainThreadCallbackList.Clear();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                OnPause?.Invoke();
            }
            else
            {
                OnResume?.Invoke();
            }
        }
    }
}