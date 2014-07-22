using System.Collections.ObjectModel;
using FoosNet.Network;

namespace FoosNet.Tests
{
    class ShowPlayersTest
    {
        public ObservableCollection<FoosPlayerListItem> GetPlayers()
        {
            var players = new ObservableCollection<FoosPlayerListItem>()
            {
                new FoosPlayerListItem("Robin Anderson", Status.Available, 1){GameState = GameState.Accepted},
                new FoosPlayerListItem("Aaron Law", Status.Available, 2),
                new FoosPlayerListItem("Mark Raymond", Status.Available, 2) {GameState = GameState.Pending},
                new FoosPlayerListItem("Oliver Lane", Status.Available, 2),
                new FoosPlayerListItem("Reka Burmeister", Status.Available, 2) {GameState = GameState.Declined},
                new FoosPlayerListItem("Aaron Law", Status.Available, 2),
                new FoosPlayerListItem("Ali Daw", Status.Available, 3),
                new FoosPlayerListItem("Martin Podlubny", Status.Available, 4) {GameState = GameState.Declined},
                new FoosPlayerListItem("Kevin Boyle", Status.Busy, 5) {GameState = GameState.Declined},
                new FoosPlayerListItem("Chris Moore", Status.Offline, 6),
                new FoosPlayerListItem("Reka Burmeister", Status.Unknown, 6),
                new FoosPlayerListItem("Chris Spencer", Status.Useless, 6),
            };
            return players;
        }
    }
}
