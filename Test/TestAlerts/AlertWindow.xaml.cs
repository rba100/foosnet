using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestAlerts
{
    /// <summary>
    /// Interaction logic for AlertWindow.xaml
    /// </summary>
    public partial class AlertWindow : Window
    {
        public AlertWindow()
        {
            InitializeComponent();

            var strobeTimer = new Timer {Interval = 1000};

            var colours = new []
            {
                new Tuple<SolidColorBrush, SolidColorBrush> (Brushes.Red, Brushes.White),
                new Tuple<SolidColorBrush, SolidColorBrush> (Brushes.Green, Brushes.Black)
            };

            var currentColour = 0;
            
            AlertWindowElement.Background = colours[currentColour].Item1;
            AlertText.Foreground = colours[currentColour].Item2;

            strobeTimer.Elapsed += (sender, elapsedEventArgs) => Dispatcher.Invoke(() =>
            {
                currentColour = (currentColour + 1) % colours.Length;
                AlertWindowElement.Background = colours[currentColour].Item1;
                AlertText.Foreground = colours[currentColour].Item2;
            });

            strobeTimer.Start();
        }
        
    }
}
