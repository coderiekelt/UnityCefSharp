using Lidgren.Network;
using System.IO;

namespace CefShared.Event
{
    public class CefCreateInstanceEvent : CefEvent
    {
        public int Width;

        public int Height;

        public string Url;

        public override int GetEventID()
        {
            return 1000;
        }

        public override NetOutgoingMessage Serialize(NetOutgoingMessage message)
        {
            message.Write(InstanceID);
            message.Write(Width);
            message.Write(Height);
            message.Write(Url);

            return message;
        }

        public override void Deserialize(NetIncomingMessage message)
        {
            InstanceID = message.ReadString();
            Width = message.ReadInt32();
            Height = message.ReadInt32();
            Url = message.ReadString();
        }
    }
}
