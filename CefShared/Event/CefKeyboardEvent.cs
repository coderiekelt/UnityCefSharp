using System.IO;
using Lidgren.Network;

namespace CefShared.Event
{
    public class CefKeyboardEvent : CefEvent
    {
        public string InputString;

        public override int GetEventID()
        {
            return 2000;
        }

        public override NetOutgoingMessage Serialize(NetOutgoingMessage message)
        {
            message.Write(InstanceID);
            message.Write(InputString);

            return message;
        }

        public override void Deserialize(NetIncomingMessage message)
        {
            InstanceID = message.ReadString();
            InputString = message.ReadString();
        }
    }
}