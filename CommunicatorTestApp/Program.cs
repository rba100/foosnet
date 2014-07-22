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
            ci.SubscribeEmail("martin.podlubny@red-gate.com");
            ci.StatusChanged += c_StatusChanged;
            
            new System.Threading.AutoResetEvent(false).WaitOne();
        }

        static void c_StatusChanged(Object sender, StatusChangedEventArgs e)
        {
            Console.WriteLine("{0} is now {1}.", e.Email, e.CurrentStatus);
        }
    }
}
