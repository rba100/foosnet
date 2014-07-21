
using System;
using FoosNet.Network;

namespace TestAlerts
{
    /// <summary>
    /// Interface for the alerter which pops up on a player's screen when 
    /// someone challenges them. Only one challenge can be active at a time.
    /// </summary>
    public interface IFoosAlerter
    {
        void AlertChallenge(IFoosPlayer challengingPlayer);
        void CancelChallenge();
        void CloseAlerts();
    }
}
