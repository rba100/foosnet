using System.Windows;

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

            alerter.AlertClosed += onAlertClosed;

            alerter.CancelChallenge();

        }

        private void OnChallengeResponseReceived(ChallengeResponse response)
        {
            MessageBox.Show("Response: "+ response.Accepted);
            TestWindow.Show();
        }

        private void onAlertClosed()
        {
            MessageBox.Show("Alert was closed");
        }
    }
}
