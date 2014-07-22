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
            (Tuple<SolidColorBrush, SolidColorBrush> [] alertColors,
             Tuple<SolidColorBrush, SolidColorBrush> cancelledColors)
        {
            m_CancelledColors = cancelledColors;
            InitializeComponent();

            m_StrobeTimer = new Timer {Interval = 1000};

            var currentColour = 0;
            
            SecondaryAlertWindowElement.Background = alertColors[currentColour].Item1;
            AlertText.Foreground = alertColors[currentColour].Item2;

            m_StrobeTimer.Elapsed += (sender, elapsedEventArgs) => Dispatcher.Invoke(() =>
            {
                currentColour = (currentColour + 1) % alertColors.Length;
                SecondaryAlertWindowElement.Background = alertColors[currentColour].Item1;
                AlertText.Foreground = alertColors[currentColour].Item2;
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
