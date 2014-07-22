namespace FoosNet.Network
{
    public class ChallengeResponse
    {
        public IFoosPlayer Player { get; set; }
        public bool Accepted { get; set; }
    }
}