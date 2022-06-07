using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PicEditor.Model.Layer;
using PicEditor.ViewModel;

namespace PicEditor.View.Control
{
    internal partial class TreeViewEx : TreeView
    {
        private Point pressedPoint = new();             // 鼠标点击的位置
        private Border? dragSource = null;              // 拖拽源的控件
        private LayerBase? source = null;               // 源对象
        private LayerBase? target = null;               // 目标对象
        private bool dragToTopmost = false;             // 是否拖拽到最顶层
        private readonly Brush dragSourceBackground;    // 拖拽源控件的背景色
        private readonly Brush dragTargetForeground;    // 拖拽目标控件的背景色
        private readonly Brush thisBackground;          // 此控件的背景色

        public TreeViewEx()
        {
            InitializeComponent();
            Background = thisBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333436"));
            dragSourceBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#131416"));
            dragTargetForeground = Brushes.RoyalBlue;
            AllowDrop = true;
        }


        private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border != null && border.Child is StackPanel sp && sp != null)
            {
                if (sp.Tag != null && sp.Tag is LayerBase layer)
                {
                    pressedPoint = e.GetPosition(this);
                    dragSource = border;
                    source = layer;
                    target = null;
                    dragToTopmost = false;
                }
            }
        }

        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(this);
                if (Math.Abs(point.X - pressedPoint.X) < 5 && Math.Abs(point.Y - pressedPoint.Y) < 5)
                {
                    return;
                }
                if (sender is Border border && border != null)
                {
                    border.Background = dragSourceBackground;
                    // 开始拖拽
                    DragDropEffects effects = DragDrop.DoDragDrop(border, string.Empty, DragDropEffects.Move);
                    // 处理拖拽结果
                    if (effects == DragDropEffects.Move)
                    {
                        if (DataContext is VmLayer vmLayer && vmLayer != null)
                        {
                            if (dragToTopmost && source != null)
                            {
                                vmLayer.Relocation(source);
                            }
                            if (source != null && target != null && !source.Equals(target))
                            {
                                vmLayer.Relocation(source, target);
                            }
                        }
                    }
                    else
                    {
                        border.Background = Brushes.Transparent;
                    }
                }
            }
        }

        private void Item_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is Border border && border != null && !border.Equals(dragSource))
            {
                border.Background = dragTargetForeground;
            }
            e.Handled = true;
        }

        private void Item_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Border border && border != null && !border.Equals(dragSource))
            {
                border.Background = Brushes.Transparent;
            }
            e.Handled = true;
        }

        private void Item_Drop(object sender, DragEventArgs e)
        {
            Item_DragLeave(sender, e);
            if (sender is Border border && border != null && border.Child is StackPanel sp1 && sp1 != null && sp1.Tag != null)
            {
                target = (LayerBase)sp1.Tag;
                border.Background = Brushes.Transparent;
            }
            else if (e.Source is StackPanel sp2 && sp2 != null && sp2.Tag != null)
            {
                target = (LayerBase)sp2.Tag;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            dragToTopmost = true;
            Background = thisBackground;
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            Background = dragTargetForeground;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            Background = thisBackground;
        }
    }
}
