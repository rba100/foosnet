using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Net;

namespace FoosNet.Vision
{
    public class ImageStreamFromFoosCam : IImageStream
    {
        private Image<Bgr, byte> m_LatestImage;
        private readonly Timer m_GetNextImageTimer;

        public Image<Bgr, byte> LatestImage
        {
            get
            {
                if (m_LatestImage == null)
                    RetrieveNextImage(false);
                return m_LatestImage;
            }
        }

        public event EventHandler<Image<Bgr, byte>> LatestImageAvailable;

        internal ImageStreamFromFoosCam()
        {
            m_GetNextImageTimer = new Timer(RetrieveNextImage, true, 500, 3621);
        }

        public static Image GetImageFromUrl(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (Stream stream = httpWebReponse.GetResponseStream())
                {
                    return Image.FromStream(stream);
                }
            }
        }

        private void RetrieveNextImage(object triggerEvent)
        {
            try
            {
                Image img = GetImageFromUrl("http://10.120.115.224/snapshot.cgi?user=viewer&pwd=");
                m_LatestImage = new Image<Bgr, byte>((Bitmap)img);

                if ((bool)triggerEvent && LatestImageAvailable != null)
                    LatestImageAvailable(this, m_LatestImage);
            }
            catch(Exception e)
            {
                //throw new WebCamNotReachableException(e);
            }
        }
    }
}
