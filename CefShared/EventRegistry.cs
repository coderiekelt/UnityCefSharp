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
            EventDictionary.Add(1000, "CefShared.Event.CefCreateInstanceEvent");
            EventDictionary.Add(1001, "CefShared.Event.CefInstanceCreatedEvent");
        }

        public static CefEvent CreateByID(int id)
        {
            CefEvent cefEvent = (CefEvent) Activator.CreateInstance(Type.GetType(EventDictionary[id]));

            return cefEvent;
        }
    }
}
