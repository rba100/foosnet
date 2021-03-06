﻿using System;
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
            if (m_TableWatcher.DebugImage != null)
                Dispatcher.Invoke(() => ImageCam.Source = Utils.ToBitmapSource(m_TableWatcher.DebugImage));
        }

        public MainWindow()
        {
            InitializeComponent();

            m_ImageTimer = new Timer(UpdateImage, null, 1000, 1000);

            m_TableWatcher = new TableWatcher();
            m_TableWatcher.TableNowFree += delegate { Dispatcher.Invoke(() => LabelTableStatus.Content = "Table is now free"); };
            m_TableWatcher.TableNowBusy += delegate { Dispatcher.Invoke(() => LabelTableStatus.Content = "Table is now busy"); };
        }

    }
}
