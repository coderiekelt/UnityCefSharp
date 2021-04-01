using Lidgren.Network;

namespace CefShared.Event
{
    public class CefEvalJavascriptEvent : CefEvent
    {
        public string CallbackID = "";

        public string Javascript = "";

        public override int GetEventID()
        {
            return 3000;
        }

        public override NetOutgoingMessage Serialize(NetOutgoingMessage message)
        {
            message.Write(InstanceID);
            message.Write(CallbackID);
            message.Write(Javascript);

            return message;
        }

        public override void Deserialize(NetIncomingMessage message)
        {
            InstanceID = message.ReadString();
            CallbackID = message.ReadString();
            Javascript = message.ReadString();
        }
    }
}
