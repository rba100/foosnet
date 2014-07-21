using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoosNet.Network;

namespace FoosNet.Tests
{
    class ShowPlayersTest
    {
        public ObservableCollection<FoosPlayer> GetPlayers()
        {
            ObservableCollection<FoosPlayer> players = new ObservableCollection<FoosPlayer>()
            {
                new FoosPlayer("Robin Anderson", Status.Available, 1),
                new FoosPlayer("Aaron Law", Status.Available, 2),
                new FoosPlayer("Ali Daw", Status.Available, 3),
                new FoosPlayer("Martin Podlubny", Status.Available, 4),
                new FoosPlayer("Kevin Boyle", Status.Busy, 5),
                new FoosPlayer("Chris Moore", Status.Offline, 6),
            };
            return players;
        }
    }
}
