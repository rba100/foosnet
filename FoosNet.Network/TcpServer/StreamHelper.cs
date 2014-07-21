using System;
using System.Collections.Generic;
using System.IO;

namespace FoosNet.Network.TcpServer
{
    public static class StreamHelper
    {
        public static MemoryStream GetMessage(Stream stream)
        {
            var bytes = new List<byte>
            {
                (byte) stream.ReadByte(),
                (byte) stream.ReadByte(),
                (byte) stream.ReadByte(),
                (byte) stream.ReadByte()
            };
            var length = BitConverter.ToInt32(bytes.ToArray(), 0);
            var message = new byte[length];
            stream.Read(message, 0, length);
            var memStream = new MemoryStream(message);
            return memStream;
        }

        public static void SendMessage(MemoryStream source, Stream stream)
        {
            var header = BitConverter.GetBytes((int)source.Length);
            stream.Write(header, 0, header.Length);
            source.CopyTo(stream);
            stream.Flush();
        }
    }
}