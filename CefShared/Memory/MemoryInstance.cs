using SharedMemory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

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
                Thread.Sleep(100);
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
    }
}
