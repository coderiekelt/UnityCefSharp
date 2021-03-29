using System;
using System.Threading;
using CefSharp;
using CefSharp.OffScreen;
using CefShared;
using CefShared.Memory;

namespace CefServer.Chromium
{
    public class CefInstance
    {
        private Thread _thread;
        private ChromiumWebBrowser _browser;

        public string InstanceID;

        private MemoryInstance GfxMemory;

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
            GfxMemory = new MemoryInstance(InstanceID + "_gfx");
            GfxMemory.Init(Width * Height * 4);

            _browser = new ChromiumWebBrowser();
            _browser.RenderHandler = new CefRenderHandler(GfxMemory, Width, Height);

            _browser.BrowserInitialized += BrowserInitialized;
        }

        private void BrowserInitialized(object sender, EventArgs e)
        {
            _browser.Load("https://www.google.com/");
        }
    }
}
