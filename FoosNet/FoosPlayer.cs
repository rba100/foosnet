using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoosNet.Network;

namespace FoosNet
{
    public class FoosPlayer : IFoosPlayer
    {
        public string Name { get; private set; }
        public Status Status { get; private set; }

        public FoosPlayer(string name, Status status)
        {
            Name = name;
            Status = status;
        }

        public Task<ChallengeResponse> ChallengePlayer()
        {
            throw new NotImplementedException();
        }
    }
}
