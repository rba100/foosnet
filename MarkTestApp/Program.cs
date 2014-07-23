using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using FoosNet.Network;

namespace MarkTestApp
{
    class Program
    {
        static void Main()
        {
            IFoosNetworkService foosnet1 = new FoosNetworkService("ws://mdr-foosnet.testnet.red-gate.com/Api/Socket", "mark.raymond@red-gate.com");
            Thread.Sleep(400);
            IFoosNetworkService foosnet2 = new FoosNetworkService("ws://mdr-foosnet.testnet.red-gate.com/Api/Socket", "robin.anderson@red-gate.com");
            foosnet1.PlayersDiscovered += players => Console.WriteLine("Mark discovered {0}", string.Join(", ", players.Players.Select(p => p.Email)));
            foosnet2.PlayersDiscovered += players => Console.WriteLine("Robin discovered {0}", string.Join(", ", players.Players.Select(p => p.Email)));
            foosnet2.GameStarting += players => Console.WriteLine("Robin sees game starting with {0}", string.Join(", ", players.Players.Select(p => p.Email)));
            foosnet2.ChallengeReceived += request => Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Robin recieved challenge");
                Thread.Sleep(1000);
                Console.WriteLine("Robin accepting challenge");
                foosnet2.Respond(new ChallengeResponse(request.Challenger, true));
            });
            foosnet1.ChallengeResponse += response => Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Mark recieved Robin's response {0}", response.Accepted);
                Thread.Sleep(1000);
                Console.WriteLine("Mark starting game");
                foosnet1.StartGame(new [] { response.Player });
            });
            Thread.Sleep(1000);
            Console.WriteLine("Mark sending Robin challenge");
            foosnet1.Challenge(new LivePlayer("robin.anderson@red-gate.com"));
            Console.ReadKey(true);
            foosnet1.Dispose();
            foosnet2.Dispose();
            Console.ReadKey(true);
        }
    }
}
