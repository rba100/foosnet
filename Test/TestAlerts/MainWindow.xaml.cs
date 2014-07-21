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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

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

            var screenCount = Screen.AllScreens.Count();

            foreach (var screen in Screen.AllScreens)
            {
                var window = new AlertWindow
                {
                    Top = screen.WorkingArea.Top,
                    Left = screen.WorkingArea.Left,
                    Width = screen.WorkingArea.Width,
                    Height = screen.WorkingArea.Height
                };
                window.Show();
                window.Activate();
                window.WindowState = WindowState.Maximized;
            }
            
            TestWindow.Hide();

        }
    }
}
