using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosNet.Vision.Test
{
    public class TableWatcherMoving : ITableWatcher
    {
        private IImageStream m_ImageStream = null;

        private bool m_TableIsInUse;
        private Image<Bgr, byte> m_DebugImage = null;
        private Image<Bgr, byte> m_PreviousImage = null;

        public TableWatcherMoving()
        {
            m_ImageStream = new ImageStreamFromStaticImages();
            m_ImageStream.LatestImageAvailable += LatestImageAvailable;
        }

        private void LatestImageAvailable(object state, Image<Bgr, byte> latestImage)
        {
            if (m_PreviousImage == null)
            {
                // On first run, there's no previous image to comapare with. So store image and return
                m_PreviousImage = latestImage;
                return;
            }
            m_DebugImage = latestImage.Copy();
            bool previousTableInUse = m_TableIsInUse;

            double tableInUseProb = ImageProcessing.DetectIfTableIsInUse(latestImage, m_PreviousImage, m_DebugImage);

            m_TableIsInUse = tableInUseProb > 0.5;

            if (m_TableIsInUse && !previousTableInUse) TableHasBecomeInUse(this, null);
            if (!m_TableIsInUse && previousTableInUse) TableHasBecomeFree(this, null);

            m_PreviousImage = latestImage;
        }

        public bool TableIsInUse
        {
            get
            {
                return m_TableIsInUse;
            }
            set { m_TableIsInUse = value; }
        }

        public Image<Bgr, byte> DebugImage
        {
            get { return m_DebugImage; }
        }
        public event EventHandler TableHasBecomeInUse;
        public event EventHandler TableHasBecomeFree;
    }
}
