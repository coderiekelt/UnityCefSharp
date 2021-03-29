using CefServer.Chromium;
using System.Collections.Generic;

namespace CefServer
{
    public static class InstanceManager
    {
        private static Dictionary<string, CefInstance> _instances = new Dictionary<string, CefInstance>();

        public static string CreateInstance(string guid = null)
        {
            CefInstance instance;

            if (guid != null)
            {
                instance = new CefInstance(guid);
                _instances[guid] = instance;

                return guid;
            }

            instance = new CefInstance();
            guid = instance.InstanceID;
            _instances[guid] = instance;

            return guid;
        }

        public static CefInstance GetInstance(string guid)
        {
            return _instances[guid];
        }
    }
}
