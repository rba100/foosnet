using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FoosNet.Network.PeerDiscovery
{
    static class UdpClientExtensions
    {
        public static void Send(this UdpClient client, string message, IPEndPoint endPoint)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            client.Send(bytes, bytes.Length, endPoint);
        }

        public static void Send(this UdpClient client, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            client.Send(bytes, bytes.Length);
        }
    }
}
