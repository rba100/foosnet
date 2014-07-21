using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Xml.Serialization;

namespace FoosNet.Network.TcpServer
{
    public class FoosballServerTcp
    {
        private readonly int m_PortNumber;
        private Thread m_ManagerThread;
        private TcpListener m_Listener;
        private List<Thread> m_Clients = new List<Thread>();
        private readonly CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();
        private CancellationToken CancellationToken { get { return m_CancellationTokenSource.Token; } }
        private List<PlayerDetail> m_Players = new List<PlayerDetail>();

        public FoosballServerTcp(int port)
        {
            if (port < 1024) throw new Exception("Don't use that port number, obviously");
            m_PortNumber = port;
        }

        public void Start()
        {
            m_Listener = new TcpListener(IPAddress.Any, m_PortNumber);
            m_Listener.Start();
            m_ManagerThread = new Thread(Manager);
            m_ManagerThread.Start();
        }

        public void Stop()
        {
            m_Listener.Stop();
            m_CancellationTokenSource.Cancel();
            foreach (var client in m_Clients)
            {
                client.Join(50);
                client.Abort();
            }
            m_Clients.Clear();
        }

        void Manager()
        {
            try
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    var client = m_Listener.AcceptTcpClient();
                    var workerThread = new Thread(Worker);
                    workerThread.Start(client);
                    m_Clients.Add(workerThread);
                }
            }
            catch { }
        }

        void Worker(object tcpClient)
        {
            var client = tcpClient as TcpClient;
            var stream = client.GetStream();
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var operation = (ClientOperation)stream.ReadByte();
                    switch (operation)
                    {
                        case ClientOperation.Register:
                            var player = PlayerDetail.FromStream(stream);
                            m_Players.RemoveAll(p => p.Name == player.Name);
                            m_Players.Add(player);
                            break;
                        case ClientOperation.GetPlayers:
                            stream.WriteByte((byte)ClientOperation.GetPlayers);
                            var playerList = new PlayerList { Players = new List<PlayerDetail>(m_Players) };
                            playerList.ToXmlStream(stream);
                            stream.Flush();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch
                {
                    break;
                }
            }
            m_Clients.Remove(Thread.CurrentThread);
        }
    }

    public enum ClientOperation { Register, GetPlayers }

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
            var length = BitConverter.ToInt32(bytes.ToArray(),0);
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
