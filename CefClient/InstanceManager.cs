using CefClient.Chromium;
using CefShared.Memory;
using CefShared;
using System.Collections.Generic;
using UnityEngine;
using System;
using CefShared.Event;

namespace CefClient
{
    public class InstanceManager : MonoBehaviour
    {
        private static InstanceManager _instance;

        public static InstanceManager Instance { get { return _instance; } }

        private Dictionary<string, CefInstance> _cefInstances;

        private MemoryInstance _eventInMemory;
        private MemoryInstance _eventOutMemory;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);

                return;
            }

            _instance = this;

            _eventInMemory = new MemoryInstance("unitycefsharp_event_in");
            _eventInMemory.Connect();

            _eventOutMemory = new MemoryInstance("unitycefsharp_event_out");
            _eventOutMemory.Connect();

            _cefInstances = new Dictionary<string, CefInstance>();
        }

        void Update()
        {
            CefEvent[] events = _eventOutMemory.ReadEvents();

            foreach (CefEvent cefEvent in events)
            {
                if (cefEvent is CefInstanceCreatedEvent)
                {
                    CefInstanceCreatedEvent createdEvent = (CefInstanceCreatedEvent)cefEvent;

                    _cefInstances[createdEvent.InstanceID].Initialize();

                    continue;
                }
            }
        }

        public CefInstance CreateCefInstance(int width, int height, string url)
        {
            CefInstance instance = new CefInstance(Guid.NewGuid().ToString());
            _cefInstances[instance.InstanceID] = instance;

            instance.name = instance.InstanceID;
            instance.transform.parent = transform;

            instance.Configure(width, height, url);

            return instance;
        }

        public void SendEvent(CefEvent cefEvent)
        {
            _eventInMemory.WriteEvent(cefEvent);
        }
    }
}
