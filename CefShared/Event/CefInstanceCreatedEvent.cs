using System.IO;
using Lidgren.Network;

namespace CefShared.Event
{
    public class CefInstanceCreatedEvent : CefEvent
    {
        public override int GetEventID()
        {
            return 1001;
        }

        public override NetOutgoingMessage Serialize(NetOutgoingMessage message)
        {
            message.Write(InstanceID);

            return message;
        }

        public override void Deserialize(NetIncomingMessage message)
        {
            InstanceID = message.ReadString();
        }
    }
}