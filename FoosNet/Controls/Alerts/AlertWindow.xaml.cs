﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Input;
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

        private readonly ChallengeRequest m_Challenge;

        private readonly Timer m_StrobeTimer;
        
        public delegate void ChallengeResponseEventHandler(ChallengeResponse response);
        public event ChallengeResponseEventHandler ChallengeResponseReceived = delegate {};
        
        // Needed so we can close the other windows from the layer above
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
                           ChallengeRequest challenge)
        {
            InitializeComponent();

            m_CancelledColors = cancelledColors;

            m_Challenge = challenge;

            m_StrobeTimer = new Timer {Interval = 1000};

            DescriptionText.Text = "You have been challenged by " 
                                    + m_Challenge.Challenger.DisplayName + "!";

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
            
            // ReSharper disable once EmptyGeneralCatchClause
            try { 
                OpenDiscDrive();
                CloseDiscDrive();
            }
            catch (Exception e)
            {
                //what's a little more evil when we're already opening the disc
                //tray for a notification?
            }
        }

        public void CancelChallenge()
        {
            m_StrobeTimer.Stop();
            
            DescriptionText.Text =  m_Challenge.Challenger.DisplayName + " has cancelled" +
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
        
        //disable Alt+F4
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }

        [DllImport("winmm.dll", EntryPoint = "mciSendString")]
        public static extern int mciSendStringA(string lpstrCommand, 
                                                string lpstrReturnString, 
                                                int uReturnLength,
                                                int hwndCallback);

        private void OpenDiscDrive()
        {
            string driveLetter = GetCDDriveLetter();
            string returnString = String.Empty;
            
            if(driveLetter.Equals(string.Empty)) return;

            mciSendStringA("open " + driveLetter + ": type CDaudio alias drive" 
                            + driveLetter, returnString, 0, 0);

            mciSendStringA("set drive" + driveLetter + " door open", 
                           returnString, 0, 0);
        }

        private void CloseDiscDrive()
        {
            string driveLetter = GetCDDriveLetter();
            string returnString = String.Empty;

            if(driveLetter.Equals(string.Empty)) return;

            mciSendStringA("open " + driveLetter + ": type CDaudio alias drive" 
                            + driveLetter, returnString, 0, 0);

            mciSendStringA("set drive" + driveLetter + " door closed", 
                           returnString, 0, 0);
        }

        private string GetCDDriveLetter()
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.DriveType == DriveType.CDRom)
                {
                    return drive.Name.Substring(0, 1);
                }
            }
            return String.Empty;
        }
    }
}
