using System.Collections.Generic;
using System.Linq;
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
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        public AllPlayersJoined(IEnumerable<IFoosPlayer> foosPlayers) : this()
        {
            var players = foosPlayers.ToArray();
            Name1.Text = players[0].DisplayName;
            Name2.Text = players[1].DisplayName;
            Name3.Text = players[2].DisplayName;
            Name4.Text = players[3].DisplayName;
        }

        private void AckButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
