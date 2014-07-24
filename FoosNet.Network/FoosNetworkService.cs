using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using FoosNet.Utils;

namespace FoosNet.Network
{
    public interface IFoosNetworkService : IDisposable
    {
        event Action<ChallengeRequest> ChallengeReceived;
        event Action CancelGameReceived;
        event Action<ChallengeResponse> ChallengeResponse;
        event Action<PlayerDiscoveryMessage> PlayersDiscovered;
        event Action<GameStartingMessage> GameStarting;
        event Action<bool> TableStatusChanged;
        void Challenge(IFoosPlayer playerToChallenge);
        void Respond(ChallengeResponse response);
        void StartGame(IEnumerable<IFoosPlayer> players);
        void CancelGame(IEnumerable<IFoosPlayer> players);
    }

    public class FoosNetworkService : Disposable, IFoosNetworkService
    {
        private readonly string m_Email;
        private readonly ReconnectingWebSocket m_WebSocket;
        private Timer m_Timer;
        private readonly TimeSpan m_SubscribeInterval;

        public event Action<ChallengeRequest> ChallengeReceived;
        public event Action CancelGameReceived;
        public event Action<ChallengeResponse> ChallengeResponse;
        public event Action<PlayerDiscoveryMessage> PlayersDiscovered;
        public event Action<GameStartingMessage> GameStarting;
        public event Action<bool> TableStatusChanged;

        public void Challenge(IFoosPlayer playerToChallenge)
        {
            m_WebSocket.SendAsJson(new { action = "challenge", toChallenge = playerToChallenge.Email });
        }

        public void Respond(ChallengeResponse response)
        {
            m_WebSocket.SendAsJson(new { action = "respond", challenger = response.Player.Email, response = response.Accepted.ToString() });
        }

        public void StartGame(IEnumerable<IFoosPlayer> players)
        {
            m_WebSocket.SendAsJson(new { action = "gametime", players = players.Select(p => p.Email).ToArray() });
        }

        public void RequestPlayers()
        {
            m_WebSocket.SendAsJson(new { action = "players" });
        }

        public void CancelGame(IEnumerable<IFoosPlayer> players)
        {
            m_WebSocket.SendAsJson(new { action = "cancelgame", players = players.Select(p => p.Email).ToArray() });
        }

        public FoosNetworkService(string endpoint, string email, TimeSpan subscribeInterval)
        {
            m_Email = email;
            m_WebSocket = new ReconnectingWebSocket(endpoint);
            m_WebSocket.Connect += OnConnect;
            m_WebSocket.MessageReceived += OnMessageReceived;
            m_WebSocket.Error += (exception) => Console.WriteLine("Error!{0}{1}", Environment.NewLine, exception);
            m_WebSocket.Open();
            m_SubscribeInterval = subscribeInterval;
        }

        void OnMessageReceived(string message)
        {
            var m = Json.Decode(message);
            var type = m.type as string;
            switch (type)
            {
                case "cancelgame":
                    if (CancelGameReceived != null) CancelGameReceived();
                    break;
                case "challenge":
                    if (ChallengeReceived != null) ChallengeReceived(new ChallengeRequest(new LivePlayer(m.challengedBy as string)));
                    break;
                case "response":
                    if (ChallengeResponse != null) ChallengeResponse(new ChallengeResponse(new LivePlayer(m.player as string), bool.Parse(m.response)));
                    break;
                case "players":
                    if (PlayersDiscovered != null) PlayersDiscovered(new PlayerDiscoveryMessage(((DynamicJsonArray) m.players).Cast<string>().Select(p => new LivePlayer(p))));
                    break;
                case "gametime":
                    if (GameStarting != null) GameStarting(new GameStartingMessage(((DynamicJsonArray)m.players).Select(p => new LivePlayer((string)p))));
                    break;
                case "tablestatus":
                    if (TableStatusChanged != null) TableStatusChanged(m.tablestatus == "Free");
                    break;
            }
        }

        private void OnConnect()
        {
            var subscribe = new { action = "subscribe", email = m_Email };
            m_Timer = new Timer(m_WebSocket.SendAsJson, subscribe, TimeSpan.Zero, m_SubscribeInterval);
            Task.Factory.StartNew(RequestPlayers);
            Task.Factory.StartNew(RequestTableStatus);
        }

        private void RequestTableStatus()
        {
            m_WebSocket.SendAsJson(new { action = "tablestatus" });
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                m_Timer.Dispose();
                m_WebSocket.Dispose();
            }
            catch
            {
            }
        }
    }

    public class ChallengeRequest
    {
        private readonly IFoosPlayer m_Challenger;

        public ChallengeRequest(IFoosPlayer challenger)
        {
            m_Challenger = challenger;
        }

        public IFoosPlayer Challenger { get { return m_Challenger; } }
    }

    public class CancelGameMessage
    {
        private readonly IEnumerable<IFoosPlayer> m_Challenger;

        public CancelGameMessage(IEnumerable<IFoosPlayer> challenger)
        {
            m_Challenger = challenger;
        }

        public IEnumerable<IFoosPlayer> Challenger { get { return m_Challenger; } }
    }

    public class PlayerDiscoveryMessage
    {
        private readonly IEnumerable<IFoosPlayer> m_Players;

        public PlayerDiscoveryMessage(IFoosPlayer player)
        {
            m_Players = new[] { player };
        }

        public PlayerDiscoveryMessage(IEnumerable<IFoosPlayer> players)
        {
            m_Players = players;
        }

        public IEnumerable<IFoosPlayer> Players { get { return m_Players; } }
    }

    public class GameStartingMessage
    {
        private readonly IEnumerable<IFoosPlayer> m_Players;

        public GameStartingMessage(IEnumerable<IFoosPlayer> players)
        {
            m_Players = players;
        }

        public IEnumerable<IFoosPlayer> Players { get { return m_Players; } }
    }
}
