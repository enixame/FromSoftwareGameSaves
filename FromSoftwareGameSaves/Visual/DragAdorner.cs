using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FromSoftwareGameSaves.Visual
{
    public sealed class DragAdorner : Adorner, IDisposable
    {
        private readonly ContentPresenter _contentPresenter;
        private readonly AdornerLayer _adornerLayer;

        private double _leftOffset;
        private double _topOffset;

        public DragAdorner(object data, DataTemplate dataTemplate, UIElement adornedElement, AdornerLayer adornerLayer) 
            : base(adornedElement)
        {
            _adornerLayer = adornerLayer;
            _contentPresenter = new ContentPresenter { Content = data, ContentTemplate = dataTemplate, Opacity = 0.75 };

            _adornerLayer.Add(this);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _contentPresenter.Measure(constraint);
            return _contentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            return _contentPresenter;
        }

        protected override int VisualChildrenCount => 1;

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var generalTransformGroup = new GeneralTransformGroup();
            generalTransformGroup.Children.Add(base.GetDesiredTransform(transform));
            generalTransformGroup.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return generalTransformGroup;
        }

        public void Update(Point location)
        {
            _leftOffset = location.X;
            _topOffset = location.Y;
            _adornerLayer?.Update(AdornedElement);
        }

        public void Dispose()
        {
            _adornerLayer?.Remove(this);
        }
    }
}
