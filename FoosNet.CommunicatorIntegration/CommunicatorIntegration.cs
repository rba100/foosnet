using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using CommunicatorAPI;
using FoosNet.Network;

namespace FoosNet.CommunicatorIntegration
{
    public class CommunicatorIntegration
    {
        private IMessengerAdvanced m_Messenger;
        private string m_ServiceID;

        public CommunicatorIntegration()
        {
            m_Messenger = (IMessengerAdvanced) new Messenger();
            m_ServiceID = m_Messenger.MyServiceId;
        }

        public Status StatusOfRedgateEmail(string email)
        {
            IMessengerContact contact = GetContactByRedGateEmail(email);
            switch (contact.Status)
            {
                case MISTATUS.MISTATUS_ONLINE:
                case MISTATUS.MISTATUS_MAY_BE_AVAILABLE:
                    return Status.Available;
                case MISTATUS.MISTATUS_OFFLINE:
                    return Status.Offline;
                default:
                    return Status.Busy;
            }
        }

        public IMessengerConversationWndAdvanced OpenConversationWithRedgateEmail(string email)
        {
            return m_Messenger.InstantMessage(GetContactByRedGateEmail(email));
        }

        public static string NameToRegateEmail(string first, string last)
        {
            return first + "." + last + "@red-gate.com";
        }

        private dynamic GetContactByRedGateEmail(string email)
        {
            return m_Messenger.GetContact(email, m_ServiceID);
        }
    }
}
