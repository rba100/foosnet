using System.Collections.Generic;
using FoosNet.Network;

namespace FoosNet
{
    class AllPlayersJoinedViewModel
    {
        public IEnumerable<IFoosPlayer> FoosPlayers { get; set; }
    }
}
