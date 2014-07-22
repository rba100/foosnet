using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosNet.Vision.Test
{
    internal class ImageStreamFromStaticImages: IImageStream
    {
        private int m_ImageNumber = 0;
        private Timer m_Timer;

        private void ChangeImage(object state)
        {
            m_ImageNumber++;
            if (m_ImageNumber == 2) m_ImageNumber = 0;
            if (LatestImageAvailable != null) LatestImageAvailable(this, LatestImage);
        }

        internal ImageStreamFromStaticImages()
        {
            m_Timer = new Timer(ChangeImage, null, 500, 1000);
        }

        public Image<Bgr, byte> LatestImage
        {
            get
            {
                if (m_ImageNumber == 0)
                    return new Image<Bgr, byte>(System.IO.Path.GetFullPath(".\\..\\..\\..\\FoosNet.Vision\\Images\\inuse1.png"));
                else if (m_ImageNumber == 1)
                    return new Image<Bgr, byte>(System.IO.Path.GetFullPath(".\\..\\..\\..\\FoosNet.Vision\\Images\\inuse2.png"));
                else
                    throw new ApplicationException();
            }
        }

        public event EventHandler<Image<Bgr, byte>> LatestImageAvailable;
    }
}
