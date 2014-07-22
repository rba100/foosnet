using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FoosNet.Controls
{
    public class Spinner : FrameworkElement
    {
        private const int c_Blobs = 15;
        private readonly Brush[] m_Brushes = new Brush[c_Blobs];
        private bool m_IsAnimating = false;
        private readonly DoubleAnimationUsingKeyFrames m_Animation;

        public Spinner()
        {
            var duration = TimeSpan.FromSeconds(1.3);

            m_Animation = new DoubleAnimationUsingKeyFrames()
            {
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = duration
            };

            for (int i = 0; i < c_Blobs; i++)
            {
                m_Animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(i * 360.0 / c_Blobs, KeyTime.Paced));
            }

            var rotateTransform = new RotateTransform();
            RenderTransform = rotateTransform;

            for (int i = 0; i < c_Blobs; i++)
            {
                var r = (byte)(0xFF - (((0xFF - 0x17) / c_Blobs) * i));
                var g = (byte)(0xFF - (((0xFF - 0x7F) / c_Blobs) * i));
                var b = (byte)(0xFF - (((0xFF - 0x2E) / c_Blobs) * i));

                m_Brushes[i] = new SolidColorBrush(Color.FromRgb(r, g, b));
                m_Brushes[i].Freeze();
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var renderTransform = RenderTransform as RotateTransform;
            if (renderTransform != null)
            {
                renderTransform.CenterX = finalSize.Width / 2;
                renderTransform.CenterY = finalSize.Height / 2;
            }
            return base.ArrangeOverride(finalSize);
        }
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            AnimationControl();
            var width = Width < 1 ? 1 : Width;
            drawingContext.PushTransform(new TranslateTransform(RenderSize.Width / 2, RenderSize.Height / 2));

            for (int i = 0; i < c_Blobs; i++)
            {
                drawingContext.PushTransform(new RotateTransform((360 / c_Blobs) * i));
                drawingContext.DrawRectangle(m_Brushes[i], null, new Rect(0, width / 4.25, width / 16.0, width / 4.0));
                drawingContext.Pop();
            }
            drawingContext.Pop();
        }

        /// <summary>
        /// Not sure if this actually saves CPU, but there could be lots of
        /// these things animating and hidden if they're used per row of a large dataset
        /// </summary>
        private void AnimationControl()
        {
            var rotateTransform = (RenderTransform as RotateTransform);
            if (rotateTransform == null) return;
            if (Visibility == Visibility.Visible && !m_IsAnimating)
            {
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, m_Animation);
                m_IsAnimating = true;
            }
            else if (Visibility != Visibility.Visible && m_IsAnimating)
            {
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, null);
                m_IsAnimating = false;
            }
        }
    }
}
