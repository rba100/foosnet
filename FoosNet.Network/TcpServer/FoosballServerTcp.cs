using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;

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
            PlayerDetail currentPlayer = null;
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
                            if (currentPlayer != null) m_Players.RemoveAll(p => p.Name == currentPlayer.Name);
                            m_Players.Add(player);
                            currentPlayer = player;
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
            if (currentPlayer != null) m_Players.RemoveAll(p=>p.Name == currentPlayer.Name);
        }
    }
}
