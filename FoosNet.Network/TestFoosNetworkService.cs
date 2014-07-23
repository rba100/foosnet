using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FoosNet.Network
{
    public class TestFoosNetworkService : IFoosNetworkService
    {
        private readonly Random m_Random = new Random();

        public event Action<ChallengeRequest> ChallengeReceived;
        public event Action<ChallengeResponse> ChallengeResponse;
        public event Action<PlayerDiscoveryMessage> PlayersDiscovered;
        public event Action<GameStartingMessage> GameStarting;

        public TestFoosNetworkService()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                var players = new List<LivePlayer>
                {
                    new LivePlayer("robin.anderson@red-gate.com"),
                    new LivePlayer("reka.burmeister@red-gate.com"),
                    new LivePlayer("jason.crease@red-gate.com"),
                    new LivePlayer("martin.podlubny@red-gate.com"),
                    new LivePlayer("mark.raymond@red-gate.com"),
                    new LivePlayer("oliver.lane@red-gate.com")
                };
                if (PlayersDiscovered != null) PlayersDiscovered(new PlayerDiscoveryMessage(players));
            });
        }

        public void Challenge(IFoosPlayer playerToChallenge)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2000);
                var repsonse = new ChallengeResponse(playerToChallenge, m_Random.Next(1) == 0);
                if (ChallengeResponse != null) ChallengeResponse(repsonse);
            });
        }

        public void Respond(ChallengeResponse response)
        {
            
        }

        public void StartGame(IEnumerable<IFoosPlayer> players)
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}