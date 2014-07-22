using System.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Tokens;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using FoosNet.Annotations;
using FoosNet.Network;
using FoosNet.Tests;

namespace FoosNet
{
    public class NotifyWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FoosPlayer> m_FoosPlayers;
        private bool m_IsTableFree;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly FoosNetworkService m_NetworkService;

        public ObservableCollection<FoosPlayer> FoosPlayers
        {
            get { return m_FoosPlayers; }
            set
            {
                m_FoosPlayers = value;
                m_IsTableFree = true;
                OnPropertyChanged();
            }
        }

        public bool IsTableFree
        {
            get
            {
                return m_IsTableFree;
            }
            set
            {
                m_IsTableFree = value;
                OnPropertyChanged();
            }
        }

        public NotifyWindowViewModel()
        {
            var endpoint = ConfigurationManager.AppSettings["networkServiceEndpoint"];
            m_NetworkService = new FoosNetworkService(); // TODO: FoosNetworkService(endpoint);
            var testObjects = new ShowPlayersTest();
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
