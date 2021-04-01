using CefShared.Event;
using CefSharp;
using System;
using System.Collections.Generic;

namespace CefServer.Chromium
{
    public class CefStaticBinding
    {
        private CefInstance _cefIntance;
        private Dictionary<string, IJavascriptCallback> _jsCallbacks;

        public CefStaticBinding(CefInstance cefInstance)
        {
            _cefIntance = cefInstance;
            _jsCallbacks = new Dictionary<string, IJavascriptCallback>();
        }

        public void Invoke(string ns, string method, List<object> arguments, IJavascriptCallback callback)
        {
            List<string> stringArgs = new List<string>();

            foreach (object argument in arguments)
            {
                stringArgs.Add((string)argument);
            }

            string callbackId = Guid.NewGuid().ToString();

            CefJavascriptStaticCallEvent callEvent = new CefJavascriptStaticCallEvent()
            {
                InstanceID = _cefIntance.InstanceID,
                CallbackID = callbackId,
                Namespace = ns,
                Method = method,
                Arguments = stringArgs.ToArray(),
            };

            _jsCallbacks[callbackId] = callback;

            Program.SendEvent(callEvent);
        }

        public void HandleEvent(CefJavascriptResultEvent cefEvent)
        {
            _jsCallbacks[cefEvent.CallbackID].ExecuteAsync(cefEvent.Result);
            _jsCallbacks.Remove(cefEvent.CallbackID);
        }
    }
}
