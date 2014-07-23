using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FoosNet.Annotations;
using FoosNet.Network;

namespace FoosNet.Game
{
    public class GameManager : INotifyPropertyChanged
    {
        private const int c_PlayersPerGame = 4;
        private readonly List<FoosPlayerListItem> m_MatchingPlayers = new List<FoosPlayerListItem>();

        private readonly IFoosNetworkService m_NetworkService;
        private ObservableCollection<FoosPlayerListItem> m_PlayerList;
        private string m_SelfEmail;
        private bool m_IsOrganisingGame;

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
            var player = m_MatchingPlayers.FirstOrDefault(p => p.Email.Equals(challengeResponse.Player.Email, StringComparison.OrdinalIgnoreCase));
            if (player != null)
            {
                if (player.GameState == GameState.Pending) player.GameState = challengeResponse.Accepted ? GameState.Accepted : GameState.Declined;
                if (player.GameState == GameState.Declined) m_MatchingPlayers.Remove(player);

                if (m_MatchingPlayers.Count == c_PlayersPerGame)
                {
                    if (m_MatchingPlayers.All(p => p.GameState == GameState.Accepted))
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

        private void NetworkServiceOnChallengeReceived(ChallengeRequest challengeRequest)
        {
            if (m_IsOrganisingGame)
            {
                m_NetworkService.Respond(new ChallengeResponse(challengeRequest.Challenger, false));
            }
        }

        public bool IsGameReadyToStart { get { return m_MatchingPlayers.Count == c_PlayersPerGame; } }
        public bool GameCreationInProgress { get { return m_IsOrganisingGame; } set { m_IsOrganisingGame = value;OnPropertyChanged(); }}

        public bool CanAddPlayer { get { return !IsGameReadyToStart; } }
        public bool CanCreateGameAuto { get { return !m_IsOrganisingGame; } }

        public void AddPlayer(FoosPlayerListItem player)
        {
            if (!m_IsOrganisingGame) GameCreationInProgress = true;
            if(m_MatchingPlayers.Count >= c_PlayersPerGame) throw new InvalidOperationException("Cannot add another player, max players reached");
            m_MatchingPlayers.Add(player);
            player.GameState = GameState.Pending;
        }

        public void Reset()
        {
            m_MatchingPlayers.Clear();
            GameCreationInProgress = false;
            foreach (var item in m_PlayerList)
            {
                item.GameState = GameState.None;
            }
        }

        public void CreateGameAuto(List<FoosPlayerListItem> prefferedPlayers = null)
        {
            if(m_IsOrganisingGame) throw new InvalidOperationException("Already creating a game");
            GameCreationInProgress = true;
            var exceptionCatcher = Task.Factory.StartNew(() => { CreateGameAutoInternal(prefferedPlayers); });
        }

        private bool CreateGameAutoInternal(List<FoosPlayerListItem> prefferedPlayers = null)
        {
            var remainingPlayers = new List<FoosPlayerListItem>(m_PlayerList);
            remainingPlayers.RemoveAll(prefferedPlayers.Contains);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
