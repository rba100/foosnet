using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FoosNet.Annotations;
using FoosNet.Network;

namespace FoosNet.Game
{
    public class GameManager : INotifyPropertyChanged
    {
        private const int c_PlayersPerGame = 4;

        private readonly IFoosNetworkService m_NetworkService;
        private readonly ObservableCollection<FoosPlayerListItem> m_PlayerList;

        private readonly List<FoosPlayerListItem> m_PlayerLineUp = new List<FoosPlayerListItem>();
        private string m_SelfEmail;
        private bool m_IsOrganisingGame;

        // Messages
        private const string c_Idle = "Waiting for game";
        private const string c_ReadyToStart = "Ready to start!";
        private const string c_Accepted = "Waiting for game to start...";
        private const string c_Finding = "Finding players...";
        private const string c_FindingFailed = "Not enough players!";
        private const string c_CustomGame = "Creating custom game...";

        private string m_StatusMessage = c_Idle;

        private Thread m_Worker = null;

        public GameManager(IFoosNetworkService networkService, ObservableCollection<FoosPlayerListItem> playerList, string selfEmail)
        {
            m_NetworkService = networkService;
            m_PlayerList = playerList;
            m_SelfEmail = selfEmail;

            m_NetworkService.ChallengeReceived += NetworkServiceOnChallengeReceived;
            m_NetworkService.ChallengeResponse += NetworkServiceOnChallengeResponse;
        }

        private void NetworkServiceOnChallengeResponse(ChallengeResponse challengeResponse)
        {
            var player = m_PlayerLineUp.FirstOrDefault(p => p.Email.Equals(challengeResponse.Player.Email, StringComparison.OrdinalIgnoreCase));
            if (player != null)
            {
                if (player.GameState == GameState.Pending) player.GameState = challengeResponse.Accepted ? GameState.Accepted : GameState.Declined;
                if (player.GameState == GameState.Declined) m_PlayerLineUp.Remove(player);

                if (m_PlayerLineUp.Count == c_PlayersPerGame)
                {
                    if (m_PlayerLineUp.All(p => p.GameState == GameState.Accepted))
                    {
                        BeginGame();
                    }
                }
            }
            else
            {
                // TODO: reply to the user to say 'sorry, you're not in this game'
            }
        }

        private void BeginGame()
        {
            //m_NetworkService.
            Reset();
        }

        private void AddSelf()
        {
            var self = m_PlayerList.FirstOrDefault(p => p.Email == m_SelfEmail);
            if (self != null)
            {
                self.GameState = GameState.Accepted;
                m_PlayerLineUp.Add(self);
            }
            else
            {
                m_PlayerLineUp.Add(new FoosPlayerListItem(m_SelfEmail, Status.Available, 1) { GameState = GameState.Accepted });
            }
        }

        private void NetworkServiceOnChallengeReceived(ChallengeRequest challengeRequest)
        {
            if (m_IsOrganisingGame)
            {
                m_NetworkService.Respond(new ChallengeResponse(challengeRequest.Challenger, false));
            }
        }

        public bool IsGameReadyToStart { get { return m_PlayerLineUp.Count == c_PlayersPerGame; } }
        public bool GameCreationInProgress { get { return m_IsOrganisingGame; } set { m_IsOrganisingGame = value; OnPropertyChanged(); } }

        public bool CanAddPlayer { get { return !IsGameReadyToStart; } }
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
            if (m_PlayerLineUp.Count >= c_PlayersPerGame) throw new InvalidOperationException("Cannot add another player, max players reached");
            m_PlayerLineUp.Add(player);
            player.GameState = GameState.Pending;
        }

        public void Reset()
        {
            StatusMessage = c_Idle;
            m_PlayerLineUp.Clear();
            GameCreationInProgress = false;
            foreach (var item in m_PlayerList)
            {
                item.GameState = GameState.None;
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
            try
            {
                var orderedPlayerList = new List<FoosPlayerListItem>(
                    (prefferedPlayers as List<FoosPlayerListItem>) ?? new List<FoosPlayerListItem>());
                orderedPlayerList.AddRange(m_PlayerList.Where(p => !orderedPlayerList.Contains(p)).ToList());

                foreach (var player in orderedPlayerList)
                {
                    if (player.Status == Status.Available)
                    {
                        InvitePlayer(player);
                    }
                }
            }
            catch (Exception ex)
            {
                if (OnError != null) OnError(this, new ErrorEventArgs(ex));
                Reset();
            }
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
