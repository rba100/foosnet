using System;
using System.Threading.Tasks;
using FoosNet.Network;

namespace FoosNet
{
    public class FoosPlayer : IFoosPlayer
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public Status Status { get; set; }
        public int Priority { get; set; }

        public FoosPlayer(string email, Status status, int priority)
        {
            Email = email;
            DisplayName = email;
            Status = status;
            if (priority < 1 || priority > 99)
            {
                throw new Exception("Player priority invalid, has to be between 1 and 99");
            }

            Priority = priority;
        }
    }
}
