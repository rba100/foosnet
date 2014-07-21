using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace TestAlerts
{
    /// <summary>
    /// Interaction logic for SecondaryAlertWindow.xaml
    /// </summary>
    public partial class SecondaryAlertWindow : Window
    {
        public SecondaryAlertWindow()
        {
            InitializeComponent();

            var strobeTimer = new Timer {Interval = 1000};

            var colours = new []
            {
                new Tuple<SolidColorBrush, SolidColorBrush> (Brushes.Red, Brushes.White),
                new Tuple<SolidColorBrush, SolidColorBrush> (Brushes.Green, Brushes.Black)
            };

            var currentColour = 0;
            
            SecondaryAlertWindowElement.Background = colours[currentColour].Item1;
            AlertText.Foreground = colours[currentColour].Item2;

            strobeTimer.Elapsed += (sender, elapsedEventArgs) => Dispatcher.Invoke(() =>
            {
                currentColour = (currentColour + 1) % colours.Length;
                SecondaryAlertWindowElement.Background = colours[currentColour].Item1;
                AlertText.Foreground = colours[currentColour].Item2;
            });

            strobeTimer.Start(); 
        }
    }
}
