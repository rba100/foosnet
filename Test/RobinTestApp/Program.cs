using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoosNet.Network;
using FoosNet.Network.TcpServer;

namespace RobinTestApp
{
    class Program
    {
        static void Main(string[] args)
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
