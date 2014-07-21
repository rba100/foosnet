using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using FoosNet.Network;

namespace TestAlerts
{
    /// <summary>
    /// Interaction logic for AlertWindow.xaml
    /// </summary>
    public partial class AlertWindow : Window
    {
        private readonly Tuple<SolidColorBrush, SolidColorBrush> m_CancelledColors;

        private readonly Timer m_StrobeTimer;

        public delegate void ChallengeResponseEventHandler(ChallengeResponse response);

        public event ChallengeResponseEventHandler ChallengeResponseReceived;

        /// <param name="alertColors">
        /// List of (backgroundColor, foregroundColor) tuples to cycle through
        /// when alerting
        /// </param>
        /// <param name="cancelledColors">
        /// (backgroundColor, foregroundColor) tuple, used if the challenge is
        /// cancelled
        /// </param>
        public AlertWindow(Tuple<SolidColorBrush, SolidColorBrush> [] alertColors,
                           Tuple<SolidColorBrush, SolidColorBrush> cancelledColors)
        {
            m_CancelledColors = cancelledColors;
            InitializeComponent();

            m_StrobeTimer = new Timer {Interval = 1000};

            var currentColour = 0;
            
            AlertWindowElement.Background = alertColors[currentColour].Item1;
            AlertText.Foreground = alertColors[currentColour].Item2;

            m_StrobeTimer.Elapsed += (sender, elapsedEventArgs) => Dispatcher.Invoke(() =>
            {
                currentColour = (currentColour + 1) % alertColors.Length;
                AlertWindowElement.Background = alertColors[currentColour].Item1;
                AlertText.Foreground = alertColors[currentColour].Item2;
            });

            m_StrobeTimer.Start();
        }

        public void CancelAlert()
        {
            m_StrobeTimer.Stop();
            
            AlertWindowElement.Background = m_CancelledColors.Item1;
            AlertText.Foreground = m_CancelledColors.Item2;

            AcceptButton.IsEnabled = false;
            DeclineButton.IsEnabled = false;
        }

        private void AcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            ChallengeResponseReceived(new ChallengeResponse {Accepted = true});
        }

        private void DeclineButton_OnClick(object sender, RoutedEventArgs e)
        {
            ChallengeResponseReceived(new ChallengeResponse {Accepted = false});
        }


    }
}
