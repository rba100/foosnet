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
        internal static double TableBusyProbability(Image<Bgr, byte> img1, Image<Bgr, byte> img2, Image<Bgr, byte> debugImage)
        {
            int threshold = 40;
            int contourThreshold = 70;

            Image<Bgr, byte> diffPic = img1.AbsDiff(img2);


            diffPic = diffPic.PyrDown().PyrDown().PyrUp().PyrUp();
            diffPic = diffPic.Erode(1).Dilate(1);

            diffPic = diffPic.ThresholdBinary(new Bgr(threshold, threshold, threshold), new Bgr(255, 255, 255));

            int contourCount = 0;
            int contourLeftTotal = 0;
            int contourTopTotal = 0;

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
                        contourCount++;
                        contourLeftTotal += currentContour.BoundingRectangle.Left + (currentContour.BoundingRectangle.Width / 2);
                        contourTopTotal += currentContour.BoundingRectangle.Top + (currentContour.BoundingRectangle.Height / 2);
                        if (debugImage != null)
                            debugImage.Draw(currentContour.BoundingRectangle, new Bgr(Color.Red), 2);
                    }
                }

            if (contourCount > 0)
            {
                float actionCentreLeft = contourLeftTotal / contourCount;
                float actionCentreTop = contourTopTotal / contourCount;

                if (debugImage != null)
                    debugImage.Draw(new CircleF(new PointF(actionCentreLeft, actionCentreTop), 8), new Bgr(0, 100, 200), 3);

                if (contourCount > 4)
                {
                    if (actionCentreLeft < 350 && actionCentreTop < 300) return 0.9;
                    if (actionCentreLeft < 400 && actionCentreTop < 350) return 0.8;
                }
                else
                {
                    if (actionCentreLeft < 350 && actionCentreTop < 300) return 0.8;
                    if (actionCentreLeft < 400 && actionCentreTop < 350) return 0.7;
                }
            }

            return 0.0;
        }
    }
}
