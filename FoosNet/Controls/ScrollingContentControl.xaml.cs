using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Threading;

namespace FoosNet.Controls
{
    // Public enum
    public enum ScrollDirection { Down, Up }

    /// <summary>
    /// Interaction logic for ScrollingContentControl.xaml
    /// </summary>
    public partial class ScrollingContentControl : UserControl
    {
        public static readonly DependencyProperty IsScrollingProperty = DependencyProperty.Register("IsScrolling",
            typeof(bool),
            typeof(ScrollingContentControl), new FrameworkPropertyMetadata(true, IsScrollingChanged));

        public static readonly DependencyProperty IsDraggableProperty = DependencyProperty.Register("IsDraggable",
            typeof(bool),
            typeof(ScrollingContentControl), new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty ScrollDirectionProperty =
            DependencyProperty.Register("ScrollDirection", typeof(ScrollDirection),
                typeof(ScrollingContentControl), new FrameworkPropertyMetadata(ScrollDirection.Down));

        public static readonly DependencyProperty TargetVelocityProperty = DependencyProperty.Register(
            "TargetVelocity", typeof(double),
            typeof(ScrollingContentControl), new FrameworkPropertyMetadata(0.5, TargetVelocitychanged));

        private ContentPresenter HostedContent;

        public double TargetVelocity
        {
            get { return (double)GetValue(TargetVelocityProperty); }
            set { SetValue(TargetVelocityProperty, value); }
        }

        public bool IsScrolling
        {
            get { return (bool)GetValue(IsScrollingProperty); }
            set { SetValue(IsScrollingProperty, value); }
        }

        public bool IsDraggable
        {
            get { return (bool)GetValue(IsDraggableProperty); }
            set { SetValue(IsDraggableProperty, value); }
        }

        public ScrollDirection ScrollDirection
        {
            get { return (ScrollDirection)GetValue(ScrollDirectionProperty); }
            set { SetValue(ScrollDirectionProperty, value); }
        }

        // Private members
        private readonly TimeSpan m_ScrollInterval = TimeSpan.FromSeconds(1.0 / 60);
        private readonly TimeSpan m_MinInterval = TimeSpan.FromSeconds(1.0 / 60);

        private DispatcherTimer m_Timer;
        private double m_ContentOffset = 20;

        private bool m_IsDragging;
        private Point m_LastDragPoint;
        private double m_Velocity = 0.5;
        private double m_PixelsPerInterval = 1;
        private readonly Stopwatch m_DragTimer = new Stopwatch();
        private bool m_HasDragged;
        private bool m_IsPaused;

        public ScrollingContentControl()
        {
            InitializeComponent();
            m_Timer = new DispatcherTimer(m_ScrollInterval, DispatcherPriority.Normal, Scroll, Dispatcher.CurrentDispatcher);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var content = GetTemplateChild("HostedContent");
            HostedContent = content as ContentPresenter;
            if (HostedContent != null)
            {
                Canvas.SetLeft(HostedContent, 0);
                Canvas.SetTop(HostedContent, 0);
            }
        }

        private static void IsScrollingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var here = sender as ScrollingContentControl;
            if (here == null) return;
            if ((bool)e.NewValue) here.m_Timer.Start();
            else here.m_Timer.Stop();
        }

        private static void TargetVelocitychanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var here = sender as ScrollingContentControl;
            if (here == null) return;
            here.m_Velocity = (double) e.NewValue;
        }

        private void Scroll(object sender, EventArgs e)
        {
            if (HostedContent == null || m_IsDragging || m_IsPaused) return;

            LevelOutVelocity();
            SetScrollParameters();

            if (ScrollDirection == ScrollDirection.Down) m_ContentOffset += m_PixelsPerInterval;
            else m_ContentOffset -= m_PixelsPerInterval;

            if (m_ContentOffset > ActualHeight) m_ContentOffset = -HostedContent.ActualHeight;
            else if (m_ContentOffset + HostedContent.ActualHeight < 0) m_ContentOffset = ActualHeight;

            Canvas.SetTop(HostedContent, m_ContentOffset);
        }

        private void MainCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var w = e.NewSize.Width;
            if (HostedContent != null)
            {
                HostedContent.Width = w >= 0 ? w : 0;
                var userContent = HostedContent.Content as FrameworkElement;
                if (userContent != null) userContent.Width = HostedContent.Width;
            }
        }

        private void MainCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsDraggable)
            {
                m_DragTimer.Reset();
                m_IsDragging = true;
                m_HasDragged = false;
                m_LastDragPoint = e.GetPosition(this);
                m_Velocity = TargetVelocity;
            }
        }

        private void MainCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_IsDragging && HostedContent != null)
            {
                m_DragTimer.Restart();
                m_HasDragged = true;
                var oldYpos = m_LastDragPoint.Y;
                m_LastDragPoint = e.GetPosition(this);
                var yPos = m_LastDragPoint.Y;
                var change = yPos - oldYpos;
                ScrollDirection = change < 0 ? ScrollDirection.Up : ScrollDirection.Down;
                m_Velocity = Math.Abs(change);
                m_ContentOffset += change;
                Canvas.SetTop(HostedContent, m_ContentOffset);
            }
        }

        private void MainCanvas_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (m_IsDragging)
            {
                m_IsDragging = false;
                m_IsPaused = !m_HasDragged;
            }
        }

        private void MainCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_IsDragging)
            {
                m_IsDragging = false;
                if (!m_HasDragged) m_IsPaused = !m_IsPaused;
                else m_IsPaused = m_DragTimer.ElapsedMilliseconds > 250;
                m_DragTimer.Stop();
            }
        }

        private void LevelOutVelocity()
        {
            var target = TargetVelocity;
            var diff = target - m_Velocity;
            if (Math.Abs(diff) < 0.1) m_Velocity = target;
            else
            {
                m_Velocity += diff / 25;
            }
        }

        private void SetScrollParameters()
        {
            if (m_Velocity < 1)
            {
                m_PixelsPerInterval = 1;
                var newInterval = TimeSpan.FromTicks(Convert.ToInt64(1.0/m_Velocity*m_MinInterval.Ticks));
                if (m_Timer.Interval != newInterval) m_Timer.Interval = newInterval;
            }
            else
            {
                m_PixelsPerInterval = m_Velocity;
                if (m_Timer.Interval != m_MinInterval) m_Timer.Interval = m_MinInterval;
            }
        }
    }
}
