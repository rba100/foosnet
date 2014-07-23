using System;
using System.Windows.Forms;
using FoosNet.Network;

namespace FoosNet.Controls.Alerts
{
    public class MinimalFoosAlerter : IFoosAlerter
    {
        private AlertBubble m_AlertBubble;

        public void ShowChallengeAlert(ChallengeRequest foosChallenge)
        {
            m_AlertBubble = new AlertBubble(foosChallenge, 30)
            {
                Top = Screen.PrimaryScreen.WorkingArea.Height - 200,
                Left = Screen.PrimaryScreen.WorkingArea.Width - 300
            };
            m_AlertBubble.ChallengeResponseReceived += AlertBubbleOnChallengeResponseReceived;
            m_AlertBubble.Show();
        }

        private void AlertBubbleOnChallengeResponseReceived(ChallengeRequest request, bool accepted)
        {
            if (ChallengeResponseReceived != null) ChallengeResponseReceived(request, accepted);
        }

        public event Action<ChallengeRequest, bool> ChallengeResponseReceived;

        public void CancelChallengeAlert()
        {
            m_AlertBubble.CancelChallenge();
        }

        public void CloseChallengeAlert()
        {
            if (!m_AlertBubble.Dispatcher.CheckAccess())
            {
                m_AlertBubble.Dispatcher.BeginInvoke(new Action(() => m_AlertBubble.Close()));
                return;
            }
            m_AlertBubble.Close();
        }
    }
}
