using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoosNet.Network;

namespace FoosNet.Tests
{
    class ShowPlayersTest
    {
        public IEnumerable<FoosPlayer> GetPlayers()
        {
            List<FoosPlayer> players = new List<FoosPlayer>()
            {
                new FoosPlayer("Robin Anderson", Status.Available),
                new FoosPlayer("Aaron Law", Status.Available),
                new FoosPlayer("Ali Daw", Status.Available),
                new FoosPlayer("Martin Podlubny", Status.Available),
                new FoosPlayer("Kevin Boyle", Status.Busy),
                new FoosPlayer("Chris Moore", Status.Offline),
            };
            return players;
        }
    }
}
