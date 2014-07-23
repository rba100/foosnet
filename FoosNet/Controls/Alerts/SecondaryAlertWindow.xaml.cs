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
        private readonly Tuple<SolidColorBrush, SolidColorBrush>[] m_AlertColorSequence;
        private readonly string[] m_AlertTextSequence;
        private readonly Tuple<SolidColorBrush, SolidColorBrush> m_CancelledColors;
        private readonly Timer m_StrobeTimer;

        public SecondaryAlertWindow
            (Tuple<SolidColorBrush, SolidColorBrush> [] alertColorSequence,
             String [] alertTextSequence,
             Tuple<SolidColorBrush, SolidColorBrush> cancelledColors)
        {
            m_AlertColorSequence = alertColorSequence;
            m_AlertTextSequence = alertTextSequence;
            m_CancelledColors = cancelledColors;
            InitializeComponent();
            
            var random = new Random();

            m_StrobeTimer = new Timer {Interval = 1000 + random.Next(10)};

            var currentFrame = 1;
            
            SetStrobeFrame(currentFrame);

            m_StrobeTimer.Elapsed += (sender, elapsedEventArgs) => Dispatcher.Invoke(() =>
            {
                currentFrame++;
                Dispatcher.Invoke(() => SetStrobeFrame(currentFrame));
            });

            m_StrobeTimer.Start(); 
        }

        private void SetStrobeFrame(int currentFrame)
        {
                var colorIndex = currentFrame % m_AlertColorSequence.Length;
                var textIndex = currentFrame % m_AlertTextSequence.Length;

                SecondaryAlertWindowElement.Background = m_AlertColorSequence[colorIndex].Item1;
                AlertText.Foreground = m_AlertColorSequence[colorIndex].Item2;

                AlertText.Text = m_AlertTextSequence[textIndex];
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
