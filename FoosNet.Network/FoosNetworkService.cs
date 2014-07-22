namespace FoosNet.Network
{
    public class FoosNetworkService
    {
        public delegate void ChallengeReceivedHandler(IFoosPlayer player);
        public event ChallengeReceivedHandler ChallengeReceived;

        public delegate void ChallengeResponseHandler(ChallengeResponse response);
        public event ChallengeResponseHandler ChallengeResponse;
    }
}
