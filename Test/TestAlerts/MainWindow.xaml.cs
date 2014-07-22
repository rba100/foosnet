﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

using FoosNet.Network;
using FoosNet.Network.Annotations;

namespace TestAlerts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class TestFoosPlayer : IFoosPlayer
        {
            public string Email { get; set; }
            public string DisplayName { get; set; }
            public Status Status { get; set; }

            public TestFoosPlayer(string email, string displayName, Status status)
            {
                Email = email;
                DisplayName = displayName;
                Status = status;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged1([CallerMemberName] string propertyName = null)
            {
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            var alerter = new FoosAlerter();

            alerter.ShowChallengeAlert(new FoosChallenge {
                Challenger = new TestFoosPlayer("Dave", "Dave", Status.Available)
            });

            alerter.ChallengeResponseReceived += OnChallengeResponseReceived;

            alerter.AlertClosed += onAlertClosed;
            
            alerter.ShowChallengeAlert(new FoosChallenge {
                Challenger = new TestFoosPlayer("Gary", "Gary", Status.Available)
            });

        }

        private void OnChallengeResponseReceived(ChallengeResponse response)
        {
            MessageBox.Show("Response: "+ response.Accepted);
        }

        private void onAlertClosed()
        {
            MessageBox.Show("Alert was closed");
        }
    }
}
