using System.IO;
using Lidgren.Network;

namespace CefShared.Event
{
    public class CefKeyboardEvent : CefEvent
    {
        public int Key;

        public bool IsChar = false;

        public bool IsDown = false;

        public bool Shift = false;

        public bool Alt = false;

        public override int GetEventID()
        {
            return 2000;
        }

        public override NetOutgoingMessage Serialize(NetOutgoingMessage message)
        {
            message.Write(InstanceID);
            message.Write(Key);
            message.Write(IsChar);
            message.Write(IsDown);
            message.Write(Shift);
            message.Write(Alt);

            return message;
        }

        public override void Deserialize(NetIncomingMessage message)
        {
            InstanceID = message.ReadString();
            Key = message.ReadInt32();
            IsChar = message.ReadBoolean();
            IsDown = message.ReadBoolean();
            Shift = message.ReadBoolean();
            Alt = message.ReadBoolean();
        }
    }
}