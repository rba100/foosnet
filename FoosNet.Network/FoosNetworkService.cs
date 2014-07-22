namespace FoosNet.Network
{
    public class FoosNetworkService
    {
        public delegate void ChallengeReceivedHandler(IFoosPlayer player);
        public event ChallengeReceivedHandler ChallengeReceived;

        public delegate void ChallengeResponseHandler(IFoosPlayer player);
        public event ChallengeResponseHandler ChallengeResponse;
    }
}
