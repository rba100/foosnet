using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using FoosNet.Network;
using FoosNet.Network.TcpServer;

namespace TestAlerts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var alerter = new FoosAlerter();

            alerter.AlertChallenge(new TcpFoosballPlayer("Dave", 
                                                         Status.Available, 
                                                         null));

            alerter.ChallengeResponseReceived += OnChallengeResponseReceived;

            TestWindow.Hide();

        }

        private void OnChallengeResponseReceived(ChallengeResponse response)
        {
            System.Windows.MessageBox.Show("Response: "+ response.Accepted);
        }
    }
}
