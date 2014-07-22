using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using FoosNet.Network;
using TestAlerts;

namespace FoosNet.Controls.Alerts
{
    /// <summary>
    /// Interaction logic for AlertWindow.xaml
    /// </summary>
    public partial class AlertWindow : Window
    {
        private readonly Tuple<SolidColorBrush, SolidColorBrush> m_CancelledColors;

        private readonly IFoosChallenge m_Challenge;

        private readonly Timer m_StrobeTimer;
        
        public delegate void ChallengeResponseEventHandler(ChallengeResponse response);

        public event ChallengeResponseEventHandler ChallengeResponseReceived = delegate {};

        public delegate void AlertClosedEventHandler();

        public event AlertClosedEventHandler AlertClosed = delegate {};

        /// <param name="alertColors">
        /// List of (backgroundColor, foregroundColor) tuples to cycle through
        /// when alerting
        /// </param>
        /// <param name="cancelledColors">
        /// (backgroundColor, foregroundColor) tuple, used if the challenge is
        /// cancelled
        /// </param>
        public AlertWindow(Tuple<SolidColorBrush, SolidColorBrush> [] alertColors,
                           Tuple<SolidColorBrush, SolidColorBrush> cancelledColors,
                           IFoosChallenge challenge)
        {
            InitializeComponent();

            m_CancelledColors = cancelledColors;

            m_Challenge = challenge;

            m_StrobeTimer = new Timer {Interval = 1000};

            DescriptionText.Text = "You have been challenged by " 
                                    + m_Challenge.Challenger.Name + "!";

            var currentColour = 0;
            
            AlertWindowElement.Background = alertColors[currentColour].Item1;
            AlertText.Foreground = alertColors[currentColour].Item2;
            DescriptionText.Foreground = alertColors[currentColour].Item2;

            m_StrobeTimer.Elapsed += (sender, elapsedEventArgs) => Dispatcher.Invoke(() =>
            {
                currentColour = (currentColour + 1) % alertColors.Length;
                AlertWindowElement.Background = alertColors[currentColour].Item1;
                AlertText.Foreground = alertColors[currentColour].Item2;
                DescriptionText.Foreground = alertColors[currentColour].Item2;
            });

            m_StrobeTimer.Start();
        }

        public void CancelChallenge()
        {
            m_StrobeTimer.Stop();
            
            DescriptionText.Text =  m_Challenge.Challenger.Name + " has cancelled" +
                                    " the challenge.";
            
            AlertWindowElement.Background = m_CancelledColors.Item1;
            AlertText.Foreground = m_CancelledColors.Item2;

            AcceptButton.IsEnabled = false;
            DeclineButton.IsEnabled = false;

            CloseButton.Visibility = Visibility.Visible;
        }

        private void AcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            ChallengeResponseReceived(new ChallengeResponse {Accepted = true});
        }

        private void DeclineButton_OnClick(object sender, RoutedEventArgs e)
        {
            ChallengeResponseReceived(new ChallengeResponse {Accepted = false});
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            AlertClosed();
        }
    }
}
