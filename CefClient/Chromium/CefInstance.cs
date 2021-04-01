using CefShared;
using CefShared.Event;
using CefShared.Memory;
using UnityEngine;
using System.Threading;
using Unity.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace CefClient.Chromium
{
    public class CefInstance : MonoBehaviour
    {
        private MemoryInstance _gfxMemory;

        public string InstanceID;
        public int TargetFPS = 24;
        public bool ApplyMipMaps = true;

        private int _width;
        private int _height;
        private string _url;

        private byte[] _viewTextureBuffer;
        private NativeArray<byte> _viewTextureArray;
        private Thread _renderThread;

        private Dictionary<string, Action<string>> _jsCallbacks;

        public Texture2D ViewTexture;
        public bool IsInitialized { get; private set; }

        void Awake()
        {
            Debug.Log(System.Reflection.Assembly.GetExecutingAssembly());

            _jsCallbacks = new Dictionary<string, Action<string>>();
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
            }

            ViewTexture.Apply(ApplyMipMaps);
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

           SendEvent(createEvent);
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

            if (cefEvent is CefJavascriptResultEvent)
            {
                CefJavascriptResultEvent javascriptEvent = (CefJavascriptResultEvent)cefEvent;

                _jsCallbacks[javascriptEvent.CallbackID](javascriptEvent.Result);
                _jsCallbacks.Remove(javascriptEvent.CallbackID);

                return;
            }

            if (cefEvent is CefJavascriptStaticCallEvent)
            {
                CefJavascriptStaticCallEvent staticCallEvent = (CefJavascriptStaticCallEvent)cefEvent;

                try
                {
                    string result = (string) GetType(staticCallEvent.Namespace).GetMethod(staticCallEvent.Method).Invoke(this, staticCallEvent.Arguments);

                    RespondToStaticCall(staticCallEvent, result);
                } catch (Exception e) {
                    Debug.LogError(e.Message);
                    Debug.LogError(e.StackTrace);

                    RespondToStaticCall(staticCallEvent, "error");
                }

                return;
            }
        }

        public void Eval(string javascript)
        {
            CefEvalJavascriptEvent javascriptEvent = new CefEvalJavascriptEvent()
            {
                InstanceID = InstanceID,
                Javascript = javascript,
            };

            SendEvent(javascriptEvent);
        }

        public void EvalCallback(string javascript, Action<string> callback)
        {
            string callbackId = Guid.NewGuid().ToString();
            _jsCallbacks[callbackId] = callback;

            CefEvalJavascriptEvent cefEvent = new CefEvalJavascriptEvent
            {
                InstanceID = InstanceID,
                CallbackID = callbackId,
                Javascript = javascript,
            };

            SendEvent(cefEvent);
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

        private void RespondToStaticCall(CefJavascriptStaticCallEvent cefEvent, string response)
        {
            SendEvent(new CefJavascriptResultEvent()
            {
                InstanceID = InstanceID,
                CallbackID = cefEvent.CallbackID,
                Result = response,
            });
        }

        #region Reflection helpers
        public static Type GetType(string TypeName)
        {

            // Try Type.GetType() first. This will work with types defined
            // by the Mono runtime, in the same assembly as the caller, etc.
            var type = Type.GetType(TypeName);

            // If it worked, then we're done here
            if (type != null)
                return type;

            // If the TypeName is a full name, then we can try loading the defining assembly directly
            if (TypeName.Contains("."))
            {

                // Get the name of the assembly (Assumption is that we are using 
                // fully-qualified type names)
                var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

                // Attempt to load the indicated Assembly
                var assembly = Assembly.Load(assemblyName);
                if (assembly == null)
                    return null;

                // Ask that assembly to return the proper Type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;

            }

            // If we still haven't found the proper type, we can enumerate all of the 
            // loaded assemblies and see if any of them define the type
            var currentAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in referencedAssemblies)
            {
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }

            // The type just couldn't be found...
            return null;

        }

        #endregion
    }
}
