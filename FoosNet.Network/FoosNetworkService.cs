namespace FoosNet.Network
{
    public class FoosNetworkService
    {
        public delegate void ChallengeReceivedHandler(IFoosPlayer player);
        event ChallengeReceivedHandler ChallengeReceived;

        public delegate void ChallengeResponseHandler(IFoosPlayer player);
        event ChallengeResponseHandler ChallengeResponse;
    }
}
