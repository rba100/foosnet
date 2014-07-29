using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FoosNet.Network;

namespace FoosNet.Views
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
            Name1.Text = GetPlayerName(0, players);
            Name2.Text = GetPlayerName(1, players);
            Name3.Text = GetPlayerName(2, players);
            Name4.Text = GetPlayerName(3, players);
        }

        private string GetPlayerName(int playerIndex, IFoosPlayer[] playerArray)
        {
            if (playerArray.Length -1 < playerIndex) return String.Empty;
            return playerArray[playerIndex].DisplayName;
        }

        private void AckButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
