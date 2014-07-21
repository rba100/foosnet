using System.Threading.Tasks;

namespace FoosNet.Network.TcpServer
{
    public class TcpFoosballPlayer : IFoosPlayer
    {
        public string Name { get; private set; }
        public Status Status { get; private set; }
        private FoosballClientTcp m_Client;

        public TcpFoosballPlayer(string name, Status status, FoosballClientTcp client)
        {
            Name = name;
            Status = status;
            m_Client = client;
        }

        public Task<ChallengeResponse> ChallengePlayer()
        {
            throw new System.NotImplementedException();
        }
    }
}