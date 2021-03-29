using CefServer.Chromium;
using CefShared;

namespace CefServer
{
    public class EventDispatcher
    {
        public static void ForwardEvent(string instanceID, CefEvent cefEvent)
        {
            InstanceManager.GetInstance(instanceID).HandleEvent(cefEvent);
        }
    }
}
