using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FoosNet.Network.Annotations;

namespace FoosNet.Network
{
    public interface IFoosNetworkService
    {
        event Action<ChallengeRequest> ChallengeReceived;
        event Action<ChallengeResponse> ChallengeResponse;
        event Action<PlayerDiscoveryMessage> PlayersDiscovered;
    }

    public class FoosNetworkService : IFoosNetworkService
    {
        public event Action<ChallengeRequest> ChallengeReceived;
        public event Action<ChallengeResponse> ChallengeResponse;
        public event Action<PlayerDiscoveryMessage> PlayersDiscovered;
    }

    public class LivePlayer : IFoosPlayer
    {
        private Status m_Status;
        private string m_Email;
        private string m_DisplayName;

        public string Email
        {
            get { return m_Email; }
            set { m_Email = value; OnPropertyChanged(); }
        }

        public string DisplayName
        {
            get { return m_DisplayName; }
            set { m_DisplayName = value; OnPropertyChanged(); }
        }

        public Status Status
        {
            get { return m_Status; }
            set { m_Status = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ChallengeRequest
    {
        public IFoosPlayer Challenger { get; set; }
    }

    public class PlayerDiscoveryMessage
    {
        public IEnumerable<IFoosPlayer> Players { get; set; }
    }
}
