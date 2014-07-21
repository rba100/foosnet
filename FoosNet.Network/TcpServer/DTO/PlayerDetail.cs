using System;
using System.IO;
using System.Xml.Serialization;

namespace FoosNet.Network.TcpServer
{
    [Serializable]
    public class PlayerDetail
    {
        public string Name;
        public Status PlayerStatus;

        public void ToXmlStream(Stream stream)
        {
            var memStream = new MemoryStream();
            var serialiser = new XmlSerializer(typeof(PlayerDetail));
            serialiser.Serialize(memStream, this);
            memStream.Seek(0, SeekOrigin.Begin);
            StreamHelper.SendMessage(memStream, stream);
            memStream.Dispose();
        }

        public static PlayerDetail FromStream(Stream stream)
        {
            var serialiser = new XmlSerializer(typeof(PlayerDetail));
            using (var message = StreamHelper.GetMessage(stream))
            {
                var player = serialiser.Deserialize(message) as PlayerDetail;
                return player;
            }
        }
    }
}