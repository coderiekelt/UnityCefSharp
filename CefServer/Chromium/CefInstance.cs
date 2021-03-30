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
        private CefRenderHandler _renderHandler;

        public string InstanceID;

        private MemoryInstance _gfxMemory;
        private MemoryInstance _eventInMemory;
        private MemoryInstance _eventOutMemory;

        public int Width;
        public int Height;
        public string Url;

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
            Console.WriteLine("Starting instance {0}", InstanceID);

            _thread.Start();

            return InstanceID;
        }

        public void ReceiveEvent(CefEvent cefEvent)
        {
            if (cefEvent is CefMouseEvent)
            {
                CefMouseEvent cefMouseEvent = (CefMouseEvent)cefEvent;

                if (cefMouseEvent.MouseButton != -1)
                {
                    MouseButtonType pressedButton;

                    switch (cefMouseEvent.MouseButton)
                    {
                        case 0:
                            pressedButton = MouseButtonType.Left;
                            break;
                        case 1:
                            pressedButton = MouseButtonType.Right;
                            break;
                        case 2:
                            pressedButton = MouseButtonType.Middle;
                            break;
                        default:
                            pressedButton = MouseButtonType.Left;
                            break;
                    }

                    _browser.GetBrowser().GetHost().SendMouseClickEvent(new MouseEvent(cefMouseEvent.MouseX, cefMouseEvent.MouseY, CefEventFlags.None), pressedButton, !cefMouseEvent.MouseButtonDown, 1);

                    return;
                }

                _browser.GetBrowser().GetHost().SendMouseMoveEvent(new MouseEvent(cefMouseEvent.MouseX, cefMouseEvent.MouseY, CefEventFlags.None), false);

                return;
            }

            if (cefEvent is CefKeyboardEvent)
            {
                CefKeyboardEvent cefKeyboardEvent = (CefKeyboardEvent)cefEvent;

                // handle
                _browser.GetBrowser().GetHost().

                return;
            }
        }

        private void Run()
        {
            _gfxMemory = new MemoryInstance(InstanceID + "_gfx");
            _gfxMemory.Init(Width * Height * 4);

            _eventInMemory = new MemoryInstance(InstanceID + "_event_in");
            _eventInMemory.Init(1);

            _eventOutMemory = new MemoryInstance(InstanceID + "_event_out");
            _eventOutMemory.Init(1);

            _browser = new ChromiumWebBrowser();
            _renderHandler = new CefRenderHandler(_gfxMemory, Width, Height);

            _browser.RenderHandler = _renderHandler;

            _browser.BrowserInitialized += BrowserInitialized;
        }

        private void BrowserInitialized(object sender, EventArgs e)
        {
            _browser.Load(Url);
            Console.WriteLine("Initialized instance {0}", InstanceID);
            Program.SendEvent(new CefInstanceCreatedEvent() { InstanceID = InstanceID });
        }
    }
}
