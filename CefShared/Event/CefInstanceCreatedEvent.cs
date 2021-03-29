using System.IO;

namespace CefShared.Event
{
    public class CefInstanceCreatedEvent : CefEvent
    {
        public string InstanceID;

        public override int GetEventID()
        {
            return 1001;
        }

        public override byte[] Serialize()
        {
            WriteStringToBuffer(InstanceID);

            return FlushBuffer();
        }

        public override void Deserialize(MemoryStream stream)
        {
            InstanceID = ReadStringFromStream(stream);
        }
    }
}