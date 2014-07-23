namespace FoosNet.Network
{
    public class ChallengeResponse
    {
        private readonly IFoosPlayer m_Player;
        private readonly bool m_Accepted;

        public ChallengeResponse(IFoosPlayer player, bool accepted)
        {
            m_Player = player;
            m_Accepted = accepted;
        }

        public IFoosPlayer Player { get { return m_Player; } }
        public bool Accepted { get { return m_Accepted; } }
    }
}
