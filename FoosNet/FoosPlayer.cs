using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FoosNet.Annotations;
using FoosNet.Network;

namespace FoosNet
{
    public enum GameState { None, Pending, Accepted, Declined }

    public class FoosPlayer : IFoosPlayer
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public Status Status { get; set; }
        public int Priority { get; set; }

        public GameState GameState { get; set; }

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

        public FoosPlayer(IFoosPlayer player)
        {
            Email = player.Email;
            DisplayName = player.DisplayName;
            Status = player.Status;
            Priority = 50;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
