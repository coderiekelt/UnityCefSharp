using System.IO;
using Lidgren.Network;

namespace CefShared.Event
{
    public class CefKeyboardEvent : CefEvent
    {
        public string Key;

        public bool Shift = false;

        public bool AltGr = false;

        public override int GetEventID()
        {
            return 2000;
        }

        public override NetOutgoingMessage Serialize(NetOutgoingMessage message)
        {
            message.Write(InstanceID);
            message.Write(Key);
            message.Write(Shift);
            message.Write(AltGr);

            return message;
        }

        public override void Deserialize(NetIncomingMessage message)
        {
            InstanceID = message.ReadString();
            Key = message.ReadString();
            Shift = message.ReadBoolean();
            AltGr = message.ReadBoolean();
        }
    }
}