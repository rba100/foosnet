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
    public class TableWatcherRandom : ITableWatcher
    {
        private bool m_TableIsInUse;
        private Timer m_CheckPitchStatusTimer;

        public TableWatcherRandom()
        {
            m_CheckPitchStatusTimer = new Timer(CheckPitchStatus, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
        }

        private void CheckPitchStatus(object state)
        {
            bool previousTableInUse = m_TableIsInUse;
            m_TableIsInUse = (new Random()).NextDouble() > 0.5;

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

        public Image<Bgr, byte> DebugImage
        {
            get { return new Image<Bgr, byte>(640, 480, new Bgr(100, 10, 200)); }
        }
        public event EventHandler TableHasBecomeInUse;
        public event EventHandler TableHasBecomeFree;
    }
}
