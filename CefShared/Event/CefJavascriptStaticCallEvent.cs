using Lidgren.Network;
using Newtonsoft.Json;

namespace CefShared.Event
{
    public class CefJavascriptStaticCallEvent : CefEvent
    {
        public string CallbackID = "";

        public string Method = "";

        public string Namespace = "";

        public string[] Arguments = new string[] { };

        public override int GetEventID()
        {
            return 3002;
        }

        public override NetOutgoingMessage Serialize(NetOutgoingMessage message)
        {
            message.Write(InstanceID);
            message.Write(CallbackID);
            message.Write(Method);
            message.Write(Namespace);
            message.Write(JsonConvert.SerializeObject(Arguments, Formatting.None));

            return message;
        }

        public override void Deserialize(NetIncomingMessage message)
        {
            InstanceID = message.ReadString();
            CallbackID = message.ReadString();
            Method = message.ReadString();
            Namespace = message.ReadString();
            Arguments = JsonConvert.DeserializeObject<string[]>(message.ReadString());
        }
    }
}
