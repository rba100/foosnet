using System;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FoosNet.Vision.Test
{
    public class TableWatcherRandom : ITableWatcher
    {
        private TableUsage m_TableUsage;
        private Timer m_CheckPitchStatusTimer;

        public TableWatcherRandom()
        {
            m_CheckPitchStatusTimer = new Timer(CheckPitchStatus, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
        }

        private void CheckPitchStatus(object state)
        {
            TableUsage previousTableUsage = m_TableUsage;
            m_TableUsage = (new Random()).NextDouble() > 0.5 ? TableUsage.Free : TableUsage.Busy;

            if (m_TableUsage == TableUsage.Busy && previousTableUsage != TableUsage.Busy) TableNowBusy(this, null);
            if (m_TableUsage == TableUsage.Free && previousTableUsage != TableUsage.Free) TableNowFree(this, null);
        }

        public TableUsage TableUsage
        {
            get
            {
                return m_TableUsage;
            }

            set { m_TableUsage = value; }
        }

        public Image<Bgr, byte> DebugImage
        {
            get { return new Image<Bgr, byte>(640, 480, new Bgr(100, 10, 200)); }
        }
        public event EventHandler TableNowBusy;
        public event EventHandler TableNowFree;
    }
}
