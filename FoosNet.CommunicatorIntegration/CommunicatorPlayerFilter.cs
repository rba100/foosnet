using System.Collections.Generic;
using System.Linq;
using FoosNet.Network;

namespace FoosNet.CommunicatorIntegration
{
    public class CommunicatorPlayerFilter : IPlayerTransformation
    {
        private readonly List<IFoosPlayer> m_KnownPlayers = new List<IFoosPlayer>();

        private readonly CommunicatorIntegration m_CommunicatorIntegration;

        public CommunicatorPlayerFilter(CommunicatorIntegration communicatorIntegration)
        {
            m_CommunicatorIntegration = communicatorIntegration;
        }

        public IFoosPlayer Process(IFoosPlayer inputPlayer)
        {
            var player = m_KnownPlayers.FirstOrDefault(p => p.Email == inputPlayer.Email);
            var known = player != null;
            if (!known) player = inputPlayer;
            try
            {
                player.Status = m_CommunicatorIntegration.StatusOfRedgateEmail(player.Email);
                player.DisplayName = m_CommunicatorIntegration.FriendlyName(player.Email);
            }
            catch
            {
                player.Status = Status.Unknown;
                player.DisplayName = player.Email;
            }

            if(!known) m_CommunicatorIntegration.StatusChangedSubscribeEmail(player.Email);
            return player;
        }
    }
}
