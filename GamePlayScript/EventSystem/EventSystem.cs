using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class EventSystem
    {
        private static EventSystem s_instance = null;

        public static EventSystem GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new EventSystem();
            }
            return s_instance;
        }

        public delegate void Listener(NotificationData _data);

        private Dictionary<EventID, List<Listener>> registry = null;

        private EventSystem()
        {
            registry = new Dictionary<EventID, List<Listener>>();
        }

        public void AddListener(EventID id, Listener listener)
        {
            if (listener != null)
            {
                if (registry.TryGetValue(id, out List<Listener> listeners) == false)
                {
                    listeners = new List<Listener>();
                    registry.Add(id, listeners);
                }
                listeners.Add(listener);
            }
        }

        public void RemoveListener(EventID id, Listener listener)
        {
            if (listener != null)
            {
                if (registry.TryGetValue(id, out List<Listener> listeners))
                {
                    listeners.Remove(listener);
                }
            }
        }

        public void Notify(EventID id, NotificationData data = null)
        {
            if (registry.TryGetValue(id, out List<Listener> listeners))
            {
                var tempListeners = FetchTempListeners();
                tempListeners.listeners.AddRange(listeners);
                foreach (var listener in tempListeners.listeners)
                {
                    listener?.Invoke(data);
                }
                RecycleTempListeners(tempListeners);
            }
        }

        #region Temp Listeners

        private class TempListeners
        {
            public List<Listener> listeners = new List<Listener>();

            public bool isWorking = false;
        }

        private List<TempListeners> _allTempListeners = new List<TempListeners>();

        private TempListeners FetchTempListeners()
        {
            TempListeners tempListeners = null;
            for (int i = 0; i < _allTempListeners.Count; i++)
            {
                if (_allTempListeners[i].isWorking == false)
                {
                    tempListeners = _allTempListeners[i];
                    break;
                }
            }

            if (tempListeners == null)
            {
                tempListeners = new TempListeners();
                _allTempListeners.Add(tempListeners);
            }

            tempListeners.isWorking = true;
            tempListeners.listeners.Clear();

            return tempListeners;
        }

        private void RecycleTempListeners(TempListeners tempListeners)
        {
            if (tempListeners != null)
            {
                tempListeners.isWorking = false;
                tempListeners.listeners.Clear();
            }
        }

        #endregion

    }
}