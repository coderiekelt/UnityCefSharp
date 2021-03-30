using CefClient.Chromium;
using CefClient;
using CefShared.Event;
using UnityEngine;
using System.Collections.Generic;

public class ChromiumGUI : MonoBehaviour
{
    public CefInstance CefInstance;

    public string URL;

    public bool CaptureKeyboard = false;
    public bool CaptureMouse = false;

    private bool _initialized = false;

    private Vector3 _lastMousePoint;
    private int _lastMouseButton = -1;

    private List<bool> _mouseButtonStates = new List<bool>();

    void Awake()
    {
        _mouseButtonStates.Add(false); // LMB
        _mouseButtonStates.Add(false); // RMB
        _mouseButtonStates.Add(false); // MMB
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
                        MouseY = Screen.height - (int) Input.mousePosition.y
                    };

                    _mouseButtonStates[i] = newState;
                    InstanceManager.Instance.SendEvent(cefMouseEvent);

                    updateMousePos = false;
                }
            }

            if (updateMousePos && _lastMousePoint != Input.mousePosition) {
                cefMouseEvent = new CefMouseEvent()
                {
                    InstanceID = CefInstance.InstanceID,
                    MouseButton = -1,
                    MouseButtonDown = false,
                    MouseX = (int) Input.mousePosition.x,
                    MouseY = Screen.height - (int) Input.mousePosition.y
                };

                InstanceManager.Instance.SendEvent(cefMouseEvent);
                _lastMousePoint = Input.mousePosition;
            }
        }

        if (CaptureKeyboard && Input.anyKeyDown) {
            string key = Input.inputString;

            // Send event
        }
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
    }
}
