using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FoosNet.Network.PeerDiscovery
{
    public class UdpPeerFinder : IDisposable
    {
        private readonly string m_Email;
        private readonly IPEndPoint m_BroadcastEndPoint;
        private readonly UdpClient m_BroadcastClient;
        private readonly Timer m_BroadcasTimer;

        private readonly UdpClient m_RecieveClient;
        private readonly Thread m_Manager;
        private readonly CancellationTokenSource m_Cts = new CancellationTokenSource();

        public delegate void PlayerAvailableHandler(string email, IPAddress ipAddress);
        public event PlayerAvailableHandler PlayerAvailable;

        private CancellationToken CancellationToken { get { return m_Cts.Token; } }

        public UdpPeerFinder(string email, int port, TimeSpan broadcastInterval)
        {
            m_Email = email;

            m_BroadcastEndPoint = new IPEndPoint(IPAddress.Parse("10.120.115.255") /*IPAddress.Broadcast*/, port);
            var localAddress = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .SelectMany(n => n.GetIPProperties().UnicastAddresses)
                .First(a => a.Address.ToString().StartsWith("10."))
                .Address;
            m_BroadcastClient = new UdpClient(new IPEndPoint(localAddress, port)) { EnableBroadcast = true };
            m_BroadcasTimer = new Timer(Broadcast, null, TimeSpan.Zero, broadcastInterval);

            m_RecieveClient = new UdpClient(new IPEndPoint(IPAddress.Any, port));

            m_Manager = new Thread(Manager);
            m_Manager.Start();
        }

        private void Broadcast(object state)
        {
            m_BroadcastClient.Send(m_Email, m_BroadcastEndPoint);
        }

        private void Manager()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                IPEndPoint endPoint = null;
                var bytes = m_RecieveClient.Receive(ref endPoint);
                if (PlayerAvailable != null) { PlayerAvailable(Encoding.UTF8.GetString(bytes), endPoint.Address); }
            }
        }

        public void Dispose()
        {
            m_Cts.Cancel();
            m_Manager.Abort();
            m_BroadcasTimer.Dispose();
            m_BroadcastClient.Close();
        }
    }
}
