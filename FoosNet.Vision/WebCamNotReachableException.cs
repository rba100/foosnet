using System;

namespace FoosNet.Vision
{
    public class WebCamNotReachableException : Exception
    {
        public WebCamNotReachableException(Exception e) : base("Webcam not reachable for some reason", e)
        {
        }
    }
}
