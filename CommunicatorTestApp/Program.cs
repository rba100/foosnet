using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoosNet.CommunicatorIntegration;

namespace CommunicatorTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CommunicatorIntegration ci = new CommunicatorIntegration();
            ci.OpenConversationWithRedgateEmail(CommunicatorIntegration.NameToRegateEmail("christopher", "moore"));

            Console.ReadKey(true);
        }
    }
}
