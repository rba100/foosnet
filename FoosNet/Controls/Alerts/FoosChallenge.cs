using FoosNet.Network;
using TestAlerts;

namespace FoosNet.Controls.Alerts
{
    class FoosChallenge : IFoosChallenge
    {
        public IFoosPlayer Challenger { get; set; }
    }
}