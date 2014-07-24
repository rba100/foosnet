using FoosNet.Network;

namespace FoosNet.PlayerFilters
{
    public class StatusToUnknownTransformation : IPlayerTransformation
    {
        public IFoosPlayer Process(IFoosPlayer player)
        {
            player.Status = Status.Unknown;
            return player;
        }
    }
}
