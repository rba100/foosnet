using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace TestAlerts
{
    /// <summary>
    /// Interaction logic for SecondaryAlertWindow.xaml
    /// </summary>
    public partial class SecondaryAlertWindow : Window
    {
        private readonly Tuple<SolidColorBrush, SolidColorBrush> m_CancelledColors;
        private readonly Timer m_StrobeTimer;

        public SecondaryAlertWindow
            (Tuple<SolidColorBrush, SolidColorBrush> [] alertColorSequence,
             String [] alertTextSequence,
             Tuple<SolidColorBrush, SolidColorBrush> cancelledColors)
        {
            m_CancelledColors = cancelledColors;
            InitializeComponent();
            
            var random = new Random();

            m_StrobeTimer = new Timer {Interval = 1000 + random.Next(10)};

            var currentFrame = 0;
            
            SecondaryAlertWindowElement.Background = alertColorSequence[0].Item1;
            AlertText.Foreground = alertColorSequence[0].Item2;

            AlertText.Text = alertTextSequence[0];

            m_StrobeTimer.Elapsed += (sender, elapsedEventArgs) => Dispatcher.Invoke(() =>
            {
                currentFrame++;
                var colorIndex = currentFrame % alertColorSequence.Length;
                var textIndex = currentFrame % alertTextSequence.Length;

                SecondaryAlertWindowElement.Background = alertColorSequence[colorIndex].Item1;
                AlertText.Foreground = alertColorSequence[colorIndex].Item2;

                AlertText.Text = alertTextSequence[textIndex];
            });

            m_StrobeTimer.Start(); 
        }

        public void CancelAlert()
        {
            m_StrobeTimer.Stop();
            
            SecondaryAlertWindowElement.Background = m_CancelledColors.Item1;
            AlertText.Foreground = m_CancelledColors.Item2;
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
    }
}
