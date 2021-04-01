using CefShared;
using CefShared.Event;
using CefShared.Memory;
using UnityEngine;
using System.Threading;
using Unity.Collections;

namespace CefClient.Chromium
{
    public class CefInstance : MonoBehaviour
    {
        private MemoryInstance _gfxMemory;

        public string InstanceID;
        public int TargetFPS = 24;

        private int _width;
        private int _height;
        private string _url;

        private byte[] _viewTextureBuffer;
        private NativeArray<byte> _viewTextureArray;
        private Thread _renderThread;

        public Texture2D ViewTexture;
        public bool IsInitialized { get; private set; }

        void Awake()
        {
            _renderThread = new Thread(RenderThread);
        }

        void Update()
        {
            if (!IsInitialized)
            {
                return;
            }

            if (_viewTextureBuffer == null)
            {
                _viewTextureBuffer = new byte[_width * _height * 4];
                ViewTexture = new Texture2D(_width, _height, TextureFormat.BGRA32, false, true);

                _viewTextureArray = ViewTexture.GetRawTextureData<byte>();
                _renderThread.Start();

                Debug.Log("Starting render thread");
            }

            ViewTexture.Apply(true);
        }

        void OnDestroy()
        {
            if (_renderThread.IsAlive == false) { return; }

            _renderThread.Abort();
        }

        public void Configure(int width, int height, string url)
        {
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
            _gfxMemory = new MemoryInstance(InstanceID + "_gfx");
            _gfxMemory.Connect();

            IsInitialized = true;
        }

        public void SendEvent(CefEvent cefEvent)
        {
            InstanceManager.Instance.SendEvent(cefEvent);
        }

        public void ReceiveEvent(CefEvent cefEvent)
        {
            if (cefEvent is CefInstanceCreatedEvent)
            {
                Initialize();

                return;
            }
        }

        private void RenderThread()
        {
            while (true)
            {
                _viewTextureBuffer = _gfxMemory.ReadBytes();
                _viewTextureArray.CopyFrom(_viewTextureBuffer);

                Thread.Sleep(1000 / TargetFPS); // We'll try...
            }
        }
    }
}
