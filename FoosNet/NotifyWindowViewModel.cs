using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FoosNet.Annotations;
using FoosNet.Tests;

namespace FoosNet
{
    public class NotifyWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FoosPlayer> m_FoosPlayers; 
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FoosPlayer> FoosPlayers
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
