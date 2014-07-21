using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosNet.Vision
{
    /// <summary>
    /// The image source for use by a watcher
    /// </summary>
    interface IImageStream
    {
        Image<Bgr, byte> LatestImage { get; }
        event EventHandler<Image<Bgr, byte>> LatestImageAvailable;
    }
}
