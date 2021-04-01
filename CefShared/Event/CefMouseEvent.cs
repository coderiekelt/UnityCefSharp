using System.IO;
using Lidgren.Network;

namespace CefShared.Event
{
    public class CefMouseEvent : CefEvent
    {
        public int MouseButton;

        public bool MouseButtonDown;

        public int MouseX;

        public int MouseY;

        public int ScollDeltaX = 0;

        public int ScollDeltaY = 0;

        public override int GetEventID()
        {
            return 2001;
        }

        public override NetOutgoingMessage Serialize(NetOutgoingMessage message)
        {
            message.Write(InstanceID);
            message.Write(MouseButton);
            message.Write(MouseButtonDown);
            message.Write(MouseX);
            message.Write(MouseY);
            message.Write(ScollDeltaX);
            message.Write(ScollDeltaY);

            return message;
        }

        public override void Deserialize(NetIncomingMessage message)
        {
            InstanceID = message.ReadString();
            MouseButton = message.ReadInt32();
            MouseButtonDown = message.ReadBoolean();
            MouseX = message.ReadInt32();
            MouseY = message.ReadInt32();
            ScollDeltaX = message.ReadInt32();
            ScollDeltaY = message.ReadInt32();
        }
    }
}