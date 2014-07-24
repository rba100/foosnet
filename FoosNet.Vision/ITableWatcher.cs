using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosNet.Vision
{
    public interface ITableWatcher
    {
        TableUsage TableUsage { get; }
        event EventHandler TableNowBusy;
        event EventHandler TableNowFree;
        Image<Bgr, byte> DebugImage { get; }
    }
}