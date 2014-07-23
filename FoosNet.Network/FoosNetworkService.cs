﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Helpers;
using WebSocket4Net;

namespace FoosNet.Network
{
    public interface IFoosNetworkService : IDisposable
    {
        event Action<ChallengeRequest> ChallengeReceived;
        event Action<ChallengeResponse> ChallengeResponse;
        event Action<PlayerDiscoveryMessage> PlayersDiscovered;
        event Action<GameStartingMessage> GameStarting;
        void Challenge(IFoosPlayer playerToChallenge);
        void Respond(ChallengeResponse response);
    }

    public class FoosNetworkService : IFoosNetworkService
    {
        private readonly string m_Email;
        private readonly WebSocket m_WebSocket;
        private Timer m_Timer;

        public event Action<ChallengeRequest> ChallengeReceived;
        public event Action<ChallengeResponse> ChallengeResponse;
        public event Action<PlayerDiscoveryMessage> PlayersDiscovered;
        public event Action<GameStartingMessage> GameStarting;

        public void Challenge(IFoosPlayer playerToChallenge)
        {
            SendJson(new { action = "challenge", toChallenge = playerToChallenge.Email });
        }

        public void Respond(ChallengeResponse response)
        {
            SendJson(new { action = "respond", challenger = response.Player.Email, response = response.Accepted.ToString() });
        }

        public FoosNetworkService(string endpoint, string email)
        {
            m_Email = email;
            m_WebSocket = new WebSocket(endpoint);
            m_WebSocket.Opened += OnOpen;
            m_WebSocket.MessageReceived += OnMessageReceived;
            m_WebSocket.Error += (sender, args) => Console.WriteLine("Error!{0}{1}", Environment.NewLine, args.Exception);
            m_WebSocket.Open();
        }

        void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var m = Json.Decode(e.Message);
            var type = m.type as string;
            switch (type)
            {
                case "challenge":
                    if (ChallengeReceived != null) ChallengeReceived(new ChallengeRequest(new LivePlayer(m.challengedBy as string)));
                    break;
                case "response":
                    if (ChallengeResponse != null) ChallengeResponse(new ChallengeResponse(new LivePlayer(m.player as string), bool.Parse(m.response)));
                    break;
                case "player":
                    if (PlayersDiscovered != null) PlayersDiscovered(new PlayerDiscoveryMessage(new LivePlayer(m.player as string)));
                    break;
                case "players":
                    if (PlayersDiscovered != null) PlayersDiscovered(new PlayerDiscoveryMessage(((string[]) m.players).Select(p => new LivePlayer(p))));
                    break;
                case "gametime":
                    if (GameStarting != null) GameStarting(new GameStartingMessage(((string[])m.players).Select(p => new LivePlayer(p))));
                    break;
            }
        }

        private void OnOpen(object sender, EventArgs e)
        {
            var subscribe = new { action = "subscribe", email = m_Email };
            m_Timer = new Timer(SendJson, subscribe, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void SendJson(object message)
        {
            var json = Json.Encode(message);
            m_WebSocket.Send(json);
        }

        public void Dispose()
        {
            try
            {
                m_Timer.Dispose();
                m_WebSocket.Close();
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
