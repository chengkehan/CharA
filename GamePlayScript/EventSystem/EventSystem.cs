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

        public enum ListenerPriority
        {
            High = 0,
            Normal = 100,
            Low = 200
        }

        public delegate void Listener(NotificationData _data);

        private Dictionary<EventID, List<ListenerItem>> registry = null;

        private EventSystem()
        {
            registry = new Dictionary<EventID, List<ListenerItem>>();
        }

        public void AddListener(EventID id, Listener listener)
        {
            AddListener(id, listener, ListenerPriority.Normal, null);
        }

        public void AddListener(EventID id, Listener listener, GameObject bindingGo)
        {
            AddListener(id, listener, ListenerPriority.Normal, bindingGo);
        }

        public void AddListener(EventID id, Listener listener, ListenerPriority priority)
        {
            AddListener(id, listener, priority, null);
        }

        public void AddListener(EventID id, Listener listener, ListenerPriority priority = ListenerPriority.Normal, GameObject bindingGo = null)
        {
            if (listener != null)
            {
                if (registry.TryGetValue(id, out List<ListenerItem> listeners) == false)
                {
                    listeners = new List<ListenerItem>();
                    registry.Add(id, listeners);
                }
                listeners.Add(new ListenerItem() { listener=listener, priority=priority, bindingGo=bindingGo, isBindingGoSet=bindingGo!=null });
            }
        }

        public void RemoveListener(EventID id, Listener listener)
        {
            if (listener != null)
            {
                if (registry.TryGetValue(id, out List<ListenerItem> listeners))
                {
                    foreach (var listenerItem in listeners)
                    {
                        if (listenerItem != null && listenerItem.listener == listener)
                        {
                            listeners.Remove(listenerItem);
                            break;
                        }
                    }
                }
            }
        }

        public void Notify(EventID id, NotificationData data = null)
        {
            if (registry.TryGetValue(id, out List<ListenerItem> listeners))
            {
                for (int listenerI = 0; listenerI < listeners.Count; listenerI++)
                {
                    var listener = listeners[listenerI];
                    if (listener.isBindingGoSet && listener.bindingGo == null)
                    {
                        listeners.RemoveAt(listenerI);
                        --listenerI;
                    }
                }

                var tempListeners = FetchTempListeners();
                {
                    tempListeners.listeners.AddRange(listeners);
                    tempListeners.listeners.Sort(SortTempListeners);

                    if (data != null)
                    {
                        data.interrupted = false;
                    }

                    foreach (var listener in tempListeners.listeners)
                    {
                        listener?.listener?.Invoke(data);
                        if (data != null && data.interrupted)
                        {
                            break;
                        }
                    }
                }
                RecycleTempListeners(tempListeners);
            }
        }

        #region Temp Listeners

        private class TempListeners
        {
            public List<ListenerItem> listeners = new List<ListenerItem>();

            public bool isWorking = false;
        }

        private List<TempListeners> _allTempListeners = new List<TempListeners>();

        private int SortTempListeners(ListenerItem a, ListenerItem b)
        {
            int pa = (int)a.priority;
            int pb = (int)b.priority;
            return
                pa > pb ? 1 :
                pa < pb ? -1 : 0;
        }

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

        private class ListenerItem
        {
            public Listener listener = null;

            public ListenerPriority priority = ListenerPriority.Normal;

            public bool isBindingGoSet = false;
            public GameObject bindingGo = null;
        }
    }
}