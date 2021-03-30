using CefSharp;
using CefSharp.OffScreen;
using System;
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

            _eventInMemory = new MemoryInstance(args[0] + "_event_in");
            _eventInMemory.Init(1);

            _eventOutMemory = new MemoryInstance(args[0] + "_event_out");
            _eventOutMemory.Init(1);

            CefSettings settings = new CefSettings();
            CefSharpSettings.ShutdownOnExit = true;
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            while (true)
            {
                CefEvent[] cefEvents = _eventInMemory.ReadEvents();

                foreach (CefEvent cefEvent in cefEvents)
                {
                    Console.WriteLine("Received event " + cefEvent);

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
