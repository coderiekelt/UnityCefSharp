using CefSharp;
using CefSharp.OffScreen;
using System;
using CefShared.Event;
using CefShared.Memory;
using CefShared;
using CefServer.Chromium;
using CefShared.Network;
using System.Threading;
using CefShared.Network.EventArgs;

namespace CefServer
{
    class Program
    {
        private static EventServer _eventServer;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Do not execute this application!");

                return;
            }

            EventRegistry.BuildDictionary();

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            _eventServer = new EventServer(Int32.Parse(args[0]));
            _eventServer.OnEventReceived += EventReceivedHandler;

            _eventServer.Start();

            CefSettings settings = new CefSettings();
            CefSharpSettings.ShutdownOnExit = true;
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            while (true)
            {
                Thread.Sleep(1);
            }
        }

        private static void EventReceivedHandler(object sender, EventReceivedEventArgs args)
        {
            CefEvent cefEvent = args.CefEvent;

            if (cefEvent is CefCreateInstanceEvent)
            {
                CefCreateInstanceEvent createEvent = (CefCreateInstanceEvent)cefEvent;

                InstanceManager.CreateInstance(createEvent.InstanceID);
                CefInstance cefInstance = InstanceManager.GetInstance(createEvent.InstanceID);

                cefInstance.Width = createEvent.Width;
                cefInstance.Height = createEvent.Height;
                cefInstance.Url = createEvent.Url;

                cefInstance.Start();

                return;
            }

            InstanceManager.GetInstance(cefEvent.InstanceID).ReceiveEvent(cefEvent);
        }

        public static void SendEvent(CefEvent cefEvent)
        {
            _eventServer.WriteEvent(cefEvent);
        }
    }
}
