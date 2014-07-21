using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FoosNet.Network.TcpServer
{
    [Serializable]
    public class PlayerList
    {
        public List<PlayerDetail> Players;

        public void ToXmlStream(Stream stream)
        {
            var memStream = new MemoryStream();
            var serialiser = new XmlSerializer(typeof(PlayerList));
            serialiser.Serialize(memStream, this);
            memStream.Seek(0, SeekOrigin.Begin);
            StreamHelper.SendMessage(memStream, stream);
            memStream.Dispose();
        }

        public static PlayerList FromStream(Stream stream)
        {
            var serialiser = new XmlSerializer(typeof(PlayerList));
            using (var message = StreamHelper.GetMessage(stream))
            {
                var list = serialiser.Deserialize(message) as PlayerList;
                return list;
            }
        }
    }
}