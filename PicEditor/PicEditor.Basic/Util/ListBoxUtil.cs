using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PicEditor.Basic.Util
{
    public class ListBoxUtil : DependencyObject
    {
        public enum DragDirection
        {
            Front,
            Back,
        }

        public class DragResult
        {
            public object? Source { get; set; } = null;

            public object? Target { get; set; } = null;

            public DragDirection Direction { get; set; }
        }

        public static void SetIsDragEnable(DependencyObject obj, bool value) => obj.SetValue(IsDragEnableProperty, value);

        public static DragResult GetDrag(DependencyObject obj) => (DragResult)obj.GetValue(DragProperty);

        public static void SetDrag(DependencyObject obj, DragResult value) => obj.SetValue(DragProperty, value);

        private static void IsDragEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isDragEnable)
            {
                if (isDragEnable)
                {
                    if (listBoxEvent == null && d is ListBox listBox && listBox != null)
                    {
                        listBoxEvent = new ListBoxEvent(listBox);
                        listBoxEvent.Drag += (dragResult) =>
                        {
                            SetDrag(listBox, dragResult);
                        };
                    }
                }
            }
        }

        private static ListBoxEvent? listBoxEvent = null;

        private class ListBoxEvent
        {
            public event Action<DragResult>? Drag = null;

            public ListBoxEvent(ListBox listBox)
            {
                this.listBox = listBox;
                if (listBox.IsLoaded)
                {
                    InitListBoxEvents();
                }
                this.listBox.Loaded += ListBox_Loaded;
            }

            private void ListBox_Loaded(object sender, RoutedEventArgs e)
            {
                listBox.Loaded -= ListBox_Loaded;
                InitListBoxEvents();
            }

            private void InitListBoxEvents()
            {
                listBox.PreviewMouseLeftButtonDown += ListBoxEx_MouseLeftButtonDown;
                listBox.MouseMove += ListBoxEx_MouseMove;
                listBox.GiveFeedback += ListBoxEx_GiveFeedback;
                listBox.QueryContinueDrag += ListBoxEx_QueryContinueDrag;

                listBox.DragEnter += ListBoxEx_DragEnter;
                listBox.DragLeave += ListBoxEx_DragLeave;
                listBox.DragOver += ListBoxEx_DragOver;

                listBox.Drop += ListBoxEx_Drop;
            }

            private void ListBoxEx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                isPressed = true;
            }

            private void ListBoxEx_MouseMove(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed && isPressed && sourceItem == null)
                {
                    UIElement element = (UIElement)listBox.InputHitTest(e.GetPosition(listBox));
                    sourceItem = (ListBoxItem)listBox.ContainerFromElement(element);
                    if (sourceItem != null)
                    {
                        startPoint = e.GetPosition(sourceItem);

                        dragThumbnail.PlacementTarget = sourceItem;
                        dragThumbnail.SetContent(new VisualBrush(sourceItem), sourceItem.ActualWidth, sourceItem.ActualHeight, startPoint);
                        dragThumbnail.IsOpen = true;

                        var dragData = new DataObject(this);
                        var effects = DragDrop.DoDragDrop(sourceItem, dragData, DragDropEffects.Move);
                        if (effects == DragDropEffects.Move)
                        {
                            if (!sourceItem.Equals(targetItem) && targetItem != null)
                            {
                                Drag?.Invoke(new DragResult
                                {
                                    Source = sourceItem.Content,
                                    Target = targetItem.Content,
                                    Direction = dragDirection
                                });
                            }
                        }
                        dragThumbnail.IsOpen = false;
                        sourceItem = targetItem = null;
                    }
                }
            }

            private void ListBoxEx_GiveFeedback(object sender, GiveFeedbackEventArgs e)
            {
                Mouse.SetCursor(Cursors.Arrow);
                e.UseDefaultCursors = false;
                e.Handled = true;
            }

            private void ListBoxEx_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
            {
                if (e.EscapePressed)
                {
                    e.Action = DragAction.Cancel;
                    isPressed = false;
                }
                else if (dragThumbnail != null && sourceItem != null)
                {
                    dragThumbnail.SetPosition(sourceItem.PointFromScreen(Win32Util.GetMousePosition()));
                }
            }

            private void ListBoxEx_DragEnter(object sender, DragEventArgs e)
            {
                ListBoxEx_DragOver(sender, e);
            }

            private void ListBoxEx_DragLeave(object sender, DragEventArgs e)
            {
                DragAdorner.ClearAdornerLayer(lastAdorneredItem);
            }

            private void ListBoxEx_DragOver(object sender, DragEventArgs e)
            {
                Point point = e.GetPosition(listBox);
                UIElement element = (UIElement)listBox.InputHitTest(point);
                if (element != null)
                {
                    ListBoxItem item = (ListBoxItem)listBox.ContainerFromElement(element);
                    if (item != null)
                    {
                        DragAdorner.ClearAdornerLayer(lastAdorneredItem);
                        if (!item.Equals(lastAdorneredItem))
                        {
                            lastAdorneredItem = item;
                        }
                        DragDirection direction = e.GetPosition(item).Y < item.ActualHeight / 2 ? DragDirection.Front : DragDirection.Back;
                        AdornerLayer.GetAdornerLayer(item).Add(new DragAdorner(item, direction));

                        if (point.Y < item.ActualHeight / 2)
                        {
                            int index = listBox.Items.IndexOf(item.Content);
                            if (index > 0)
                            {
                                listBox.ScrollIntoView(listBox.Items[index - 1]);
                            }
                        }
                        else if (listBox.ActualHeight - point.Y < item.ActualHeight / 2)
                        {
                            int index = listBox.Items.IndexOf(item.Content);
                            if (index + 1 < listBox.Items.Count)
                            {
                                listBox.ScrollIntoView(listBox.Items[index + 1]);
                            }
                        }
                    }
                }
            }

            private void ListBoxEx_Drop(object sender, DragEventArgs e)
            {
                DragAdorner.ClearAdornerLayer(lastAdorneredItem);
                UIElement element = (UIElement)listBox.InputHitTest(e.GetPosition(listBox));
                if (element != null)
                {
                    targetItem = (ListBoxItem)listBox.ContainerFromElement(element);
                    if (targetItem != null)
                    {
                        dragDirection = e.GetPosition(targetItem).Y < targetItem.ActualHeight / 2 ? DragDirection.Front : DragDirection.Back;
                    }
                }
            }

            private readonly ListBox listBox;
            private readonly DragThumbnail dragThumbnail = new();
            private ListBoxItem? sourceItem = null;
            private ListBoxItem? targetItem = null;
            private ListBoxItem? lastAdorneredItem = null;
            private DragDirection dragDirection = DragDirection.Front;
            private Point startPoint = new(0, 0);
            private bool isPressed = false;
        }

        private class DragThumbnail : Popup
        {
            public DragThumbnail()
            {
                Child = content = new Border { Opacity = 0.5 };
                Placement = PlacementMode.Top;
                PopupAnimation = PopupAnimation.None;
                StaysOpen = true;
                AllowsTransparency = true;
            }

            public void SetContent(VisualBrush contentVisual, double contentWidth, double contentHeight, Point relativeContentPosition)
            {
                content.Background = contentVisual;
                Width = contentWidth;
                Height = contentHeight;
                HorizontalOffset = 0;
                VerticalOffset = contentHeight + relativeContentPosition.Y + SystemParameters.MinimumVerticalDragDistance;
                startPoint = relativeContentPosition;
            }

            public void SetPosition(Point mousePosition)
            {
                HorizontalOffset = mousePosition.X - startPoint.X;
                VerticalOffset = Height + mousePosition.Y + SystemParameters.MinimumVerticalDragDistance;
            }

            private readonly Border content;
            private Point startPoint;
        }

        public class DragAdorner : Adorner
        {
            public DragAdorner(UIElement adornedElement, DragDirection dragDirection) : base(adornedElement)
            {
                this.dragDirection = dragDirection;
                IsHitTestVisible = false;
            }

            public static void ClearAdornerLayer(UIElement? element)
            {
                if (element != null)
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
                    Adorner[]? adorners = adornerLayer.GetAdorners(element);
                    if (adorners != null)
                    {
                        for (int i = 0; i < adorners.Length; ++i)
                        {
                            adornerLayer.Remove(adorners[i]);
                        }
                    }
                }
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                if (frontGeometry == null && dragDirection == DragDirection.Front)
                {
                    var size = AdornedElement.RenderSize;
                    frontGeometry = Geometry.Parse(
                        $"M0,-2 L10,-2 10,2 0,2Z " +
                        $"M{size.Width - 10},-2 L{size.Width},-2 {size.Width},2 {size.Width - 10},2Z " +
                        $"M10,-1 L{size.Width - 10},-1 {size.Width - 10},1 10,1Z");
                }
                if (backGeometry == null && dragDirection == DragDirection.Back)
                {
                    var size = AdornedElement.RenderSize;
                    backGeometry = Geometry.Parse(
                        $"M0,{size.Height - 2} L10,{size.Height - 2} 10,{size.Height + 2} 0,{size.Height + 2}Z " +
                        $"M{size.Width - 10},{size.Height - 2} L{size.Width},{size.Height - 2} {size.Width},{size.Height + 2} {size.Width - 10},{size.Height + 2}Z " +
                        $"M10,{size.Height - 1} L{size.Width - 10},{size.Height - 1} {size.Width - 10},{size.Height + 1} 10,{size.Height + 1}");
                }
                drawingContext.DrawGeometry(renderBrush, null, dragDirection == DragDirection.Front ? frontGeometry : backGeometry);
            }

            private readonly DragDirection dragDirection;
            private static readonly SolidColorBrush renderBrush = new(Colors.Orange);
            private static Geometry? frontGeometry = null;
            private static Geometry? backGeometry = null;
        }

        private class Win32Util
        {
            public static Point GetMousePosition()
            {
                Win32Point point = new();
                GetCursorPos(ref point);
                return new Point(point.X, point.Y);
            }

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetCursorPos(ref Win32Point pt);

            [StructLayout(LayoutKind.Sequential)]
            private struct Win32Point
            {
                public int X;
                public int Y;
            }
        }

        public static readonly DependencyProperty IsDragEnableProperty = DependencyProperty.RegisterAttached("IsDragEnable", typeof(bool), typeof(ListBoxUtil), new PropertyMetadata(false, IsDragEnableChanged));
        public static readonly DependencyProperty DragProperty = DependencyProperty.RegisterAttached("Drag", typeof(DragResult), typeof(ListBoxUtil), new PropertyMetadata(null));
    }
}
