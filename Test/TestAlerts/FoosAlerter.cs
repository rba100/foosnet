using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using FoosNet.Network;

namespace TestAlerts
{
    class FoosAlerter : IFoosAlerter
    {
        private AlertWindow m_MainAlertWindow;

        private List<SecondaryAlertWindow> m_SecondaryAlertWindows;

        public delegate void ChallengeResponseEventHandler(ChallengeResponse response);

        public event ChallengeResponseEventHandler ChallengeResponseReceived;

       // public delegate void ChallengeResponseDelegate(ChallengeResponse response);

        public void AlertChallenge(IFoosPlayer challengingPlayer)
        {
            var alertColors = new []
            {
                new Tuple<SolidColorBrush, SolidColorBrush> (Brushes.Red, Brushes.White),
                new Tuple<SolidColorBrush, SolidColorBrush> (Brushes.Green, Brushes.Black)
            };

            var cancelledColors =
                new Tuple<SolidColorBrush, SolidColorBrush> (Brushes.Gray, Brushes.White);

            m_MainAlertWindow = new AlertWindow(alertColors, cancelledColors, challengingPlayer)
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

        private void ChallengeResponseHandler(ChallengeResponse response)
        {
            CloseAlerts();
            ChallengeResponseReceived(response);
        }

        public void CancelChallenge()
        {
            m_MainAlertWindow.CancelAlert();
            foreach (var secondaryAlertWindow in m_SecondaryAlertWindows)
            {
                secondaryAlertWindow.CancelAlert();
            }
        }

        public void CloseAlerts()
        {
            m_MainAlertWindow.Close();
            foreach (var secondaryAlertWindow in m_SecondaryAlertWindows)
            {
                secondaryAlertWindow.Close();
            }
        }


    }
}