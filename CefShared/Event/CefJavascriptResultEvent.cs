using Lidgren.Network;

namespace CefShared.Event
{
    public class CefJavascriptResultEvent : CefEvent
    {
        public string CallbackID = "";

        public string Result = "";

        public override int GetEventID()
        {
            return 3001;
        }

        public override NetOutgoingMessage Serialize(NetOutgoingMessage message)
        {
            message.Write(InstanceID);
            message.Write(CallbackID);
            message.Write(Result);

            return message;
        }

        public override void Deserialize(NetIncomingMessage message)
        {
            InstanceID = message.ReadString();
            CallbackID = message.ReadString();
            Result = message.ReadString();
        }
    }
}
