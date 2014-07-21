using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FoosNet.Network;
using FoosNet.Network.PeerDiscovery;
using FoosNet.Network.TcpServer;

namespace RobinTestApp
{
    class Program
    {
        static void Main()
        {
            var peerFinder = new UdpPeerFinder(Console.ReadLine(), 7331, TimeSpan.FromSeconds(1));
            peerFinder.PlayerAvailable += peerFinder_PlayerAvailable;
            Console.ReadKey(true);
        }

        static void peerFinder_PlayerAvailable(string email, IPAddress address)
        {
            Console.WriteLine("{0} on {1}", email, address);
        }

        static void MainOld(string[] args)
        {
            var client = new FoosballClientTcp("localhost", 7331);

            client.SetStatus("Robin", Status.Busy);
            client.SetStatus("Reka", Status.Available);
            client.SetStatus("Martin", Status.Available);
            client.SetStatus("Mark", Status.Available);
            client.SetStatus("Jason", Status.Available);

            var players = client.GetPlayers();

            foreach (var player in players)
            {
                Console.WriteLine(player.Name + " : " + player.Status);
            }

            Console.ReadLine();
        }
    }
}
