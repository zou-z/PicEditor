﻿using PicEditor.Basic.Util;
using PicEditor.Interface;
using PicEditor.Layer;
using PicEditor.Model.PictureData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.View.Control
{
    internal class ImageEx : InkCanvas, IPictureLayer
    {
        public Size CanvasSize
        {
            get => (Size)GetValue(CanvasSizeProperty);
            set => SetValue(CanvasSizeProperty, value);
        }

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public double RealLeft
        {
            get => (double)GetValue(RealLeftProperty);
            set => SetValue(RealLeftProperty, value);
        }

        public double RealTop
        {
            get => (double)GetValue(RealTopProperty);
            set => SetValue(RealTopProperty, value);
        }

        public double RealWidth
        {
            get => (double)GetValue(RealWidthProperty);
            set => SetValue(RealWidthProperty, value);
        }

        public double RealHeight
        {
            get => (double)GetValue(RealHeightProperty);
            set => SetValue(RealHeightProperty, value);
        }

        public PictureRotate Rotate
        {
            get => (PictureRotate)GetValue(RotateProperty);
            set => SetValue(RotateProperty, value);
        }

        public PictureMirror Mirror
        {
            get => (PictureMirror)GetValue(MirrorProperty);
            set => SetValue(MirrorProperty, value);
        }

        public ImageEx(string id, WriteableBitmap bitmap, bool isAutoScaleMode = true)
        {
            this.id = id;
            this.isAutoScaleMode = isAutoScaleMode;
            image = new Image { Source = bitmap, Stretch = Stretch.Fill };
            Children.Add(image);
            Background = Brushes.Transparent;
            EditingMode = InkCanvasEditingMode.None;
            if (this.isAutoScaleMode)
            {
                RealWidth = bitmap.PixelWidth;
                RealHeight = bitmap.PixelHeight;
            }
            else
            {
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            }
        }

        public string GetID()
        {
            return id;
        }

        public void SetVisible(Visibility visiblity)
        {
            Visibility = visiblity;
        }

        public VisualBrush GetVisualBrush()
        {
            return new VisualBrush { Visual = this };
        }

        public void SetAutoScaleMode()
        {
            isAutoScaleMode = true;
            AutoScaleMode();
        }
        
        // 应用移动和缩放
        public void ApplyMoveAndResize()
        {
            BitmapSource bitmapSource = BitmapUtil.Resize((BitmapSource)image.Source, (int)RealWidth, (int)RealHeight);
            WriteableBitmap bitmap = BitmapUtil.MoveAndResize((int)CanvasSize.Width, (int)CanvasSize.Height, new WriteableBitmap(bitmapSource), (int)RealLeft, (int)RealTop);
            image.Source = null;
            image.Source = bitmap;
            RealLeft = RealTop = 0;
            RealWidth = CanvasSize.Width;
            RealHeight = CanvasSize.Height;
        }
        
        // 应用画笔
        // 应用文本

        // 禁止点击左键时滚动条自动移动
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            e.Handled = true;
        }

        // 禁止点击右键时滚动条自动移动
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            e.Handled = true;
        }

        private void AutoScaleMode()
        {
            RenderOptions.SetBitmapScalingMode(image, Scale >= 4 ? BitmapScalingMode.NearestNeighbor : BitmapScalingMode.Linear);
        }

        private readonly string id = string.Empty;
        private readonly Image image;
        private bool isAutoScaleMode;

        private static void CanvasSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx self && self != null && self.Scale > 0)
            {
                Size size = (Size)e.NewValue;
                double width = size.Width * self.Scale;
                double height = size.Height * self.Scale;
                if (self.RealLeft != 0 || self.RealTop != 0 || !double.IsNaN(self.Width) && self.RealWidth != self.Width || !double.IsNaN(self.Height) && self.RealHeight != self.Height)
                {
                    LogUtil.Log.Error(new Exception("图片与画布尺寸不一致"), $"CanvasSize({self.Width},{self.Height}) ImagePosition({self.RealLeft},{self.RealTop},{self.Width},{self.Height})");
                }
                else
                {
                    self.RealWidth = width;
                    self.RealHeight = height;
                }
                self.Width = width;
                self.Height = height;
            }
        }

        private static void ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx self && self != null)
            {
                double scale = (double)e.NewValue;
                self.Width = self.CanvasSize.Width * scale;
                self.Height = self.CanvasSize.Height * scale;
                self.image.Width = self.RealWidth * scale;
                self.image.Height = self.RealHeight * scale;
                SetLeft(self.image, self.RealLeft * scale);
                SetTop(self.image, self.RealTop * scale);
                if (self.isAutoScaleMode)
                {
                    self.AutoScaleMode();
                }
            }
        }

        private static void RealLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx self && self != null && self.Scale > 0)
            {
                double realLeft = (double)e.NewValue;
                SetLeft(self.image, realLeft * self.Scale);
            }
        }

        private static void RealTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx self && self != null && self.Scale > 0)
            {
                double realTop = (double)e.NewValue;
                SetTop(self.image, realTop * self.Scale);
            }
        }

        private static void RealWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx self && self != null && self.Scale > 0)
            {
                double realWidth = (double)e.NewValue;
                self.image.Width = realWidth * self.Scale;
            }
        }

        private static void RealHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx self && self != null && self.Scale > 0)
            {
                double realHeight = (double)e.NewValue;
                self.image.Height = realHeight * self.Scale;
            }
        }

        private static void RotateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx self && self != null && e.NewValue is PictureRotate rotate && rotate != PictureRotate.None)
            {
                WriteableBitmap bitmap = (WriteableBitmap)self.image.Source;
                self.image.Source = null;
                bitmap = BitmapUtil.Rotate(bitmap, rotate == PictureRotate.RotateLeft ? -90 : 90);
                self.image.Source = bitmap;
                GC.Collect();
                self.Dispatcher.BeginInvoke(() =>
                {
                    self.Rotate = PictureRotate.None;
                });
            }
        }

        private static void MirrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx self && self != null && e.NewValue is PictureMirror mirror && mirror != PictureMirror.None)
            {
                WriteableBitmap bitmap = (WriteableBitmap)self.image.Source;
                self.image.Source = null;
                BitmapUtil.Mirror(bitmap, mirror == PictureMirror.MirrorHorizontal ? 1 : 2);
                self.image.Source = bitmap;
                GC.Collect();
                self.Dispatcher.BeginInvoke(() =>
                {
                    self.Mirror = PictureMirror.None;
                });
            }
        }

        public static readonly DependencyProperty CanvasSizeProperty = DependencyProperty.Register("CanvasSize", typeof(Size), typeof(ImageEx), new PropertyMetadata(new Size(0, 0), new PropertyChangedCallback(CanvasSizeChanged)));
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(ImageEx), new PropertyMetadata(0d, new PropertyChangedCallback(ScaleChanged)));
        public static readonly DependencyProperty RealLeftProperty = DependencyProperty.Register("RealLeft", typeof(double), typeof(ImageEx), new PropertyMetadata(0d, new PropertyChangedCallback(RealLeftChanged)));
        public static readonly DependencyProperty RealTopProperty = DependencyProperty.Register("RealTop", typeof(double), typeof(ImageEx), new PropertyMetadata(0d, new PropertyChangedCallback(RealTopChanged)));
        public static readonly DependencyProperty RealWidthProperty = DependencyProperty.Register("RealWidth", typeof(double), typeof(ImageEx), new PropertyMetadata(0d, new PropertyChangedCallback(RealWidthChanged)));
        public static readonly DependencyProperty RealHeightProperty = DependencyProperty.Register("RealHeight", typeof(double), typeof(ImageEx), new PropertyMetadata(0d, new PropertyChangedCallback(RealHeightChanged)));
        public static readonly DependencyProperty RotateProperty = DependencyProperty.Register("Rotate", typeof(PictureRotate), typeof(ImageEx), new PropertyMetadata(PictureRotate.None, new PropertyChangedCallback(RotateChanged)));
        public static readonly DependencyProperty MirrorProperty = DependencyProperty.Register("Mirror", typeof(PictureMirror), typeof(ImageEx), new PropertyMetadata(PictureMirror.None, new PropertyChangedCallback(MirrorChanged)));
    }
}
