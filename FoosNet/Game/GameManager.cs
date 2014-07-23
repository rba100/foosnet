using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FoosNet.Annotations;
using FoosNet.Controls.Alerts;
using FoosNet.Network;

namespace FoosNet.Game
{
    public class GameManager : INotifyPropertyChanged
    {
        private const int c_PlayersPerGame = 4;

        private readonly IFoosNetworkService m_NetworkService;
        private readonly IFoosAlerterProvider m_Alerter;
        private readonly ObservableCollection<FoosPlayerListItem> m_PlayerList;

        private readonly List<FoosPlayerListItem> m_PlayerLineUp = new List<FoosPlayerListItem>();
        private readonly FoosPlayerListItem m_Self;
        private bool m_IsOrganisingGame;
        private bool m_IsJoiningRemoteGame;
        private CancellationTokenSource m_Cts = new CancellationTokenSource();
        private CancellationToken CancellationToken { get { return m_Cts.Token; } }

        // Messages
        private const string c_Idle = "Waiting for game";
        private const string c_ReadyToStart = "Ready to start!";
        private const string c_Accepted = "Waiting for game start...";
        private const string c_Finding = "Finding players...";
        private const string c_FindingFailed = "Not enough players!";
        private const string c_CustomGame = "Creating custom game...";

        private string m_StatusMessage = c_Idle;

        private Thread m_Worker = null;

        public GameManager(IFoosNetworkService networkService, IFoosAlerterProvider alerter, ObservableCollection<FoosPlayerListItem> playerList, FoosPlayerListItem self)
        {
            m_NetworkService = networkService;
            m_Alerter = alerter;
            m_PlayerList = playerList;
            m_Self = self;

            m_NetworkService.ChallengeReceived += NetworkServiceOnChallengeReceived;
            m_NetworkService.ChallengeResponse += NetworkServiceOnChallengeResponse;
        }

        private void NetworkServiceOnChallengeResponse(ChallengeResponse challengeResponse)
        {
            var player = m_PlayerLineUp.FirstOrDefault(p => p.Email.Equals(challengeResponse.Player.Email, StringComparison.OrdinalIgnoreCase));
            if (player != null)
            {
                player.GameState = challengeResponse.Accepted ? GameState.Accepted : GameState.Declined;
                if (player.GameState == GameState.Declined) m_PlayerLineUp.Remove(player);

                if (IsGameReadyToStart)
                {
                    StatusMessage = c_ReadyToStart;
                    // This property is not automatically updated because it's a computed value
                    OnPropertyChanged("IsGameReadyToStart");
                }
            }
            else
            {
                if (challengeResponse.Accepted)
                {
                    // TODO: reply to the user to say 'sorry, you're not in this game'
                    //m_NetworkService.UnChallenge(challengeResponse.Player);
                }
            }
        }

        public void BeginGame()
        {
            if (!IsGameReadyToStart) throw new InvalidOperationException("Need four accepted players to start a game.");
            m_NetworkService.StartGame(m_PlayerLineUp.ToArray());
            Reset();
        }

        private void AddSelf()
        {
            m_Self.GameState = GameState.Accepted;
            m_PlayerLineUp.Add(m_Self);
        }

        private void NetworkServiceOnChallengeReceived(ChallengeRequest challengeRequest)
        {
            if (m_IsOrganisingGame || m_IsJoiningRemoteGame)
            {
                m_NetworkService.Respond(new ChallengeResponse(challengeRequest.Challenger, false));
            }
            else
            {
                IsJoiningRemoteGame = true;
                OnPropertyChanged("CanCreateGameAuto");
                OnPropertyChanged("CanAddPlayer");
                StatusMessage = c_Accepted;

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var alert = m_Alerter.GetAlerter();
                    alert.ShowChallengeAlert(challengeRequest);
                }));

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1500);
                    m_NetworkService.Respond(new ChallengeResponse(challengeRequest.Challenger, true));
                });

                //Action<bool> lam = (accepted) =>
                //{
                //    try
                //    {
                //        if (IsJoiningRemoteGame || m_IsOrganisingGame) return;
                //        m_NetworkService.Respond(new ChallengeResponse(m_Self, accepted));
                //        if (accepted)
                //        {
                //            IsJoiningRemoteGame = true;
                //            OnPropertyChanged("CanCreateGameAuto");
                //            OnPropertyChanged("CanAddPlayer");
                //            StatusMessage = c_Accepted;
                //        }
                //    }
                //    catch { }
                //};
            }
        }

        public bool IsGameReadyToStart
        {
            get
            {
                lock (m_PlayerLineUp) return m_PlayerLineUp.Count(p => p.GameState == GameState.Accepted) == c_PlayersPerGame;
            }
        }

        public bool GameCreationInProgress { get { return m_IsOrganisingGame; } set { m_IsOrganisingGame = value; OnPropertyChanged(); } }
        public bool IsJoiningRemoteGame { get { return m_IsJoiningRemoteGame; } set { m_IsJoiningRemoteGame = value; OnPropertyChanged(); } }
        public bool CanAddPlayer { get { return !IsGameReadyToStart; } }
        public int FreeSlots { get { return Math.Min(c_PlayersPerGame - m_PlayerLineUp.Count, 3); } }
        public bool CanCreateGameAuto { get { return !m_IsOrganisingGame; } }

        public string StatusMessage
        {
            get { return m_StatusMessage; }
            set
            {
                if (value == m_StatusMessage) return;
                m_StatusMessage = value;
                OnPropertyChanged();
            }
        }

        public void InvitePlayer(FoosPlayerListItem player)
        {
            if (!m_IsOrganisingGame)
            {
                GameCreationInProgress = true;
                StatusMessage = c_CustomGame;
                AddSelf();
            }
            lock (m_PlayerLineUp)
            {
                if (m_PlayerLineUp.Count >= c_PlayersPerGame)
                    throw new InvalidOperationException("Cannot add another player, max players reached");
                player.GameState = GameState.Pending;
                m_PlayerLineUp.Add(player);
            }
            m_NetworkService.Challenge(player);
            Task.Factory.StartNew(() => PlayerTimeOutWatcher(player, CancellationToken));
        }

        public void Reset()
        {
            lock (m_PlayerLineUp)
            {
                StatusMessage = c_Idle;
                m_Self.GameState = GameState.None;
                foreach (var player in m_PlayerLineUp)
                {
                    if (!player.Email.Equals(m_Self.Email, StringComparison.OrdinalIgnoreCase) &&
                        player.GameState == GameState.Accepted)
                    {
                        //TODO: m_NetworkService.UnChallenge(player);
                    }
                }
                m_PlayerLineUp.Clear();
                GameCreationInProgress = false;
                m_IsJoiningRemoteGame = false;
                m_Cts.Cancel();

                foreach (var item in m_PlayerList)
                {
                    item.GameState = GameState.None;
                }

                m_Cts.Dispose();
                m_Cts = new CancellationTokenSource();
            }
        }

        public void CreateGameAuto(List<FoosPlayerListItem> prefferedPlayers = null)
        {
            if (m_IsOrganisingGame) throw new InvalidOperationException("Already creating a game");
            GameCreationInProgress = true;
            AddSelf();
            StatusMessage = c_Finding;
            m_Worker = new Thread(CreateGameAutoWorker);
            m_Worker.Start(prefferedPlayers);
        }

        private void CreateGameAutoWorker(object prefferedPlayers)
        {
            var token = CancellationToken;
            try
            {
                var orderedPlayerList = new List<FoosPlayerListItem>(
                    (prefferedPlayers as List<FoosPlayerListItem>) ?? new List<FoosPlayerListItem>());
                orderedPlayerList.AddRange(m_PlayerList.Where(p => !orderedPlayerList.Contains(p)).ToList());

                foreach (var player in orderedPlayerList)
                {
                    if (!PlayableStatus(player)) continue;

                    while (!IsGameReadyToStart && m_PlayerLineUp.Count == c_PlayersPerGame)
                    {
                        Task.Delay(TimeSpan.FromSeconds(1), token).Wait(token);
                        if (token.IsCancellationRequested) return;
                    }

                    if (IsGameReadyToStart) break;
                    InvitePlayer(player);
                }

                if (!IsGameReadyToStart) StatusMessage = c_FindingFailed;
            }
            catch (Exception ex)
            {
                if (OnError != null) OnError(this, new ErrorEventArgs(ex));
                Reset();
            }
        }

        private void PlayerTimeOutWatcher(object playerObj, CancellationToken token)
        {
            var player = playerObj as FoosPlayerListItem;
            var marker = ++player.InviteMarker;
            Task.Delay(TimeSpan.FromSeconds(20), token).Wait();
            if (token.IsCancellationRequested || player.InviteMarker != marker) return;
            if (player.GameState == GameState.Pending)
            {
                player.GameState = GameState.Timeout;
                lock (m_PlayerLineUp) m_PlayerLineUp.RemoveAll(p => p.Email.Equals(player.Email));
            }
        }

        private bool PlayableStatus(IFoosPlayer player)
        {
            return player.Status == Status.Available || player.Status == Status.Unknown;
        }

        public event ErrorEventHandler OnError;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
