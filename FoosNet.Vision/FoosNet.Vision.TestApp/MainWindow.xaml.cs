using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FoosNet.Vision.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer m_ImageTimer;
        private ITableWatcher m_TableWatcher;

        private void UpdateImage(object state)
        {
            Dispatcher.Invoke(() => ImageCam.Source = Utils.ToBitmapSource(m_TableWatcher.LatestImage));
        }

        public MainWindow()
        {
            InitializeComponent();

            m_ImageTimer = new Timer(UpdateImage, null, 2000, 2000);

            m_TableWatcher = new Test.TableWatcherMoving();
            m_TableWatcher.TableHasBecomeFree += delegate { Dispatcher.Invoke(() => LabelTableStatus.Content = "Table is free"); };
            m_TableWatcher.TableHasBecomeInUse += delegate { Dispatcher.Invoke(() => LabelTableStatus.Content = "Table is in use"); };
        }

    }
}
