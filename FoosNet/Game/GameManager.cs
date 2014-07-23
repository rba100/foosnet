using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FoosNet.Network;

namespace FoosNet.Game
{
    public class GameManager
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
                if (player.GameState == GameState.Pending)
                    player.GameState = challengeResponse.Accepted ? GameState.Accepted : GameState.Declined;
            }
            else
            {
                // TODO: reply to the user to say 'sorry, you're not in this game'
            }
        }

        private void NetworkServiceOnChallengeReceived(ChallengeRequest challengeRequest)
        {
            if (m_IsOrganisingGame)
            {
                //TODO: m_NetworkService.RejectGame(challengeRequest.Challenger, "Already organising a game");
            }
        }

        public bool IsGameReadyToStart
        {
            get { return m_MatchingPlayers.Count == c_PlayersPerGame; }
        }

        public bool CanAddPlayer { get { return !IsGameReadyToStart; } }
        public bool CanCreateGameAuto { get { return !m_IsOrganisingGame; } }

        public void AddPlayer(FoosPlayerListItem player)
        {
            if(m_MatchingPlayers.Count >= c_PlayersPerGame) throw new InvalidOperationException("Cannot add another player, max players reached");
            m_MatchingPlayers.Add(player);
            player.GameState = GameState.Pending;
        }

        public void Reset()
        {
            m_MatchingPlayers.Clear();
            m_IsOrganisingGame = false;
            foreach (var item in m_PlayerList)
            {
                item.GameState = GameState.None;
            }
        }

        public void CreateGameAuto(List<FoosPlayerListItem> prefferedPlayers = null)
        {
            if(m_IsOrganisingGame) throw new InvalidOperationException("Already creating a game");
            m_IsOrganisingGame = true;
            var exceptionCatcher = Task.Factory.StartNew(() => { CreateGameAutoInternal(prefferedPlayers); });
        }

        private bool CreateGameAutoInternal(List<FoosPlayerListItem> prefferedPlayers = null)
        {
            var remainingPlayers = new List<FoosPlayerListItem>(m_PlayerList);
            remainingPlayers.RemoveAll(prefferedPlayers.Contains);
            return true;
        }
    }
}
