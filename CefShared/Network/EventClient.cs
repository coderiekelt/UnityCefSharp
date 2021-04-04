using CefShared.Network.EventArgs;
using Lidgren.Network;
using System.Threading;

namespace CefShared.Network
{
    public class EventClient
    {
        private NetClient _netClient;
        private Thread _thread;
        private int _port;

        public EventReceivedHandler OnEventReceived;

        public EventClient(int port)
        {
            NetPeerConfiguration netConfig = new NetPeerConfiguration("cef_event");

            _port = port;

            _netClient = new NetClient(netConfig);
            _netClient.RegisterReceivedCallback(new SendOrPostCallback(ReceiveMessage));

            _thread = new Thread(ThreadStart);
        }

        public bool IsReady()
        {
            return _netClient.ConnectionStatus == NetConnectionStatus.Connected;
        }

        public void WriteEvent(CefEvent cefEvent)
        {
            NetOutgoingMessage message = _netClient.CreateMessage();

            message.Write(cefEvent.GetEventID());
            message = cefEvent.Serialize(message);

            _netClient.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void Start()
        {
            _thread.Start();
        }

        private void ThreadStart()
        {
            _netClient.Start();
            _netClient.Connect("127.0.0.1", _port);
        }

        private void ReceiveMessage(object peer)
        {
            NetIncomingMessage message;

            while ((message = _netClient.ReadMessage()) != null)
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
