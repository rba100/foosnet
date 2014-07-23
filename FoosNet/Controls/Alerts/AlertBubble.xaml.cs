using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FoosNet.Network;

namespace FoosNet.Controls.Alerts
{
    /// <summary>
    /// Interaction logic for AlertBubble.xaml
    /// </summary>
    public partial class AlertBubble : Window
    {
        private readonly ChallengeRequest m_Challenge;
        private int? m_AutoDeclineTimeLeft;
        private readonly Timer m_AutoDeclineTimer;
        private Timer m_ClosePopupTimer;

        public event Action<ChallengeRequest, bool> ChallengeResponseReceived = delegate {};


        /// <param name="autoDeclineTimeLeft">
        /// Time, in seconds, before the challenge is auto declined.
        /// If null, message will not be auto declined.
        /// </param>
        public AlertBubble(ChallengeRequest challenge, int? autoDeclineTimeLeft)
        {
            InitializeComponent();

            if (challenge == null) throw new ArgumentNullException("challenge");

            m_Challenge = challenge;
            m_AutoDeclineTimeLeft = autoDeclineTimeLeft;

            SetDescriptionText(challenge, autoDeclineTimeLeft);

            if (autoDeclineTimeLeft != null)
            {
                m_AutoDeclineTimer = new Timer {Interval = 1000};
                m_AutoDeclineTimer.Elapsed += (s, e) =>
                {
                    m_AutoDeclineTimeLeft--;

                    Dispatcher.Invoke
                        (() => SetDescriptionText(m_Challenge, 
                                                  m_AutoDeclineTimeLeft));

                    if (m_AutoDeclineTimeLeft == 0)
                    {
                        m_AutoDeclineTimer.Stop();
                        Dispatcher.Invoke(() =>
                        {
                            Decline();
                            Close();
                        });
                    }
                };
                m_AutoDeclineTimer.Start();
            }
        }


        /// <param name="autoDeclineTimeLeft">
        /// If null, auto decline message will not be shown
        /// </param>
        private void SetDescriptionText(ChallengeRequest challenge,
                                        int? autoDeclineTimeLeft)
        {
            DescriptionText.Text = challenge.Challenger.DisplayName + 
                                   " challenged you to a match!";

            if (autoDeclineTimeLeft != null)
            {
                DescriptionText.Text += "\r\nThis request will be auto-" +
                                        "declined after " + autoDeclineTimeLeft
                                        + " seconds.";
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
            AlertBubbleBorder.Background = Brushes.Gray;
            ChallengeResponseReceived(m_Challenge, false);
        }

        private void Accept()
        {
            AlertBubbleBorder.Background = Brushes.Green;
            ChallengeResponseReceived(m_Challenge, true);
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
            anim.Completed += (s, _) => Close();
            BeginAnimation(OpacityProperty, anim);
        }
    }
}
