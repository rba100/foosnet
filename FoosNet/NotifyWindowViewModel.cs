using System;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using FoosNet.Annotations;
using FoosNet.Network;
using FoosNet.Tests;

namespace FoosNet
{
    public class NotifyWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FoosPlayer> m_FoosPlayers;
        private bool m_IsTableFree;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly FoosNetworkService m_NetworkService;
        private List<IPlayerTransformation> m_PlayerProcessors;

        public ObservableCollection<FoosPlayer> FoosPlayers
        {
            get { return m_FoosPlayers; }
            set
            {
                m_FoosPlayers = value;
                m_IsTableFree = true;
                OnPropertyChanged();
            }
        }

        public bool IsTableFree
        {
            get
            {
                return m_IsTableFree;
            }
            set
            {
                m_IsTableFree = value;
                OnPropertyChanged();
            }
        }

        public NotifyWindowViewModel()
        {
            var endpoint = ConfigurationManager.AppSettings["networkServiceEndpoint"];
            
            m_PlayerProcessors = new List<IPlayerTransformation>();
            m_NetworkService = new FoosNetworkService(); // TODO: FoosNetworkService(endpoint);
            m_NetworkService.PlayersDiscovered += NetworkServiceOnPlayersDiscovered;
            m_NetworkService.ChallengeReceived += NetworkServiceOnChallengeReceived;
            m_NetworkService.ChallengeResponse += NetworkServiceOnChallengeResponse;
            var testObjects = new ShowPlayersTest();
            FoosPlayers = testObjects.GetPlayers();
        }

        private void NetworkServiceOnChallengeResponse(ChallengeResponse challengeResponse)
        {
            
        }

        private void NetworkServiceOnChallengeReceived(ChallengeRequest challengeRequest)
        {
            
        }

        private void NetworkServiceOnPlayersDiscovered(PlayerDiscoveryMessage playerDiscoveryMessage)
        {
            var newPlayers = new List<IFoosPlayer>();
            foreach (var player in playerDiscoveryMessage.Players)
            {
                var transformedPlayer = player;
                foreach (var playerTransformation in m_PlayerProcessors)
                {
                    transformedPlayer = playerTransformation.Process(transformedPlayer);
                }
                newPlayers.Add(transformedPlayer);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
