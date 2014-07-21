using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FoosNet.Network.TcpServer
{
    public class TcpFoosballPlayer : IFoosPlayer
    {
        public string Name { get; private set; }
        public Status Status { get; private set; }
        private FoosballClientTcp m_Client;

        public TcpFoosballPlayer(string name, Status status, FoosballClientTcp client)
        {
            Name = name;
            Status = status;
            m_Client = client;
        }

        public Task<ChallengeResponse> ChallengePlayer()
        {
            throw new System.NotImplementedException();
        }
    }

    public class FoosballClientTcp : IFoosballNetwork
    {
        private readonly IPEndPoint m_EndPoint;
        private readonly TcpClient m_Client;
        private Stream m_Stream;

        public FoosballClientTcp(string address, int port)
        {
            var entry = Dns.GetHostEntry(address);
            m_EndPoint = new IPEndPoint(entry.AddressList.FirstOrDefault(p=>p.AddressFamily == AddressFamily.InterNetwork) , port);
            m_Client = new TcpClient();
        }

        public IEnumerable<IFoosPlayer> GetPlayers()
        {
            Connect();

            m_Stream.WriteByte((byte)ClientOperation.GetPlayers);
            m_Stream.Flush();
            var responseCode = m_Stream.ReadByte();
            if (responseCode != (byte)ClientOperation.GetPlayers) throw new Exception("Unexpected op code");

            var response = PlayerList.FromStream(m_Stream);

            return response.Players.Select(foosPlayer => new TcpFoosballPlayer(foosPlayer.Name, foosPlayer.PlayerStatus, this));
        }

        public void SetStatus(string name, Status status)
        {
            Connect();
            m_Stream.WriteByte((byte)ClientOperation.Register);
            var player = new PlayerDetail() { Name = name, PlayerStatus = status };
            player.ToXmlStream(m_Stream);
            m_Stream.Flush();
        }

        private void Connect()
        {
            if (!m_Client.Connected)
            {
                m_Client.Connect(m_EndPoint);
                m_Stream = m_Client.GetStream();
            }
        }
    }
}
