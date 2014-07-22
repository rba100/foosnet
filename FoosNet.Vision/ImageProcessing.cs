using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosNet.Vision
{
    class ImageProcessing
    {
        /// <summary>
        /// Get sureness that table is in use
        /// </summary>
        /// <param name="img1">Current image</param>
        /// <param name="img2">Previous image</param>
        /// <param name="debugImage">Debug image. Optional. Pass null if you don't care</param>
        /// <returns>Magnitude of difference. Roughly it's the probability</returns>
        internal static double DetectIfTableIsInUse(Image<Bgr, byte> img1, Image<Bgr, byte> img2, Image<Bgr, byte> debugImage)
        {
            int threshold = 50;
            int contourThreshold = 100;

            Image<Bgr, byte> diffPic = img1.AbsDiff(img2);


            diffPic = diffPic.PyrDown().PyrDown().PyrUp().PyrUp();
            diffPic = diffPic.Erode(1).Dilate(1);

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
                        if (debugImage != null)
                            debugImage.Draw(currentContour.BoundingRectangle, new Bgr(Color.Red), 2);
                    }
                }

            byte[, ,] data = diffPic.Data;
            long totalB = 0, totalG = 0, totalR = 0;

            for (int i = diffPic.Rows - 1; i >= 0; i--)
                for (int j = diffPic.Cols - 1; j >= 0; j--)
                {
                    totalB += data[i, j, 0];
                    totalG += data[i, j, 1];
                    totalR += data[i, j, 2];
                }

            double diffMag = (double)(totalB + totalG + totalR) / 10e5;

            if (diffMag  > 1)
                return 0.1;
            else if (diffMag > 2)
                return 0.3;
            else if (diffMag > 4)
                return 0.5;
            else if (diffMag > 8)
                return 0.7;
            else if (diffMag > 16)
                return 0.9;
            else return
                1.0;
        }
    }
}
