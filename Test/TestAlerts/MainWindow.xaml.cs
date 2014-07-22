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

            alerter.ShowChallengeAlert(new FoosChallenge {
                Challenger = new TcpFoosballPlayer("Dave", 
                                                   Status.Available, 
                                                   null)
            });

            alerter.ChallengeResponseReceived += OnChallengeResponseReceived;

            alerter.AlertClosed += onAlertClosed;
            
            alerter.ShowChallengeAlert(new FoosChallenge {
                Challenger = new TcpFoosballPlayer("Gary", 
                                                   Status.Available, 
                                                   null)
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
