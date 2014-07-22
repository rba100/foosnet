using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using FoosNet.Network;
using TestAlerts;

namespace FoosNet.Controls.Alerts
{
    public class FullScreenFoosAlerter : IFoosAlerter
    {
        private AlertWindow m_MainAlertWindow;

        private List<SecondaryAlertWindow> m_SecondaryAlertWindows;

        public delegate void ChallengeResponseEventHandler(ChallengeResponse response);
        public event ChallengeResponseEventHandler ChallengeResponseReceived = delegate {};

        public void ShowChallengeAlert(ChallengeRequest challenge)
        {
            // Close any alerts already open before we start
            CloseChallengeAlert();

            SpeakerBeep();

            var alertColors = new []
            {
                new Tuple<SolidColorBrush, SolidColorBrush> (Brushes.Red, Brushes.White),
                new Tuple<SolidColorBrush, SolidColorBrush> (Brushes.Green, Brushes.Black)
            };

            var cancelledColors =
                new Tuple<SolidColorBrush, SolidColorBrush> (Brushes.Gray, Brushes.White);

            m_MainAlertWindow = new AlertWindow(alertColors, cancelledColors, challenge)
            {
                Top = Screen.PrimaryScreen.WorkingArea.Top,
                Left = Screen.PrimaryScreen.WorkingArea.Left,
                Width = Screen.PrimaryScreen.WorkingArea.Width,
                Height = Screen.PrimaryScreen.WorkingArea.Height
            };
            m_MainAlertWindow.Show();
            m_MainAlertWindow.Activate();
            m_MainAlertWindow.WindowState = WindowState.Maximized;

            m_MainAlertWindow.ChallengeResponseReceived += ChallengeResponseHandler;
            m_MainAlertWindow.AlertClosed += AlertClosedHandler;

            m_SecondaryAlertWindows = new List<SecondaryAlertWindow>();

            foreach (var screen in Screen.AllScreens)
            {
                if (!Equals(screen, Screen.PrimaryScreen)) { 
                    var window = new SecondaryAlertWindow(alertColors, cancelledColors)
                    {
                        Top = screen.WorkingArea.Top,
                        Left = screen.WorkingArea.Left,
                        Width = screen.WorkingArea.Width,
                        Height = screen.WorkingArea.Height
                    };
                    window.Show();
                    window.Activate();
                    window.WindowState = WindowState.Maximized;

                    m_SecondaryAlertWindows.Add(window);
                }
            }
        }


        public void CancelChallengeAlert()
        {
            if (m_MainAlertWindow != null) { 
                m_MainAlertWindow.CancelChallenge();
            }
            if (m_SecondaryAlertWindows != null)
            {
                foreach (var secondaryAlertWindow in m_SecondaryAlertWindows)
                {
                    secondaryAlertWindow.CancelAlert();
                }
            }
        }

        public void CloseChallengeAlert()
        {
            if (m_MainAlertWindow != null) { 
                m_MainAlertWindow.Close();
            }
            if (m_SecondaryAlertWindows != null)
            {
                foreach (var secondaryAlertWindow in m_SecondaryAlertWindows)
                {
                    secondaryAlertWindow.Close();
                }
            }
        }
        
        private void ChallengeResponseHandler(ChallengeResponse response)
        {
            CloseChallengeAlert();
            ChallengeResponseReceived(response);
        }

        private void AlertClosedHandler()
        {
            CloseChallengeAlert();
        }

        private void SpeakerBeep()
        {
            // Unfortunately 64-bit versions of windows no longer support
            // the internal speaker beep, but they'll still play it through 
            // the external speakers.
            Console.Beep();
        }
    }
}