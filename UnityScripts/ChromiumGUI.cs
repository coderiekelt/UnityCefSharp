using CefClient.Chromium;
using CefClient;
using CefShared.Event;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ChromiumGUI : MonoBehaviour
{
    [DllImport("user32.dll")]
    public static extern short GetKeyState(int keyCode);

    public CefInstance CefInstance;

    public string URL;

    public bool CaptureKeyboard = false;
    public bool CaptureMouse = false;

    private bool _initialized = false;
    private Vector3 _lastMousePoint;
    private int _lastMouseButton = -1;
    private List<bool> _mouseButtonStates = new List<bool>();
    private bool _capslock;

    void Awake()
    {
        _mouseButtonStates.Add(false); // LMB
        _mouseButtonStates.Add(false); // RMB
        _mouseButtonStates.Add(false); // MMB

        _capslock = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
    }

    void Update()
    {
        if (InstanceManager.Instance == null) { return; }

        if (!_initialized) {
            CefInstance = InstanceManager.Instance.CreateCefInstance(Screen.width, Screen.height, URL);
            _initialized = true;
        }

        if (!CefInstance.IsInitialized) { return; }

        if (CaptureMouse) {
            CefMouseEvent cefMouseEvent;
            bool updateMousePos = true;

            for (int i = 0; i < 2; i++) {
                bool newState = Input.GetMouseButton(i);

                if (newState != _mouseButtonStates[i]) {
                    cefMouseEvent = new CefMouseEvent()
                    {
                        InstanceID = CefInstance.InstanceID,
                        MouseButton = i,
                        MouseButtonDown = newState,
                        MouseX = (int) Input.mousePosition.x,
                        MouseY = Screen.height - (int) Input.mousePosition.y,
                        ScollDeltaX = (int)Input.mouseScrollDelta.x,
                        ScollDeltaY = (int)Input.mouseScrollDelta.y,
                    };

                    _mouseButtonStates[i] = newState;
                    InstanceManager.Instance.SendEvent(cefMouseEvent);

                    updateMousePos = false;
                }
            }

            if (updateMousePos && (_lastMousePoint != Input.mousePosition || (Input.mouseScrollDelta.x != 0 || Input.mouseScrollDelta.y != 0))) {
                cefMouseEvent = new CefMouseEvent()
                {
                    InstanceID = CefInstance.InstanceID,
                    MouseButton = -1,
                    MouseButtonDown = false,
                    MouseX = (int) Input.mousePosition.x,
                    MouseY = Screen.height - (int) Input.mousePosition.y,
                    ScollDeltaX = (int)Input.mouseScrollDelta.x,
                    ScollDeltaY = (int)Input.mouseScrollDelta.y,
                };

                InstanceManager.Instance.SendEvent(cefMouseEvent);
                _lastMousePoint = Input.mousePosition;
            }
        }

        if (CaptureKeyboard) {
            if(Input.GetKeyDown(KeyCode.CapsLock))
            {
                _capslock  = !_capslock ;
            }
        }
    }

    void Start()
    {
    }

    void OnGUI()
    {
        if (CefInstance == null) {
            return;
        }

        if (CefInstance.ViewTexture == null) {
            return;
        }

        GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(Screen.width * .5f, Screen.height * .5f));

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), CefInstance.ViewTexture);

        GUIUtility.ScaleAroundPivot(new Vector2(1f, 1f), new Vector2(Screen.width * .5f, Screen.height * .5f));

        if (CaptureKeyboard) {
            var ev = Event.current;
            if (ev.type == EventType.KeyDown || ev.type == EventType.KeyUp)
            {
                int vcode = 0;
                if (CefKeys.KeyCodeMap.TryGetValue(ev.keyCode, out vcode))
                {
                    bool isChar = CefKeys.CharKeys.Contains(ev.keyCode);

                    if (isChar && ev.type == EventType.KeyUp) {
                        return;
                    }

                    // TODO: Refactor, we are now just checking if it's a A-Z vcode and simulating shift on capslock
                    CefKeyboardEvent cefKeyboardEvent = new CefKeyboardEvent()
                    {
                        InstanceID = CefInstance.InstanceID,
                        IsChar = isChar,
                        IsDown = ev.type == EventType.KeyDown,
                        Shift = Input.GetKey(KeyCode.LeftShift) || ((vcode >= 0x41 && vcode <= 0x5A) ? _capslock : false),
                        Key = vcode
                    };

                    Debug.Log("key event " + ev.keyCode + " " + ev.type + " is char? " + isChar + " shift? " + cefKeyboardEvent.Shift);

                    InstanceManager.Instance.SendEvent(cefKeyboardEvent);
                }
            }
        }
    }
}
