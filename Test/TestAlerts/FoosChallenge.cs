using FoosNet.Network;

namespace TestAlerts
{
    class FoosChallenge : IFoosChallenge
    {
        public IFoosPlayer Challenger { get; set; }
    }
}