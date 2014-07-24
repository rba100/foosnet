using System;
using FoosNet.Network;

namespace FoosNet.Controls.Alerts
{
    public enum AlertType { Full, Minimal }

    /// <summary>
    /// Interface for the alerter which pops up on a player's screen when 
    /// somebody challenges them. Only one challenge can be active at a time.
    /// </summary>
    public interface IFoosAlerter
    {
        // If an alert is already active, calling this method again
        // will close the previous alert and open a new one
        void ShowChallengeAlert(IFoosPlayer challenger);
        event Action<IFoosPlayer, bool> ChallengeResponseReceived;

        // Has no effect if no alert is currently active
        void CancelChallengeAlert();

        // Has no effect if no alert is currently active
        void CloseChallengeAlert();
    }
}
