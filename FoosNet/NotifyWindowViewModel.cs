using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FoosNet.Annotations;
using FoosNet.Tests;

namespace FoosNet
{
    public class NotifyWindowViewModel : INotifyPropertyChanged
    {
        private IEnumerable<FoosPlayer> m_FoosPlayers; 
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<FoosPlayer> FoosPlayers
        {
            get { return m_FoosPlayers; }
            set
            {
                m_FoosPlayers = value;
                OnPropertyChanged();
            }
        }

        public NotifyWindowViewModel()
        {
            ShowPlayersTest testObjects = new ShowPlayersTest();
            FoosPlayers = testObjects.GetPlayers();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
