using System;
using FoosNet.Network;

namespace FoosNet.CommunicatorIntegration
{
    public class StatusChangedEventArgs : EventArgs
    {
        public string Email { get; set; }
        public Status CurrentStatus { get; set; }
        public DateTime TimeOfChange { get; set; }
    }
}