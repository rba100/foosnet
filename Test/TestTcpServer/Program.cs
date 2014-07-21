using System;
using FoosNet.Network.TcpServer;

namespace TestTcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Starting server...");
            var server = new FoosballServerTcp(7331);
            server.Start();
            Console.WriteLine("[DONE]");
            
            Console.Write("Press any key to shutdown server...");
            var key = Console.ReadKey();
            server.Stop();
        }
    }
}
