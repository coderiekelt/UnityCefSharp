using CefClient.Chromium;
using CefShared;
using System.Collections.Generic;
using UnityEngine;
using System;
using CefShared.Network;
using CefShared.Network.EventArgs;

namespace CefClient
{
    public class InstanceManager : MonoBehaviour
    {
        private static InstanceManager _instance;

        public static InstanceManager Instance { get { return _instance; } }

        private Dictionary<string, CefInstance> _cefInstances;

        private EventClient _eventClient;

        public int EventServerPort;

        public void Initialize()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);

                return;
            }

            _instance = this;

            EventRegistry.BuildDictionary();

            _eventClient = new EventClient(EventServerPort);
            _eventClient.OnEventReceived += EventReceivedHandler;

            _eventClient.Start();

            _cefInstances = new Dictionary<string, CefInstance>();
        }

        public bool IsReady()
        {
            return _eventClient.IsReady();
        }

        public CefInstance CreateCefInstance(int width, int height, string url)
        {
            CefInstance instance = gameObject.AddComponent<CefInstance>();
            instance.InstanceID = Guid.NewGuid().ToString();
            _cefInstances[instance.InstanceID] = instance;

            instance.name = instance.InstanceID;
            instance.transform.parent = transform;

            instance.Configure(width, height, url);

            return instance;
        }

        public void SendEvent(CefEvent cefEvent)
        {
            if (!_eventClient.IsReady())
            {
                Debug.LogWarning("Attempted to send an event whilst event client is not connected.");
            }

            _eventClient.WriteEvent(cefEvent);
        }

        private void EventReceivedHandler(object sender, EventReceivedEventArgs args)
        {
            CefEvent cefEvent = args.CefEvent;

            if (!_cefInstances.ContainsKey(cefEvent.InstanceID))
            {
                return;
            }

            _cefInstances[cefEvent.InstanceID].ReceiveEvent(cefEvent);
        }
    }
}
