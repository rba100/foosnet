using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoosNet.Network;

namespace FoosNet
{
    class FoosPlayer : IFoosPlayer
    {
        public string Name { get; private set; }
        public Status Status { get; private set; }
        public Task<ChallengeResponse> ChallengePlayer()
        {
            throw new NotImplementedException();
        }
    }
}
