using CefShared;
using CefShared.Event;
using CefShared.Memory;
using UnityEngine;

namespace CefClient.Chromium
{
    public class CefInstance : MonoBehaviour
    {
        private MemoryInstance _gfxMemory;
        private MemoryInstance _eventInMemory;
        private MemoryInstance _eventOutMemory;

        public string InstanceID;

        private int _width;
        private int _height;
        private string _url;

        private byte[] _viewTextureBuffer;

        public Texture2D ViewTexture;
        public bool IsInitialized { get; private set; }

        void Update()
        {
            if (!IsInitialized)
            {
                return;
            }

            CefEvent[] events = _eventOutMemory.ReadEvents();

            // TODO: Handle events

            if (_viewTextureBuffer == null)
            {
                _viewTextureBuffer = new byte[_width * _height * 4];
            }

            if (ViewTexture == null)
            {
                ViewTexture = new Texture2D(_width, _height, TextureFormat.BGRA32, false, true);
            }

            _viewTextureBuffer = _gfxMemory.ReadBytes();
            ViewTexture.LoadRawTextureData(_viewTextureBuffer);
            ViewTexture.Apply();
        }

        public void Configure(int width, int height, string url)
        {
            Debug.Log(InstanceID + " configuring");

            _width = width;
            _height = height;
            _url = url;

            CefCreateInstanceEvent createEvent = new CefCreateInstanceEvent()
            {
                Width = width,
                Height = height,
                Url = url,
                InstanceID = InstanceID
            };

            InstanceManager.Instance.SendEvent(createEvent);
        }

        public void Initialize()
        {
            Debug.Log(InstanceID + " initializing");

            _gfxMemory = new MemoryInstance(InstanceID + "_gfx");
            _gfxMemory.Connect();

            _eventInMemory = new MemoryInstance(InstanceID + "_event_in");
            _eventInMemory.Connect();

            _eventOutMemory = new MemoryInstance(InstanceID + "_event_out");
            _eventOutMemory.Connect();

            IsInitialized = true;
        }

        public void SendEvent(CefEvent cefEvent)
        {
            Debug.Log("Sent event " + cefEvent);
            _eventInMemory.WriteEvent(cefEvent);
        }
    }
}
