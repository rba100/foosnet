using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Web.Helpers;
using WebSocket = WebSocket4Net.WebSocket;

namespace FoosNet.Network
{
    public interface IFoosNetworkService
    {
        event Action<ChallengeRequest> ChallengeReceived;
        event Action<ChallengeResponse> ChallengeResponse;
        event Action<PlayerDiscoveryMessage> PlayersDiscovered;
    }

    public class FoosNetworkService : IFoosNetworkService
    {
        private readonly string m_Email;
        private readonly WebSocket m_WebSocket;
        private Timer m_Timer;

        public event Action<ChallengeRequest> ChallengeReceived;
        public event Action<ChallengeResponse> ChallengeResponse;
        public event Action<PlayerDiscoveryMessage> PlayersDiscovered;

        public FoosNetworkService(string endpoint, string email)
        {
            m_Email = email;
            m_WebSocket = new WebSocket(endpoint);
            m_WebSocket.Opened += OnOpen;
            m_WebSocket.Error += (sender, args) => Console.WriteLine("Error!{0}{1}", Environment.NewLine, args.Exception);
            m_WebSocket.Open();
        }

        private void OnOpen(object sender, EventArgs e)
        {
            var subscribe = new { action = "subscribe", email = m_Email };
            m_Timer = new Timer(SendJson, subscribe, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void SendJson(object message)
        {
            m_WebSocket.Send(Json.Encode(message));
        }
    }

    public class ChallengeRequest
    {
        public IFoosPlayer Challenger { get; set; }
    }

    public class PlayerDiscoveryMessage
    {
        public IEnumerable<IFoosPlayer> Players { get; set; }
    }
}
