using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MoxibustionBedAPP.Models
{
    public class SliderTouchAreaAdorner:Adorner
    {
        private readonly Slider _slider;
        private readonly VisualBrush _visualBrush;
        private readonly RectangleGeometry _touchAreaGeometry;

        public SliderTouchAreaAdorner(Slider slider) : base(slider)
        {
            _slider = slider;
            _visualBrush = new VisualBrush(slider);
            _touchAreaGeometry = new RectangleGeometry(new Rect(0, 0, 150, 150));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _slider.RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
            {
                RoutedEvent = UIElement.MouseDownEvent,
                Source = _slider
            });
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _slider.RaiseEvent(new MouseEventArgs(e.MouseDevice, e.Timestamp)
            {
                RoutedEvent = UIElement.MouseMoveEvent,
                Source = _slider
            });
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            _slider.RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
            {
                RoutedEvent = UIElement.MouseUpEvent,
                Source = _slider
            });
            base.OnMouseUp(e);
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return _touchAreaGeometry;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _slider;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }
    }
}
