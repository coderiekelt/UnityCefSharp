using System;
using System.Threading;
using CefSharp;
using CefSharp.OffScreen;
using CefShared;
using CefShared.Memory;
using CefShared.Event;

namespace CefServer.Chromium
{
    public class CefInstance
    {
        private Thread _thread;
        private ChromiumWebBrowser _browser;

        public string InstanceID;

        private MemoryInstance _gfxMemory;
        private MemoryInstance _eventInMemory;
        private MemoryInstance _eventOutMemory;

        public int Width;
        public int Height;

        public CefInstance()
        {
            _thread = new Thread(Run);
            InstanceID = Guid.NewGuid().ToString();
        }

        public CefInstance(string guid)
        {
            _thread = new Thread(Run);
            InstanceID = guid;
        }

        public string Start()
        {
            _thread.Start();

            return InstanceID;
        }

        public void HandleEvent(CefEvent cefEvent)
        {
            Console.WriteLine("Received event {0} on instance {1}", cefEvent.ToString(), InstanceID);

            
        }

        private void Run()
        {
            _gfxMemory = new MemoryInstance(InstanceID + "_gfx");
            _gfxMemory.Init(Width * Height * 4);
            _eventInMemory = new MemoryInstance(InstanceID + "_event_in");
            _eventOutMemory = new MemoryInstance(InstanceID + "_event_out");

            _browser = new ChromiumWebBrowser();
            _browser.RenderHandler = new CefRenderHandler(_gfxMemory, Width, Height);

            _browser.BrowserInitialized += BrowserInitialized;
        }

        private void BrowserInitialized(object sender, EventArgs e)
        {
            _browser.Load("https://www.google.com/");
            _eventOutMemory.WriteEvent(new CefInstanceCreatedEvent() { InstanceID = InstanceID });
        }
    }
}
