using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoosNet.Network;

namespace MarkTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var foosnet = new FoosNetworkService("ws://mdr-foosnet.testnet.red-gate.com/Api/Socket", "mark.raymond@red-gate.com");
            Console.ReadKey(true);
        }
    }
}
