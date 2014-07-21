using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoosNet.Network
{
    public interface IFoosballNetwork
    {
        IEnumerable<IFoosPlayer> GetPlayers();
        void SetStatus(string name, Status status);
    }

    public interface IFoosPlayer
    {
        // Properties
        string Name { get; }
        Status Status { get; }

        // Actions
        Task<ChallengeResponse> ChallengePlayer();
    }


    public class ChallengeResponse
    {
        public bool Accepted { get; private set; }
    }

    public enum Status { Unknown, Available, Busy, Useless }
}
