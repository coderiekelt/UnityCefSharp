using CefShared.Network.EventArgs;
using Lidgren.Network;
using System.Threading;

namespace CefShared.Network
{
    public delegate void EventReceivedHandler(object sender, EventReceivedEventArgs args);

    public class EventServer
    {
        private NetServer _netServer;
        private Thread _thread;

        public EventReceivedHandler OnEventReceived;

        public EventServer(int listenPort)
        {
            NetPeerConfiguration netConfig = new NetPeerConfiguration("cef_event");
            netConfig.Port = listenPort;

            _netServer = new NetServer(netConfig);
            _netServer.RegisterReceivedCallback(new SendOrPostCallback(ReceiveMessage));

            _thread = new Thread(ThreadStart);
        }

        public void WriteEvent(CefEvent cefEvent)
        {
            NetOutgoingMessage message = _netServer.CreateMessage();

            message.Write(cefEvent.GetEventID());
            message = cefEvent.Serialize(message);

            _netServer.SendToAll(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void Start()
        {
            _thread.Start();
        }

        private void ThreadStart()
        {
            _netServer.Start();
        }

        private void ReceiveMessage(object peer)
        {
            NetIncomingMessage message;

            while ((message = _netServer.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        int eventID = message.ReadInt32();

                        CefEvent cefEvent = EventRegistry.CreateByID(eventID);
                        cefEvent.Deserialize(message);

                        OnEventReceived(this, new EventReceivedEventArgs { CefEvent = cefEvent });

                        break;
                    default:
                        break;
                }
            }
        }
    }
}
