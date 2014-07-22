using System;
using System.Collections;
using System.Collections.Generic;

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
