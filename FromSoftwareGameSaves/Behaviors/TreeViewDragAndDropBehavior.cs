using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using FromSoftwareGameSaves.ViewModel;
using FromSoftwareGameSaves.Visual;

namespace FromSoftwareGameSaves.Behaviors
{
    public class TreeViewDragAndDropBehavior<TDataItem> : Behavior<TreeView>
        where TDataItem : class
    {
        private Point _startPosition;
        private Point _endPosition;

        private bool _isDragging;
        private DragAdorner _dragAdorner;

        #region Properties

        public DataTemplate DragDataTemplate
        {
            get { return (DataTemplate)GetValue(DragDataTemplateProperty); }
            set { SetValue(DragDataTemplateProperty, value); }
        }

        public static readonly DependencyProperty DragDataTemplateProperty =
            DependencyProperty.Register(
                "DragDataTemplate",
                typeof(DataTemplate),
                typeof(TreeViewDragAndDropBehavior<TDataItem>),
                new UIPropertyMetadata(null));

        public ICommand DropCommand
        {
            get { return (ICommand) GetValue(DropCommandProperty); }
            set { SetValue(DropCommandProperty, value); }
        }

        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.Register(
                "DropCommand",
                typeof(ICommand),
                typeof(TreeViewDragAndDropBehavior<TDataItem>),
                new UIPropertyMetadata(null));

        #endregion // Properties

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AllowDrop = true;

            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.PreviewDragOver += OnPreviewDragOver;
            AssociatedObject.DragOver += OnDragOver;
            AssociatedObject.DragLeave += OnDragLeave;
            AssociatedObject.PreviewDragEnter += OnPreviewDragEnter;
            AssociatedObject.PreviewDrop += OnPreviewDrop;
            AssociatedObject.Drop += OnDrop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject == null)
                return;

            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= OnPreviewMouseMove;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.PreviewDragOver -= OnPreviewDragOver;
            AssociatedObject.DragOver -= OnDragOver;
            AssociatedObject.DragLeave -= OnDragLeave;
            AssociatedObject.PreviewDragEnter -= OnPreviewDragEnter;
            AssociatedObject.PreviewDrop -= OnPreviewDrop;
            AssociatedObject.Drop -= OnDrop;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var startPoint = mouseButtonEventArgs.GetPosition(AssociatedObject);
            if (GetItemAtLocation(startPoint) == null) return;

            _isDragging = true;
            _startPosition = startPoint;
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.LeftButton != MouseButtonState.Pressed) return;

            var currentLocation = mouseEventArgs.GetPosition(AssociatedObject);
            if (GetItemAtLocation(currentLocation) != null) return;

            _isDragging = false; // if item is null
        }

        private void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!_isDragging) return;
            if (mouseEventArgs.LeftButton != MouseButtonState.Pressed) return;
            if (AssociatedObject.SelectedValue == null) return;

            var currentLocation = mouseEventArgs.GetPosition(AssociatedObject);
            var delta = _startPosition - currentLocation;

            if (!IsDeltaMatchesDragDistance(delta)) return;
            if (GetItemAtLocation(currentLocation) == null) return;

            DragDrop.DoDragDrop(AssociatedObject, AssociatedObject.SelectedValue, DragDropEffects.Move);
        }

        private void OnPreviewDragOver(object sender, DragEventArgs dragEventArgs)
        {
            if (!dragEventArgs.Data.GetDataPresent(typeof(TDataItem))) return;

            _dragAdorner?.Update(dragEventArgs.GetPosition(AssociatedObject));
        }

        private static void OnDragOver(object sender, DragEventArgs dragEventArgs)
        {
            dragEventArgs.Effects = dragEventArgs.Data.GetDataPresent(typeof(TDataItem)) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnDragLeave(object sender, DragEventArgs dragEventArgs)
        {
            DisposeAdorner();
        }

        private void OnPreviewDragEnter(object sender, DragEventArgs dragEventArgs)
        {
            if (!dragEventArgs.Data.GetDataPresent(typeof(TDataItem))) return;

            var data = dragEventArgs.Data.GetData(typeof(TDataItem)) as TDataItem;

            InitializeAdornerTemplate(dragEventArgs, data);
        }

        private void OnPreviewDrop(object sender, DragEventArgs dragEventArgs)
        {
            DisposeAdorner();
        }

        private void OnDrop(object sender, DragEventArgs dragEventArgs)
        {
            if (!dragEventArgs.Data.GetDataPresent(typeof(TDataItem))) return;

            var inputElement = dragEventArgs.Source as IInputElement;
            if (inputElement == null)
                return;

            _endPosition = dragEventArgs.GetPosition(inputElement);
            var delta = _startPosition - _endPosition;

            if (!IsDeltaMatchesDragDistance(delta)) return;

            var source = dragEventArgs.Data.GetData(typeof(TDataItem)) as TDataItem;
            var target = GetItemAtLocation(_endPosition);

            if (source == null) return;
            if (target == null) return;
            if (source.Equals(target)) return;

            var dragDropInfoViewModel = new DragDropInfoViewModel<TDataItem>(source, target);
            DropCommand?.Execute(dragDropInfoViewModel);
        }

        private static bool IsDeltaMatchesDragDistance(Vector delta)
        {
            return Math.Abs(delta.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(delta.Y) > SystemParameters.MinimumVerticalDragDistance;
        }

        private void InitializeAdornerTemplate(DragEventArgs dragEventArgs, TDataItem data)
        {
            if (data == null) return;
            if (DragDataTemplate == null) return;
            if (_dragAdorner != null) return;

            var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);

            _dragAdorner = new DragAdorner(data, DragDataTemplate, AssociatedObject, adornerLayer);
            _dragAdorner?.Update(dragEventArgs.GetPosition(AssociatedObject));
        }

        private void DisposeAdorner()
        {
            _isDragging = false;
            _dragAdorner?.Dispose();
            _dragAdorner = null;
        }

        private TDataItem GetItemAtLocation(Point location)
        {
            var hitTestResults = VisualTreeHelper.HitTest(AssociatedObject, location);
            if (hitTestResults == null) return null;

            var frameworkElement = hitTestResults.VisualHit as FrameworkElement;
            var dataObject = frameworkElement?.DataContext;

            return dataObject as TDataItem;
        }
    }
}
