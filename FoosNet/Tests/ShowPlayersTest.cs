using System.Collections.ObjectModel;
using FoosNet.Network;

namespace FoosNet.Tests
{
    class ShowPlayersTest
    {
        public ObservableCollection<FoosPlayer> GetPlayers()
        {
            var players = new ObservableCollection<FoosPlayer>()
            {
                new FoosPlayer("Robin Anderson", Status.Available, 1){GameState = GameState.Accepted},
                new FoosPlayer("Aaron Law", Status.Available, 2),
                new FoosPlayer("Mark Raymond", Status.Available, 2) {GameState = GameState.Pending},
                new FoosPlayer("Oliver Lane", Status.Available, 2),
                new FoosPlayer("Reka Burmeister", Status.Available, 2) {GameState = GameState.Declined},
                new FoosPlayer("Aaron Law", Status.Available, 2),
                new FoosPlayer("Ali Daw", Status.Available, 3),
                new FoosPlayer("Martin Podlubny", Status.Available, 4) {GameState = GameState.Declined},
                new FoosPlayer("Kevin Boyle", Status.Busy, 5) {GameState = GameState.Declined},
                new FoosPlayer("Chris Moore", Status.Offline, 6),
            };
            return players;
        }
    }
}
