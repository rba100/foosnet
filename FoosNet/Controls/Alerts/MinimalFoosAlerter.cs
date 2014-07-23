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

            m_AlertBubble.Show();
        }

        public void CancelChallengeAlert()
        {
            m_AlertBubble.CancelChallenge();
        }

        public void CloseChallengeAlert()
        {
            m_AlertBubble.Close();
        }
    }
}
