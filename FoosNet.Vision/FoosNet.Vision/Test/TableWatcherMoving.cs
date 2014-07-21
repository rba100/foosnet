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
        private bool m_TableIsInUse;
        private Timer m_CheckPitchStatusTimer;
        private Image<Bgr, byte> m_DebugImage = null;

        public TableWatcherMoving()
        {
            m_CheckPitchStatusTimer = new Timer(CheckPitchStatus, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3));
        }

        private void CheckPitchStatus(object state)
        {
            bool previousTableInUse = m_TableIsInUse;

            var img1 = new Image<Bgr, byte>(System.IO.Path.GetFullPath(".\\..\\FoosNet.Vision\\Images\\empty3.png"));
            var img2 = new Image<Bgr, byte>(System.IO.Path.GetFullPath(".\\..\\FoosNet.Vision\\Images\\empty4.png"));

            if(new Random().NextDouble() > 0.5)
                m_DebugImage = img2.Copy();
            else
                m_DebugImage = img1.Copy();

            int threshold = 50;
            int contourThreshold = 140;

            Image<Bgr, byte>  diffPic = img1.AbsDiff(img2);

            byte[, ,] data = diffPic.Data;
            long totalB = 0, totalG = 0, totalR = 0;

            for (int i = diffPic.Rows - 1; i >= 0; i--)
                for (int j = diffPic.Cols - 1; j >= 0; j--)
                {
                    totalB += data[i, j, 0];
                    totalG += data[i, j, 1];
                    totalR += data[i, j, 2];
                }

            diffPic = diffPic.Erode(2).Dilate(2);

            diffPic = diffPic.ThresholdBinary(new Bgr(threshold, threshold, threshold), new Bgr(255, 255, 255));

            using (MemStorage storage = new MemStorage()) //allocate storage for contour approximation
                //detect the contours and loop through each of them
                for (Contour<Point> contours = diffPic.Convert<Gray, Byte>().FindContours(
                     Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                     Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST,
                     storage);
                  contours != null;
                  contours = contours.HNext)
                {
                    //Create a contour for the current variable for us to work with
                    Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);

                    //Draw the detected contour on the image
                    if (currentContour.Area > contourThreshold) //only consider contours with area greater than 100 as default then take from form control
                    {
                        m_DebugImage.Draw(currentContour.BoundingRectangle, new Bgr(Color.Red), 2);
                    }
                }
      
            m_TableIsInUse = true;

            if (m_TableIsInUse && !previousTableInUse) TableHasBecomeInUse(this, null);
            if (!m_TableIsInUse && previousTableInUse) TableHasBecomeFree(this, null);
        }

        public bool TableIsInUse
        {
            get
            {
                return m_TableIsInUse;
            }

            set { m_TableIsInUse = value; }
        }

        public Image<Bgr, byte> LatestImage
        {
            get { return m_DebugImage; }
        }
        public event EventHandler TableHasBecomeInUse;
        public event EventHandler TableHasBecomeFree;
    }
}
