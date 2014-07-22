
using System;
using FoosNet.Network;

namespace TestAlerts
{
    /// <summary>
    /// Interface for the alerter which pops up on a player's screen when 
    /// somebody challenges them. Only one challenge can be active at a time.
    /// </summary>
    public interface IFoosAlerter
    {
        // If an alert is already active, calling this method again
        // will close the previous alert and open a new one
        void ShowChallengeAlert(IFoosChallenge foosChallenge);

        // Has no effect if no alert is currently active
        void CancelChallengeAlert();

        // Has no effect if no alert is currently active
        void CloseChallengeAlert();
    }

    public interface IFoosChallenge
    {
        IFoosPlayer Challenger {get;}
    }
}
