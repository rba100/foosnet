﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using FoosNet.Annotations;
using FoosNet.CommunicatorIntegration;
using FoosNet.Controls;
using FoosNet.Network;
using FoosNet.Tests;

namespace FoosNet
{
    public class NotifyWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FoosPlayerListItem> m_FoosPlayers;
        private bool m_IsTableFree;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly FoosNetworkService m_NetworkService;
        private List<IPlayerTransformation> m_PlayerProcessors;

        public ObservableCollection<FoosPlayerListItem> FoosPlayers
        {
            get { return m_FoosPlayers; }
            set
            {
                m_FoosPlayers = value;
                OnPropertyChanged();
            }
        }

        private ICommand m_ChallengeSelectedPlayer;

        public ICommand ChallengeSelectedPlayer
        {
            get
            {
                return m_ChallengeSelectedPlayer ?? (m_ChallengeSelectedPlayer = new SimpleCommand(ChallengePlayer, CanChallengePlayer));
            }
        }

        private bool CanChallengePlayer(object arg)
        {
            var list = (arg as IList);
            if (list == null) return false;
            return list.Count > 0 && list.Count < 4;
        }

        private void ChallengePlayer(object obj)
        {
            var list = (obj as IList);
            if (list == null) return;
            var players = list.Cast<FoosPlayerListItem>();
            MessageBox.Show(String.Join(", ", players.Select(p=>p.DisplayName)));
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
            var communicator = new CommunicatorIntegration.CommunicatorIntegration();
            m_PlayerProcessors = new List<IPlayerTransformation>();
            m_PlayerProcessors.Add(new CommunicatorPlayerFilter(communicator));
            communicator.StatusChanged += CommunicatorOnStatusChanged;
            m_NetworkService = new FoosNetworkService(endpoint, communicator.GetLocalUserEmail());
            m_NetworkService.PlayersDiscovered += NetworkServiceOnPlayersDiscovered;
            m_NetworkService.ChallengeReceived += NetworkServiceOnChallengeReceived;
            m_NetworkService.ChallengeResponse += NetworkServiceOnChallengeResponse;
            //var testObjects = new ShowPlayersTest();
            FoosPlayers = new ObservableCollection<FoosPlayerListItem>();

        }

        private void CommunicatorOnStatusChanged(object sender, StatusChangedEventArgs statusChangedEventArgs)
        {
            var player = m_FoosPlayers.FirstOrDefault(p => p.Email == statusChangedEventArgs.Email);
            if (player != null) player.Status = statusChangedEventArgs.CurrentStatus;
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
                
                var existingPlayer = m_FoosPlayers.FirstOrDefault(p => p.Email == transformedPlayer.Email);
                if (existingPlayer != null)
                {
                    existingPlayer.DisplayName = transformedPlayer.DisplayName;
                    existingPlayer.Status = transformedPlayer.Status;
                }
                else
                {
                    newPlayers.Add(transformedPlayer);
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var foosPlayer in newPlayers)
                {
                    m_FoosPlayers.Add(new FoosPlayerListItem(foosPlayer));
                }
            });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
