using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FoosNet.Network
{
    public class LivePlayer : IFoosPlayer
    {
        private Status m_Status;
        private string m_Email;
        private string m_DisplayName;

        public string Email
        {
            get { return m_Email; }
            set { m_Email = value; OnPropertyChanged(); }
        }

        public string DisplayName
        {
            get { return m_DisplayName; }
            set { m_DisplayName = value; OnPropertyChanged(); }
        }

        public Status Status
        {
            get { return m_Status; }
            set { m_Status = value; OnPropertyChanged(); }
        }

        public LivePlayer(string email)
        {
            m_Email = email;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
