using System;
using System.Threading;
using CefSharp;
using CefSharp.OffScreen;
using CefShared;
using CefShared.Memory;
using CefShared.Event;
using System.Runtime.InteropServices;
using System.Text;

namespace CefServer.Chromium
{
    public class CefInstance
    {
        private Thread _thread;
        private ChromiumWebBrowser _browser;
        private CefRenderHandler _renderHandler;

        public string InstanceID;

        private MemoryInstance _gfxMemory;

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
                KeyEvent keyEvent = new KeyEvent()
                {
                    Type = cefKeyboardEvent.IsChar ? KeyEventType.Char : (cefKeyboardEvent.IsDown ? KeyEventType.KeyDown : KeyEventType.KeyUp),
                    WindowsKeyCode = cefKeyboardEvent.IsChar ? GetCharsFromKeys(cefKeyboardEvent.Key, cefKeyboardEvent.Shift)[0] : cefKeyboardEvent.Key,
                    FocusOnEditableField = true,
                    Modifiers = cefKeyboardEvent.Shift ? CefEventFlags.ShiftDown : CefEventFlags.None,
                    IsSystemKey = false
                };

                Console.WriteLine("Sending key {0} | Type: {1} | {2} | {3}", keyEvent.WindowsKeyCode, keyEvent.Type, cefKeyboardEvent.IsChar, cefKeyboardEvent.Shift);

                _browser.GetBrowser().GetHost().SendKeyEvent(keyEvent);

                return;
            }
        }

        private void Run()
        {
            _gfxMemory = new MemoryInstance(InstanceID + "_gfx");
            _gfxMemory.Init(Width * Height * 4);

            _browser = new ChromiumWebBrowser();
            _renderHandler = new CefRenderHandler(_gfxMemory, Width, Height);

            _browser.RenderHandler = _renderHandler;

            _browser.BrowserInitialized += BrowserInitialized;
            _browser.AddressChanged += ResetViewport;
        }

        private void BrowserInitialized(object sender, EventArgs e)
        {
            _browser.Load(Url);
            Console.WriteLine("Initialized instance {0}", InstanceID);
            Program.SendEvent(new CefInstanceCreatedEvent() { InstanceID = InstanceID });
        }

        private void ResetViewport(object sender, EventArgs e)
        {
            _renderHandler.ResetViewport();
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int bufferSize, uint flags);

        /// <summary>
        /// https://stackoverflow.com/a/6949520/450141
        /// </summary>
        public static string GetCharsFromKeys(int keys, bool shift)
        {
            var buf = new StringBuilder(256);
            var keyboardState = new byte[256];
            if (shift)
                keyboardState[16] = 0xff;
            ToUnicode((uint)keys, 0, keyboardState, buf, 256, 0);
            return buf.ToString();
        }
    }
}
