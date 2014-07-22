using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoosNet.Network;

namespace FoosNet.Services
{
    public class FoosNetworkService
    {
        public delegate void ChallengeReceivedHandler(IFoosPlayer player);
        event ChallengeReceivedHandler ChallengeReceived;

        public delegate void ChallengeResponseHandler(IFoosPlayer player);
        event ChallengeResponseHandler ChallengeResponse;
    }
}
