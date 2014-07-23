using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using FoosNet.Annotations;
using FoosNet.CommunicatorIntegration;
using FoosNet.Controls;
using FoosNet.Controls.Alerts;
using FoosNet.Game;
using FoosNet.Network;
using FoosNet.PlayerFilters;
using FoosNet.Tests;

namespace FoosNet
{
    public class NotifyWindowViewModel : INotifyPropertyChanged, IFoosAlerterProvider
    {
        private ObservableCollection<FoosPlayerListItem> m_FoosPlayers;
        private bool m_IsTableFree;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IFoosNetworkService m_NetworkService;
        private readonly List<IPlayerTransformation> m_PlayerProcessors;
        public GameManager GameManager { get; set; }
        private IFoosAlerter m_Alerter;
        private FoosPlayerListItem m_Self;


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

        private ICommand m_CancelGameCommand;
        public ICommand CancelGameCommand
        {
            get
            {
                return m_CancelGameCommand ?? (m_CancelGameCommand = new SimpleCommand(CancelGame, CanCancelGame));
            }
        }

        private bool CanCancelGame(object arg)
        {
            return true;
        }

        private void CancelGame(object obj)
        {
            
        }


        private ICommand m_StartGameCommand;
        public ICommand StartGameCommand
        {
            get
            {
                return m_StartGameCommand ?? (m_StartGameCommand = new SimpleCommand(StartGame, CanStartGame));
            }
        }

        private ICommand m_CreateGameAutoCommand;

        public ICommand CreateGameAutoCommand
        {
            get
            {
                return m_CreateGameAutoCommand ?? (m_CreateGameAutoCommand = new SimpleCommand(CreateGameAuto, CanCreateGameAuto));
            }
        }

        private bool CanCreateGameAuto(object arg)
        {
            return GameManager.CanCreateGameAuto;
        }

        private void CreateGameAuto(object obj)
        {
            var param = obj as IList;
            var prefferedPlayers = new List<FoosPlayerListItem>();
            foreach (FoosPlayerListItem p in param)
            {
                prefferedPlayers.Add(p);
            }
            GameManager.CreateGameAuto(prefferedPlayers);
        }

        private bool CanStartGame(object arg)
        {
            return GameManager.IsGameReadyToStart;
        }

        private void StartGame(object obj)
        {
            
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
            m_Alerter = new FullScreenFoosAlerter();

            m_PlayerProcessors = new List<IPlayerTransformation>();
            string localEmail = Environment.UserName + "@red-gate.com";
            m_Self = new FoosPlayerListItem(localEmail, Status.Available, 1);

            try
            {
                var communicator = new CommunicatorIntegration.CommunicatorIntegration();
                m_PlayerProcessors.Add(new CommunicatorPlayerFilter(communicator));
                communicator.StatusChanged += CommunicatorOnStatusChanged;
                localEmail = communicator.GetLocalUserEmail();
            }
            catch
            {
                // If Communicator isn't working, process player names as best we can from email address
                m_PlayerProcessors.Add(new DefaultNameTransformation());
                m_PlayerProcessors.Add(new StatusToUnknownTransformation());
            }
            m_NetworkService = new TestFoosNetworkService();
            //m_NetworkService = new FoosNetworkService(endpoint, localEmail);
            m_NetworkService.PlayersDiscovered += NetworkServiceOnPlayersDiscovered;
            m_NetworkService.ChallengeReceived += NetworkServiceOnChallengeReceived;
            m_NetworkService.ChallengeResponse += NetworkServiceOnChallengeResponse;
            //var testObjects = new ShowPlayersTest();
            FoosPlayers = new ObservableCollection<FoosPlayerListItem>();

            GameManager = new GameManager(m_NetworkService, this, m_FoosPlayers, m_Self);
            GameManager.OnError += GameManagerOnOnError;
        }

        private void GameManagerOnOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            MessageBox.Show(errorEventArgs.GetException().ToString(), "Error creating game");
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
                if (player.Email == m_Self.Email) continue;
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

        public IFoosAlerter GetAlerter()
        {
            return m_Alerter;
        }
    }
}
