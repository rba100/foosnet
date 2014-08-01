using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosNet.Vision
{
    public class TableWatcher : ITableWatcher
    {
        private TableUsage m_TableUsage = TableUsage.Unknown;
        private TableUsage m_PreviousTableUsage = TableUsage.Unknown;
        private readonly IImageStream m_ImageStream;
        private Image<Bgr, byte> m_DebugImage;
        private Image<Bgr, byte> m_Previousimage;

        public TableWatcher()
        {
            m_ImageStream = new ImageStreamFromFoosCam();
            m_ImageStream.LatestImageAvailable += LatestImageAvailable;
        }

        private double m_SEMA = 0.5f;
        private const double RATE = 0.15f;

        private void LatestImageAvailable(object state, Image<Bgr, byte> image)
        {
            m_PreviousTableUsage = m_TableUsage;
            m_DebugImage = image.Clone();
            if (m_Previousimage != null)
            {
                // Use an exponential-moving average concept to smooth data
                double tableBusyProb = ImageProcessing.TableBusyProbability(image, m_Previousimage, m_DebugImage);
                m_SEMA = (m_SEMA * (1f - RATE)) + (tableBusyProb * RATE);

                if (m_SEMA > 0.5) m_TableUsage = TableUsage.Busy;
                else m_TableUsage = TableUsage.Free;
            }
            m_Previousimage = image.Clone();

            if (m_TableUsage == TableUsage.Busy && m_PreviousTableUsage != TableUsage.Busy) TableNowBusy(this, null);
            if (m_TableUsage == TableUsage.Free && m_PreviousTableUsage != TableUsage.Free) TableNowFree(this, null);
        }

        public TableUsage TableUsage
        {
            get { return m_TableUsage; }
            set { m_TableUsage = value; }
        }

        public event EventHandler TableNowBusy;
        public event EventHandler TableNowFree;

        public Image<Bgr, byte> DebugImage
        {
            get { return m_DebugImage; }
        }
    }
}
