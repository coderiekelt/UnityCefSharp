using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefShared.Event
{
    public class CefCreateInstanceEvent : CefEvent
    {
        public int EventID = 1001;

        public string InstanceID;

        public int Width;

        public int Height;

        public string Url;

        public override byte[] Serialize()
        {
            WriteStringToBuffer(InstanceID);
            WriteInt32ToBuffer(Width);
            WriteInt32ToBuffer(Height);
            WriteStringToBuffer(Url);

            return FlushBuffer();
        }

        public override void Deserialize(MemoryStream stream)
        {
            InstanceID = ReadStringFromStream(stream);
            Width = ReadInt32FromStream(stream);
            Height = ReadInt32FromStream(stream);
            Url = ReadStringFromStream(stream);
        }
    }
}
