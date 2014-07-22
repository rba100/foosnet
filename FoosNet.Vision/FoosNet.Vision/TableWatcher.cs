using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosNet.Vision
{
    public class TableWatcher : ITableWatcher
    {
        private bool m_TableIsInUse;
        private Timer m_CheckPitchStatusTimer;

        public TableWatcher()
        {
            m_TableIsInUse = false;
            m_CheckPitchStatusTimer = new Timer(CheckPitchStatus, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
            //InitializeEmgu();
        }

        private void CheckPitchStatus(object state)
        {
            m_TableIsInUse = true;
        }

        public bool TableIsInUse
        {
            get
            {
                return m_TableIsInUse;
            }

            set { m_TableIsInUse = value; }
        }

        public event EventHandler TableHasBecomeInUse;
        public event EventHandler TableHasBecomeFree;

        public Image<Bgr, byte> DebugImage
        {
            get { return new Image<Bgr, byte>(640, 480, new Bgr(100, 10, 200)); }
        }
    }
}
