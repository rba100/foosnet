using FoosNet.Network;

namespace FoosNet.CommunicatorIntegration
{
    public class CommunicatorPlayerFilter : IPlayerTransformation
    {
        private readonly CommunicatorIntegration m_CommunicatorIntegration;

        public CommunicatorPlayerFilter(CommunicatorIntegration communicatorIntegration)
        {
            m_CommunicatorIntegration = communicatorIntegration;
        }

        public IFoosPlayer Process(IFoosPlayer player)
        {
            player.Status = m_CommunicatorIntegration.StatusOfRedgateEmail(player.Email);
            player.DisplayName = m_CommunicatorIntegration.FriendlyName(player.Email);
            return player;
        }
    }
}
