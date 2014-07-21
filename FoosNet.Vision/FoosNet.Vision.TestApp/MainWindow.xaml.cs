using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();

            ITableWatcher tableWatcher = new Test.TableWatcherRandom();
            tableWatcher.TableHasBecomeFree += delegate { Dispatcher.Invoke(() => LabelTableStatus.Content = "Table is free"); };
            tableWatcher.TableHasBecomeInUse += delegate { Dispatcher.Invoke(() => LabelTableStatus.Content = "Table is in use"); };
        }
    }
}
