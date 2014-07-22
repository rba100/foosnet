using System;
using System.Net;

namespace RobinTestApp
{
    class Program
    {
        static void Main()
        {
            
            Console.ReadKey(true);
        }

        static void peerFinder_PlayerAvailable(string email, IPAddress address)
        {
            Console.WriteLine("{0} on {1}", email, address);
        }
    }
}
