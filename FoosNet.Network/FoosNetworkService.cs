using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        public event Action<ChallengeRequest> ChallengeReceived;
        public event Action<ChallengeResponse> ChallengeResponse;
        public event Action<PlayerDiscoveryMessage> PlayersDiscovered;

        public FoosNetworkService()
        {
            Task.Factory.StartNew(() =>
            {
                var players = new List<IFoosPlayer>();
                players.Add(new LivePlayer("robin.anderson@red-gate.com"));
                players.Add(new LivePlayer("reka.burmeister@red-gate.com"));
                players.Add(new LivePlayer("martin.podlubny@red-gate.com"));
                players.Add(new LivePlayer("mark.raymond@red-gate.com"));
                players.Add(new LivePlayer("jason.crease@red-gate.com"));
                players.Add(new LivePlayer("oliver.lane@red-gate.com"));
                Thread.Sleep(2000);
                PlayersDiscovered(new PlayerDiscoveryMessage() { Players = players });
            });
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
