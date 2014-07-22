using System.Collections.Generic;
using System.Windows;
using FoosNet.Network;

namespace FoosNet
{
    /// <summary>
    /// Interaction logic for AllPlayersJoined.xaml
    /// </summary>
    public partial class AllPlayersJoined : Window
    {
        public AllPlayersJoined()
        {

            InitializeComponent();
        }

        public AllPlayersJoined(IEnumerable<IFoosPlayer> foosPlayers) : this()
        {
            this.DataContext = new AllPlayersJoinedViewModel{FoosPlayers = foosPlayers};
        }

        private void AckButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
