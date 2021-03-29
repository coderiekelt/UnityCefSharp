using SharedMemory;
using System;
using System.Collections.Generic;
using System.IO;

namespace CefShared.Memory
{
    public class MemoryInstance
    {
        private string _name;
        private SharedArray<byte> _sharedBuffer;
        private int _size;

        private bool _isOpen;

        public MemoryInstance(string name)
        {
            _name = name;
        }

        public bool Init(int size)
        {
            _sharedBuffer = new SharedArray<byte>(_name, size);
            _size = size;
            _isOpen = true;

            return _isOpen;
        }

        public bool Connect()
        {
            try
            {
                _sharedBuffer = new SharedArray<byte>(_name);
                _isOpen = true;
            }
            catch (Exception ex)
            {
                _isOpen = false;
            }

            return _isOpen;
        }

        public void WriteBytes(byte[] bytes)
        {
            if (_isOpen)
            {
                if (bytes.Length > _sharedBuffer.Length)
                {
                    Resize(bytes.Length);
                }

                _sharedBuffer.Write(bytes);
            }
        }

        public void Resize(int size)
        {
            if (_sharedBuffer.Length != size)
            {
                _sharedBuffer.Close();
                _sharedBuffer = new SharedArray<byte>(_name, size);
            }
        }

        public byte[] ReadBytes()
        {
            byte[] buffer = null;
            if (_isOpen)
            {
                buffer = new byte[_sharedBuffer.Count];
                _sharedBuffer.CopyTo(buffer);
            }

            return buffer;
        }

        public bool IsOpen()
        {
            return _isOpen;
        }

        public CefEvent[] ReadEvents()
        {
            List<CefEvent> events = new List<CefEvent>();
            byte[] eventBuffer = ReadBytes();
            MemoryStream bufferStream = new MemoryStream(eventBuffer);

            int bufferPos = 0;

            while (bufferPos < bufferStream.Length)
            {
                byte[] eventID = new byte[4];

                bufferPos += bufferStream.Read(eventID, bufferPos, 4);

                int realEventID = Convert.ToInt32(eventID);
            }

            return events.ToArray();
        }

        public void WriteEvent(CefEvent cefEvent)
        {
            byte[] eventBuffer = cefEvent.Serialize();
            byte[] eventIDBuffer = new byte[4];

            BitConverter.GetBytes(cefEvent.GetEventID());

            _sharedBuffer.Write(eventIDBuffer);
            _sharedBuffer.Write(eventBuffer);
        }
    }
}
