using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using CommunicatorAPI;
using FoosNet.Network;


//
// Sample usage:
// 
// CommunicatorIntegration ci = new CommunicatorIntegration();
// ci.StatusChanged += StatusChanged_EventHandler;
// ci.StatusChangedSubscribeEmail("martin.podlubny@red-gate.com");
// 
// throws CommunicatorNotOpenException, CommunicatorNotLoggedInToRedgateException on instantiation
// 
 
namespace FoosNet.CommunicatorIntegration
{
    public class CommunicatorIntegration
    {
        private Messenger m_Messenger;
        private string m_ServiceID;

        private Dictionary<string, IMessengerContact> m_SubscribedContacts; 

        public CommunicatorIntegration()
        {
            try
            {
                m_Messenger = new Messenger();
            }
            catch (Exception)
            {
                throw new CommunicatorNotOpenException();
            }
            
            m_ServiceID = m_Messenger.MyServiceId;

            try
            {
                m_Messenger.GetContact("red-gate@red-gate.com", m_ServiceID);
            }
            catch (Exception)
            {
                throw new CommunicatorNotLoggedInToRedgateException();
            }

            m_SubscribedContacts = new Dictionary<string, IMessengerContact>();

            m_Messenger.OnContactStatusChange += communicator_OnContactStatusChange;
        }

        public string GetLocalUserEmail()
        {
            return GetSelfContact().SigninName;
        }

        public string GetLocalUserFriendlyName()
        {
            return GetSelfContact().FriendlyName;
        }

        public Status GetLocalUserStatus()
        {
            return MistatusToFoosStatus(GetSelfContact().Status);
        }

        public Status StatusOfRedgateEmail(string email)
        {
            IMessengerContact contact = GetContactByRedGateEmail(email);
            return MistatusToFoosStatus(contact.Status);
        }

        public string FriendlyName(string email)
        {
            IMessengerContact contact = GetContactByRedGateEmail(email);
            return contact.FriendlyName;
        }

        public IMessengerConversationWndAdvanced OpenConversationWithRedgateEmail(string email)
        {
            return m_Messenger.InstantMessage(GetContactByRedGateEmail(email));
        }

        public IMessengerConversationWndAdvanced OpenConversationWithRedgateEmail(string email, string initialMessage)
        {
            IMessengerConversationWndAdvanced wnd = OpenConversationWithRedgateEmail(email);
            wnd.SendText(initialMessage);
            return wnd;
        }
        
        public void StatusChangedSubscribeEmail(string email)
        {
            m_SubscribedContacts.Add(email.ToLower(), GetContactByRedGateEmail(email.ToLower()));
        }

        public void StatusChangedUnsubscribeEmail(string email)
        {
            m_SubscribedContacts.Remove(email.ToLower());
        }

        public event StatusChangedEventHandler StatusChanged;

        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler handler = StatusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private IMessengerContact GetContactByRedGateEmail(string email)
        {
            return m_Messenger.GetContact(email.ToLower(), m_ServiceID);
        }

        private void communicator_OnContactStatusChange(object pMContact, MISTATUS mStatus)
        {
            IMessengerContactAdvanced contact = (IMessengerContactAdvanced)pMContact;

            if (m_SubscribedContacts.ContainsKey(contact.SigninName.ToLower()))
            {
                StatusChangedEventArgs args = new StatusChangedEventArgs();
                args.Email = contact.SigninName.ToLower();
                args.CurrentStatus = MistatusToFoosStatus(contact.Status);
                args.TimeOfChange = DateTime.Now;
                OnStatusChanged(args);
            }
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

        private IMessengerContact GetSelfContact()
        {
            return m_Messenger.GetContact(m_Messenger.MySigninName, m_ServiceID);
        }
    }
}
