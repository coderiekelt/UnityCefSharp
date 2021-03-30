using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;

namespace CefShared
{
    public class CefEvent
    {
        public string InstanceID;

        public virtual int GetEventID()
        {
            return -1;
        }

        public virtual NetOutgoingMessage Serialize(NetOutgoingMessage netOutgoingMessage)
        {
            return null;
        }

        public virtual void Deserialize(NetIncomingMessage message)
        {

        }
    }
}
