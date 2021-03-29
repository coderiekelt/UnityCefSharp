using System;
using System.Collections.Generic;
using System.IO;

namespace CefShared
{
    public class CefEvent
    {
        protected List<byte[]> _writeBuffer;

        public CefEvent()
        {
            _writeBuffer = new List<byte[]>();
        }

        public virtual int GetEventID()
        {
            return -1;
        }

        public virtual byte[] Serialize()
        {
            return new byte[0];
        }

        public virtual void Deserialize(MemoryStream stream)
        {

        }

        protected void WriteStringToBuffer(string input)
        {
            int inputLength = System.Text.ASCIIEncoding.Unicode.GetByteCount(input);

            WriteInt32ToBuffer(inputLength);

            _writeBuffer.Add(System.Text.ASCIIEncoding.Unicode.GetBytes(input));
        }

        protected void WriteInt32ToBuffer(int input)
        {
            _writeBuffer.Add(BitConverter.GetBytes(input));
        }

        protected byte[] FlushBuffer()
        {
            int length = 0;

            foreach (byte[] array in _writeBuffer)
            {
                length += array.Length;
            }

            byte[] buffer = new byte[length];
            MemoryStream stream = new MemoryStream(buffer, 0, length, true, true);

            foreach (byte[] array in _writeBuffer)
            {
                stream.Write(array, 0, array.Length);
            }

            byte[] output = stream.GetBuffer();
            stream.Close();

            _writeBuffer.Clear();

            return output;
        }

        protected string ReadStringFromStream(MemoryStream stream)
        {
            int length = ReadInt32FromStream(stream);

            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);

            return System.Text.ASCIIEncoding.Unicode.GetString(buffer);
        }

        protected int ReadInt32FromStream(MemoryStream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);

            return BitConverter.ToInt32(buffer);
        }
    }
}
