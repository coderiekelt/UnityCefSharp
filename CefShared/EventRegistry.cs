using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefShared
{
    public static class EventRegistry
    {
        public static Dictionary<int, string> EventDictionary = new Dictionary<int, string>();

        public static void BuildDictionary()
        {
            // TODO: Reflection
            EventDictionary.Add(1000, "CefShared.Event.CefCreateInstanceEvent");
            EventDictionary.Add(1001, "CefShared.Event.CefInstanceCreatedEvent");
            EventDictionary.Add(2000, "CefShared.Event.CefKeyboardEvent");
            EventDictionary.Add(2001, "CefShared.Event.CefMouseEvent");
            EventDictionary.Add(3000, "CefShared.Event.CefEvalJavascriptEvent");
            EventDictionary.Add(3001, "CefShared.Event.CefJavascriptResultEvent");
        }

        public static CefEvent CreateByID(int id)
        {
            string eventType = EventDictionary[id];
            CefEvent cefEvent = (CefEvent) Activator.CreateInstance(Type.GetType(eventType));

            return cefEvent;
        }
    }
}
