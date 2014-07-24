using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FoosNet.Annotations;
using FoosNet.CommunicatorIntegration;
using FoosNet.Controls;
using FoosNet.Controls.Alerts;
using FoosNet.Game;
using FoosNet.Model;
using FoosNet.Network;
using FoosNet.PlayerFilters;
using FoosNet.Views;

namespace FoosNet
{
    public class NotifyWindowViewModel : INotifyPropertyChanged, IFoosAlerterProvider
    {
        private ObservableCollection<FoosPlayerListItem> m_FoosPlayers = new ObservableCollection<FoosPlayerListItem>();
        private bool m_IsTableFree;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IFoosNetworkService m_NetworkService;
        private readonly List<IPlayerTransformation> m_PlayerProcessors;
        public GameManager GameManager { get; set; }
        private readonly FoosPlayerListItem m_Self;
        private CommunicatorIntegration.CommunicatorIntegration m_Communicator;

        public ObservableCollection<FoosPlayerListItem> FoosPlayers
        {
            get { return m_FoosPlayers; }
            set
            {
                m_FoosPlayers = value;
                OnPropertyChanged();
            }
        }
        #region Commands

        //ShowSettingsCommand

        private ICommand m_ShowSettingsCommand;
        public ICommand ShowSettingsCommand
        {
            get
            {
                return m_ShowSettingsCommand ?? (m_ShowSettingsCommand = new SimpleCommand(ShowSettings, a => true));
            }
        }

        private void ShowSettings(object obj)
        {
            if (m_IsShowSettings)
            {
                Settings.Default.UseMinimalAlerts = m_UseMinimalAlerts;
                Settings.Default.Save();
            }
            IsShowSettings = !IsShowSettings;
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
            return list.Count > 0 && GameManager.CanAddPlayer && GameManager.FreeSlots >= list.Count;
        }

        private void ChallengePlayer(object obj)
        {
            var list = (obj as IList);
            if (list == null) return;
            var players = list.Cast<FoosPlayerListItem>().ToList();
            foreach (var p in players)
            {
                if (GameManager.CanAddPlayer) GameManager.InvitePlayer(p);
            }
        }

        private ICommand m_ChatToSelectedPlayerCommand;
        public ICommand ChatToSelectedPlayerCommand
        {
            get
            {
                return m_ChatToSelectedPlayerCommand ?? (m_ChatToSelectedPlayerCommand = new SimpleCommand(ChatToSelectedPlayer, CanChatToSelectedPlayer));
            }
        }

        private bool CanChatToSelectedPlayer(object arg)
        {
            var list = (arg as IList);
            if (list == null) return false;
            var players = list.Cast<FoosPlayerListItem>().ToList();
            return m_Communicator != null && players.All(p => p.Status != Status.Unknown);
        }

        public void ChatToSelectedPlayer(object obj)
        {
            var list = (obj as IList);
            if (list == null) return;
            var players = list.Cast<FoosPlayerListItem>().ToList();
            foreach (var p in players)
            {
                m_Communicator.OpenConversationWithRedgateEmail(p.Email);
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
            GameManager.Reset(true);
        }


        private ICommand m_StartGameCommand;
        public ICommand StartGameCommand
        {
            get
            {
                return m_StartGameCommand ?? (m_StartGameCommand = new SimpleCommand(StartGame, CanStartGame));
            }
        }

        private bool CanStartGame(object arg)
        {
            return GameManager.IsGameReadyToStart;
        }

        private void StartGame(object obj)
        {
            GameManager.BeginGame();
        }

        private ICommand m_CreateGameAutoCommand;
        private bool m_IsShowSettings;
        private bool m_UseMinimalAlerts;


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
        #endregion

        public bool UseMinimalAlerts
        {
            get { return m_UseMinimalAlerts; }
            set
            {
                if (value.Equals(m_UseMinimalAlerts)) return;
                m_UseMinimalAlerts = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowSettings
        {
            get { return m_IsShowSettings; }
            set
            {
                if (value.Equals(m_IsShowSettings)) return;
                m_IsShowSettings = value;
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
            UseMinimalAlerts = Settings.Default.UseMinimalAlerts;

            m_PlayerProcessors = new List<IPlayerTransformation>();
            var localEmail = Environment.UserName + "@" + Environment.UserDomainName + ".com";
            m_Self = new FoosPlayerListItem(localEmail, Status.Available, 1) { DisplayName = "You" };

            try
            {
                m_Communicator = new CommunicatorIntegration.CommunicatorIntegration();
                m_PlayerProcessors.Add(new CommunicatorPlayerFilter(m_Communicator));
                m_Communicator.StatusChanged += CommunicatorOnStatusChanged;
                localEmail = m_Communicator.GetLocalUserEmail();
            }
            catch
            {
                // If Communicator isn't working, process player names as best we can from email address
                m_PlayerProcessors.Add(new DefaultNameTransformation());
                m_PlayerProcessors.Add(new StatusToUnknownTransformation());
                m_Communicator = null;
            }

            // nasty hack to support WPF designer
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                m_NetworkService = new TestFoosNetworkService();
            }
            else
            {
                m_NetworkService = new FoosNetworkService(endpoint, localEmail, TimeSpan.FromMinutes(1));
            }

            m_NetworkService.PlayersDiscovered += NetworkServiceOnPlayersDiscovered;
            m_NetworkService.GameStarting += NetworkServiceOnGameStarting;
            m_NetworkService.TableStatusChanged += NetworkServiceOnTableStatusChanged;

            GameManager = new GameManager(m_NetworkService, this, m_FoosPlayers, m_Self);
            GameManager.PropertyChanged += GameManagerOnPropertyChanged;
            GameManager.OnError += GameManagerOnOnError;
        }

        private void NetworkServiceOnTableStatusChanged(bool free)
        {
            IsTableFree = free;
        }

        private void GameManagerOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            // For some reason the 'start game' button does not re-evaluate it's CanExecute when it should.
            if (args.PropertyName == "IsGameReadyToStart")
            {
                // This method must be called on UI thread to work, or it fails silently.
                Application.Current.Dispatcher.BeginInvoke(new Action(CommandManager.InvalidateRequerySuggested));
            }
        }

        private void NetworkServiceOnGameStarting(GameStartingMessage gameStartingMessage)
        {
            var gogoWindow = new AllPlayersJoined(gameStartingMessage.Players);
            gogoWindow.Show();
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

        private void NetworkServiceOnPlayersDiscovered(PlayerDiscoveryMessage playerDiscoveryMessage)
        {
            var newPlayerList = new List<IFoosPlayer>();
            foreach (var player in playerDiscoveryMessage.Players)
            {
                if (player.Email.Equals(m_Self.Email, StringComparison.InvariantCultureIgnoreCase)) continue;
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
                    newPlayerList.Add(existingPlayer);
                }
                else
                {
                    newPlayerList.Add(transformedPlayer);
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                var toRemove = m_FoosPlayers.Where(p => newPlayerList.All(q => q.Email != p.Email)).ToList();
                foreach (var foosPlayerListItem in toRemove)
                {
                    m_FoosPlayers.Remove(foosPlayerListItem);
                    GameManager.RemovePlayer(foosPlayerListItem);
                }
                foreach (var foosPlayer in newPlayerList.Where(p => m_FoosPlayers.All(q => q.Email != p.Email)))
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
            if (UseMinimalAlerts)
            {
                return new MinimalFoosAlerter();
            }
            return new FullScreenFoosAlerter();
        }
    }
}
