using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FoosNet.Annotations;
using FoosNet.Network;

namespace FoosNet.Model
{
    public enum GameState { None, Pending, Accepted, Declined, Timeout }

    public class FoosPlayerListItem : IFoosPlayer
    {
        private Status m_Status;
        private string m_DisplayName;
        private GameState m_GameState;
        public string Email { get; set; }
        public volatile int InviteMarker = 0;

        public string DisplayName
        {
            get { return m_DisplayName; }
            set
            {
                if (value == m_DisplayName) return;
                m_DisplayName = value;
                OnPropertyChanged();
            }
        }

        public Status Status
        {
            get { return m_Status; }
            set
            {
                if (value == m_Status) return;
                m_Status = value;
                OnPropertyChanged();
            }
        }

        public int Priority { get; set; }

        public GameState GameState
        {
            get { return m_GameState; }
            set
            {
                if (value == m_GameState) return;
                m_GameState = value;
                OnPropertyChanged();
            }
        }

        public FoosPlayerListItem(string email, Status status, int priority)
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

        public FoosPlayerListItem(IFoosPlayer player)
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
