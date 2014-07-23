using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FoosNet.Network;

namespace MarkTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var foosnet1 = new FoosNetworkService("ws://mdr-foosnet.testnet.red-gate.com/Api/Socket", "mark.raymond@red-gate.com");
            var foosnet2 = new FoosNetworkService("ws://mdr-foosnet.testnet.red-gate.com/Api/Socket", "robin.anderson@red-gate.com");
            foosnet2.ChallengeReceived += request => Console.WriteLine("Robin recieved challenge");
            Thread.Sleep(1000);
            Console.WriteLine("Mark sending Robin challenge");
            foosnet1.Challenge(new LivePlayer("robin.anderson@red-gate.com"));
            Console.ReadKey(true);
        }
    }
}
