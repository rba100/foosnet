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
            ci.StatusChanged += StatusChanged_EventHandler;
            ci.StatusChangedSubscribeEmail(ci.GetLocalUserEmail());
            ci.StatusChangedSubscribeEmail("robin.anderson@red-gate.com");
            
            new System.Threading.AutoResetEvent(false).WaitOne();
        }

        static void StatusChanged_EventHandler(Object sender, StatusChangedEventArgs e)
        {
            CommunicatorIntegration ci = (CommunicatorIntegration)sender;

            string msg = string.Format("{0}: {1} is now {2}.", e.TimeOfChange, e.Email, e.CurrentStatus);
            Console.WriteLine(msg);
            if (e.Email.ToLower().Equals("robin.anderson@red-gate.com") || e.Email.ToLower().Equals(ci.GetLocalUserEmail()))
            {
                ci.OpenConversationWithRedgateEmail("robin.anderson@red-gate.com", msg);
            }
        }
    }
}
