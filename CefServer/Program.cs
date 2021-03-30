using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using SharedMemory;
using CefShared.Event;
using CefShared.Memory;
using CefShared;
using CefServer.Chromium;

namespace CefServer
{
    class Program
    {
        private static MemoryInstance _eventInMemory;
        private static MemoryInstance _eventOutMemory;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Do not execute this application!");

                return;
            }

            _eventInMemory = new MemoryInstance("unitycefsharp_event_in");
            _eventInMemory.Init(0);

            _eventOutMemory = new MemoryInstance("unitycefsharp_event_out");
            _eventOutMemory.Init(0);

            CefSettings settings = new CefSettings();
            CefSharpSettings.ShutdownOnExit = true;
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            string ourInstance = InstanceManager.CreateInstance();

            Console.WriteLine("Spawning new instance ({0})", ourInstance);

            InstanceManager.GetInstance(ourInstance).Width = 1920;
            InstanceManager.GetInstance(ourInstance).Height = 1080;
            InstanceManager.GetInstance(ourInstance).Start();

            while (true)
            {
                CefEvent[] cefEvents = _eventInMemory.ReadEvents();

                foreach (CefEvent cefEvent in cefEvents)
                {
                    if (cefEvent is CefCreateInstanceEvent)
                    {
                        CefCreateInstanceEvent createInstanceEvent = (CefCreateInstanceEvent)cefEvent;

                        InstanceManager.CreateInstance(createInstanceEvent.InstanceID);
                        CefInstance cefInstance = InstanceManager.GetInstance(createInstanceEvent.InstanceID);

                        cefInstance.Width = createInstanceEvent.Width;
                        cefInstance.Height = createInstanceEvent.Height;
                        cefInstance.Start();
                    }
                }
            }
        }

        public static void SendEvent(CefEvent cefEvent)
        {
            _eventOutMemory.WriteEvent(cefEvent);
        }
    }
}
