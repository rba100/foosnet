using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using FoosNet.Vision.Test;

namespace FoosNet.Vision
{
    public class TableWatcher : ITableWatcher
    {
        private TableUsage m_TableUsage = TableUsage.Unknown;
        private TableUsage m_PreviousTableUsage = TableUsage.Unknown;
        private IImageStream m_ImageStream;
        private Timer m_CheckPitchStatusTimer;
        private Image<Bgr, byte> m_DebugImage;
        private Image<Bgr, byte> m_Previousimage;

        public TableWatcher()
        {
            m_ImageStream = new ImageStreamFromFoosCam();
            m_ImageStream.LatestImageAvailable += LatestImageAvailable;
        }

        private void LatestImageAvailable(object state, Image<Bgr, byte> image)
        {
            m_PreviousTableUsage = m_TableUsage;
            m_DebugImage = image.Clone();
            if (m_Previousimage != null)
            {
                double tableBusyProb = ImageProcessing.TableBusyProbability(image, m_Previousimage, m_DebugImage);
                if (tableBusyProb > 0.05) m_TableUsage = TableUsage.Busy;
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
