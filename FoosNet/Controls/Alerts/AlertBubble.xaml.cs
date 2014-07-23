using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using FoosNet.Network;

namespace FoosNet.Controls.Alerts
{
    /// <summary>
    /// Interaction logic for AlertBubble.xaml
    /// </summary>
    public partial class AlertBubble : Window
    {
        private readonly ChallengeRequest m_Challenge;
        private readonly Timer m_AutoDeclineTimer;
        private Timer m_ClosePopupTimer;

        public delegate void ChallengeResponseEventHandler(ChallengeResponse response);
        public event ChallengeResponseEventHandler ChallengeResponseReceived = delegate {};

        public AlertBubble(ChallengeRequest challenge, bool autoDecline)
        {
            InitializeComponent();

            if (challenge == null) throw new ArgumentNullException("challenge");

            m_Challenge = challenge;

            DescriptionText.Text = challenge.Challenger.DisplayName + 
                                   " challenged you to a match!";

            if (autoDecline)
            {
                DescriptionText.Text += "\r\nThis request will be auto-" +
                                        "declined after 20 seconds.";
                
                m_AutoDeclineTimer = new Timer {Interval = 20000};
                m_AutoDeclineTimer.Elapsed += (s, e) =>
                {
                    m_AutoDeclineTimer.Stop();
                    Dispatcher.Invoke(() =>
                    {
                        Decline();
                        Close();
                    });
                };
                m_AutoDeclineTimer.Start();
            }
        }

        public void CancelChallenge()
        {
            m_AutoDeclineTimer.Stop();

            m_ClosePopupTimer = new Timer{Interval = 3000};
            m_ClosePopupTimer.Elapsed += (s, e) =>
            {
                m_ClosePopupTimer.Stop();
                Dispatcher.Invoke(Close);
            };
            m_ClosePopupTimer.Start();

            AlertBubbleBorder.Background = Brushes.Gray;

            DescriptionText.Text = m_Challenge.Challenger.DisplayName +
                                   " cancelled the challenge.";

            AcceptButton.IsEnabled = false;
            DeclineButton.IsEnabled = false;
        }

        private void Decline()
        {
            ChallengeResponseReceived(new ChallengeResponse(m_Challenge.Challenger, false));
        }

        private void Accept()
        {
            ChallengeResponseReceived(new ChallengeResponse(m_Challenge.Challenger, true));
        }

        private void AcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            Accept();
            Close();
        }

        private void DeclineButton_OnClick(object sender, RoutedEventArgs e)
        {
            Decline();
            Close();
        }
    }
}
