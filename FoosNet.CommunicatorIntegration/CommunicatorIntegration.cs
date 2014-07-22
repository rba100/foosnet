using System;
using System.Collections;
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
        private Messenger m_Messenger;
        private string m_ServiceID;

        private HashSet<string> m_subscribedStatusChangeEmails;

        public CommunicatorIntegration()
        {
            if (m_Messenger == null || m_ServiceID == null)
            {
                m_Messenger = new Messenger();
                m_ServiceID = m_Messenger.MyServiceId;
                m_Messenger.OnContactStatusChange += new DMessengerEvents_OnContactStatusChangeEventHandler(communicator_OnContactStatusChange);
                m_Messenger.OnMyStatusChange += new DMessengerEvents_OnMyStatusChangeEventHandler(communicator_OnMyStatusChange);
            }

            m_subscribedStatusChangeEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public Status StatusOfRedgateEmail(string email)
        {
            IMessengerContact contact = GetContactByRedGateEmail(email);
            return MistatusToFoosStatus(contact.Status);
        }

        private static Status MistatusToFoosStatus(MISTATUS miStatus)
        {
            switch (miStatus)
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

        public void SubscribeEmail(string email)
        {
            m_subscribedStatusChangeEmails.Add(email.ToLower());
        }

        public void UnsubscribeEmail(string email)
        {
            m_subscribedStatusChangeEmails.Remove(email.ToLower());
        }

        public void communicator_OnContactStatusChange(object pMContact, MISTATUS mStatus)
        {
            Console.WriteLine("on contact status change");
            IMessengerContactAdvanced contact = (IMessengerContactAdvanced) pMContact;
            if (m_subscribedStatusChangeEmails.Contains(contact.SigninName))
            {
                StatusChangedEventArgs args = new StatusChangedEventArgs();
                args.Email = contact.SigninName.ToLower();
                args.CurrentStatus = MistatusToFoosStatus(contact.Status);
                args.TimeOfChange = DateTime.Now;
                OnStatusChanged(args);
            }
        }

        public void communicator_OnMyStatusChange(int i, MISTATUS mStatus)
        {
            Console.WriteLine("my status change");
        }

        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler handler = StatusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event StatusChangedEventHandler StatusChanged;
    }
}
