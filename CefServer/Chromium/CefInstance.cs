using System;
using System.Threading;
using CefSharp;
using CefSharp.OffScreen;
using CefShared;
using CefShared.Memory;
using CefShared.Event;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CefServer.Chromium
{
    public class CefInstance
    {
        private Thread _thread;
        private ChromiumWebBrowser _browser;
        private CefRenderHandler _renderHandler;
        private CefStaticBinding _staticBinding;

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
            _staticBinding = new CefStaticBinding(this);
            _thread.Start();

            return InstanceID;
        }

        public void ReceiveEvent(CefEvent cefEvent)
        {
            #region Handle mouse event

            if (cefEvent is CefMouseEvent)
            {
                CefMouseEvent cefMouseEvent = (CefMouseEvent)cefEvent;
                MouseEvent mouseEvent;
                CefEventFlags mouseEventFlags = CefEventFlags.None;

                if (cefMouseEvent.MouseButton != -1)
                {
                    MouseButtonType pressedButton;

                    switch (cefMouseEvent.MouseButton)
                    {
                        case 0:
                            pressedButton = MouseButtonType.Left;
                            mouseEventFlags = CefEventFlags.LeftMouseButton;
                            break;
                        case 1:
                            pressedButton = MouseButtonType.Right;
                            mouseEventFlags = CefEventFlags.RightMouseButton;
                            break;
                        case 2:
                            pressedButton = MouseButtonType.Middle;
                            mouseEventFlags = CefEventFlags.MiddleMouseButton;
                            break;
                        default:
                            pressedButton = MouseButtonType.Left;
                            mouseEventFlags = CefEventFlags.LeftMouseButton;
                            break;
                    }

                    mouseEvent = new MouseEvent(cefMouseEvent.MouseX, cefMouseEvent.MouseY, mouseEventFlags);

                    _browser.GetBrowser().GetHost().SendMouseClickEvent(mouseEvent, pressedButton, !cefMouseEvent.MouseButtonDown, 1);
                }

                mouseEvent = new MouseEvent(cefMouseEvent.MouseX, cefMouseEvent.MouseY, mouseEventFlags);

                if (cefMouseEvent.ScollDeltaX != 0 || cefMouseEvent.ScollDeltaY != 0)
                {
                    _browser.GetBrowser().GetHost().SendMouseWheelEvent(mouseEvent, cefMouseEvent.ScollDeltaX * 100, cefMouseEvent.ScollDeltaY * 100);
                }

                if (cefMouseEvent.MouseButton != -1)
                {
                    return;
                }

                _browser.GetBrowser().GetHost().SendMouseMoveEvent(mouseEvent, false);

                return;
            }

            #endregion

            #region Handle keyboard event

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

                _browser.GetBrowser().GetHost().SendKeyEvent(keyEvent);

                return;
            }

            #endregion

            #region Handle javascript event

            if (cefEvent is CefEvalJavascriptEvent)
            {
                CefEvalJavascriptEvent javascriptEvent = (CefEvalJavascriptEvent)cefEvent;

                if (javascriptEvent.CallbackID.Length > 0)
                {
                    Task<JavascriptResponse> task = _browser.EvaluateScriptAsync(javascriptEvent.Javascript);
                    task.ContinueWith(result => SendCallback(javascriptEvent, result.Result.Result.ToString()));

                    return;
                }

                _browser.EvaluateScriptAsync(javascriptEvent.Javascript);

                return;
            }

            #endregion

            #region Handle static call result event

            if (cefEvent is CefJavascriptResultEvent)
            {
                _staticBinding.HandleEvent((CefJavascriptResultEvent)cefEvent);

                return;
            }

            #endregion
        }

        private void SendCallback(CefEvalJavascriptEvent evalEvent, string result)
        {
            CefJavascriptResultEvent resultEvent = new CefJavascriptResultEvent()
            {
                InstanceID = InstanceID,
                CallbackID = evalEvent.CallbackID,
                Result = result,
            };

            Program.SendEvent(resultEvent);
        }

        private void Run()
        {
            _gfxMemory = new MemoryInstance(InstanceID + "_gfx");
            _gfxMemory.Init(Width * Height * 4);

            _browser = new ChromiumWebBrowser();
            _renderHandler = new CefRenderHandler(_gfxMemory, Width, Height);

            _browser.RenderHandler = _renderHandler;

            _browser.BrowserInitialized += BrowserInitialized;
            _browser.LoadingStateChanged += RegisterStaticBindings;
        }

        private void BrowserInitialized(object sender, EventArgs e)
        {
            _browser.JavascriptObjectRepository.Register("staticInvoker", _staticBinding, true);
            _browser.Load(Url);
            Console.WriteLine("Initialized instance {0}", InstanceID);
            Program.SendEvent(new CefInstanceCreatedEvent() { InstanceID = InstanceID });
        }

        private void RegisterStaticBindings(object sender, EventArgs e)
        {
            if (_browser.JavascriptObjectRepository.IsBound("staticInvoker")) { return;  }
            _browser.JavascriptObjectRepository.Register("staticInvoker", _staticBinding, true);
        }

        #region Keyboard helpers

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

        #endregion
    }
}
