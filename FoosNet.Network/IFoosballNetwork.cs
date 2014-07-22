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
        public IFoosPlayer Player { get; set; }
        public bool Accepted { get; set; }
    }

    public enum Status { Unknown, Available, Busy, Offline, Useless }
}
