using System;
using System.Threading.Tasks;
using FoosNet.Network;

namespace FoosNet
{
    public class FoosPlayer : IFoosPlayer
    {
        public string Name { get; private set; }
        public Status Status { get; private set; }
        public int Priority { get; set; }

        public FoosPlayer(string name, Status status, int priority)
        {
            Name = name;
            Status = status;
            if (priority < 1 || priority > 99)
            {
                throw new Exception("Player priority invalid, has to be between 1 and 99");
            }

            Priority = priority;
        }

        public Task<ChallengeResponse> ChallengePlayer()
        {
            throw new NotImplementedException();
        }
    }
}
