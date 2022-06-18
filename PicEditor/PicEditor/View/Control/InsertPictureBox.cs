using PicEditor.Model.PictureInfo;
using PicEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PicEditor.View.Control
{
    internal class InsertPictureBox : RectSelectorBase
    {
        public double WhRatio => (double)GetValue(WhRatioProperty);

        public bool IsKeepRatio => (bool)GetValue(IsKeepRatioProperty);

        public ICommand Rotate
        {
            get => (ICommand)GetValue(RotateProperty);
            set => SetValue(RotateProperty, value);
        }

        public ICommand Mirror
        {
            get => (ICommand)GetValue(MirrorProperty);
            set => SetValue(MirrorProperty, value);
        }

        public ICommand Align
        {
            get => (ICommand)GetValue(AlignProperty);
            set => SetValue(AlignProperty, value);
        }

        public InsertPictureBox()
        {
            rotateLeftMenu = new MenuItem { Header = "向左旋转90度", CommandParameter = PictureRotate.RotateLeft };
            rotateRightMenu = new MenuItem { Header = "向右旋转90度", CommandParameter = PictureRotate.RotateRight };
            mirrorHorizontalMenu = new MenuItem { Header = "水平镜像", CommandParameter = PictureMirror.MirrorHorizontal };
            mirrorVerticalMenu = new MenuItem { Header = "垂直镜像", CommandParameter = PictureMirror.MirrorVertical };
            alignLeftMenu = new MenuItem { Header = "左对齐", CommandParameter = PictureAlign.AlignLeft };
            alignRightMenu = new MenuItem { Header = "右对齐", CommandParameter = PictureAlign.AlignRight };
            alignTopMenu = new MenuItem { Header = "上对齐", CommandParameter = PictureAlign.AlignTop };
            alignBottomMenu = new MenuItem { Header = "下对齐", CommandParameter = PictureAlign.AlignBottom };
            alignHorizontalCenterMenu = new MenuItem { Header = "水平居中", CommandParameter = PictureAlign.AlignHorizontalCenter };
            alignVerticalCenterMenu = new MenuItem { Header = "垂直居中", CommandParameter = PictureAlign.AlignVerticalCenter };
            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(rotateLeftMenu);
            ContextMenu.Items.Add(rotateRightMenu);
            ContextMenu.Items.Add(mirrorHorizontalMenu);
            ContextMenu.Items.Add(mirrorVerticalMenu);
            ContextMenu.Items.Add(alignLeftMenu);
            ContextMenu.Items.Add(alignRightMenu);
            ContextMenu.Items.Add(alignTopMenu);
            ContextMenu.Items.Add(alignBottomMenu);
            ContextMenu.Items.Add(alignHorizontalCenterMenu);
            ContextMenu.Items.Add(alignVerticalCenterMenu);
        }

        private bool keptRatio = true; // 是否已经保持了宽高比例
        private readonly MenuItem rotateLeftMenu;
        private readonly MenuItem rotateRightMenu;
        private readonly MenuItem mirrorHorizontalMenu;
        private readonly MenuItem mirrorVerticalMenu;
        private readonly MenuItem alignLeftMenu;
        private readonly MenuItem alignRightMenu;
        private readonly MenuItem alignTopMenu;
        private readonly MenuItem alignBottomMenu;
        private readonly MenuItem alignHorizontalCenterMenu;
        private readonly MenuItem alignVerticalCenterMenu;

        protected override void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPressed)
            {
                Point p = e.GetPosition(parent);
                if (mode == Modes.Default)
                {
                    RealLeft = (point.X + p.X - startPoint.X) / Scale;
                    RealTop = (point.Y + p.Y - startPoint.Y) / Scale;
                }
                else
                {
                    double left = RealLeft, top = RealTop, width = RealWidth, height = RealHeight;
                    if ((mode & Modes.N) == Modes.N)
                    {
                        top = (p.Y >= point.Y - 1 ? point.Y - 1 : p.Y) / Scale;
                        height = (p.Y >= point.Y - 1 ? 1 : point.Y - p.Y) / Scale;
                    }
                    else if ((mode & Modes.S) == Modes.S)
                    {
                        height = (p.Y - 1 <= point.Y ? 1 : p.Y - point.Y) / Scale;
                    }
                    if ((mode & Modes.W) == Modes.W)
                    {
                        left = (p.X >= point.X - 1 ? point.X - 1 : p.X) / Scale;
                        width = (p.X >= point.X - 1 ? 1 : point.X - p.X) / Scale;
                    }
                    else if ((mode & Modes.E) == Modes.E)
                    {
                        width = (p.X - 1 <= point.X ? 1 : p.X - point.X) / Scale;
                    }

                    if (IsKeepRatio)
                    {
                        if (mode == Modes.N || mode == Modes.S || mode == Modes.W || mode == Modes.E)
                        {
                            RealLeft = left;
                            RealTop = top;
                            if (mode == Modes.N || mode == Modes.S)
                            {
                                RealHeight = height;
                                RealWidth = height * WhRatio;
                            }
                            else if (mode == Modes.W || mode == Modes.E)
                            {
                                RealWidth = width;
                                RealHeight = width / WhRatio;
                            }
                        }
                        else
                        {
                            if (width / height > WhRatio)
                            {
                                RealTop = top;
                                RealHeight = height;
                                if (mode == (Modes.N | Modes.W) || mode == (Modes.S | Modes.W))
                                {
                                    RealLeft = point.X / Scale - height * WhRatio;
                                    RealWidth = height * WhRatio;
                                }
                                else if (mode == (Modes.N | Modes.E) || mode == (Modes.S | Modes.E))
                                {
                                    RealLeft = left;
                                    RealWidth = height * WhRatio;
                                }
                            }
                            else
                            {
                                RealLeft = left;
                                RealWidth = width;
                                if (mode == (Modes.N | Modes.W) || mode == (Modes.N | Modes.E))
                                {
                                    RealTop = point.Y / Scale - width / WhRatio;
                                    RealHeight = width / WhRatio;
                                }
                                else if (mode == (Modes.S | Modes.W) || mode == (Modes.S | Modes.E))
                                {
                                    RealTop = top;
                                    RealHeight = width / WhRatio;
                                }
                            }
                        }
                    }
                    else
                    {
                        RealLeft = left;
                        RealTop = top;
                        RealWidth = width;
                        RealHeight = height;
                    }
                    keptRatio = IsKeepRatio;
                }
            }
        }

        private static void IsKeepRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InsertPictureBox self && self != null && e.NewValue is bool b && b && !self.keptRatio)
            {
                if (self.RealWidth / self.RealHeight > self.WhRatio)
                {
                    self.RealWidth = self.RealHeight * self.WhRatio;
                }
                if (self.RealWidth / self.RealHeight < self.WhRatio)
                {
                    self.RealHeight = self.RealWidth / self.WhRatio;
                }
                self.keptRatio = true;
            }
        }

        private static void RotateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InsertPictureBox self && self != null)
            {
                ICommand command = (ICommand)e.NewValue;
                self.rotateLeftMenu.Command = command;
                self.rotateRightMenu.Command = command;
            }
        }

        private static void MirrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InsertPictureBox self && self != null)
            {
                ICommand command = (ICommand)e.NewValue;
                self.mirrorHorizontalMenu.Command = command;
                self.mirrorVerticalMenu.Command = command;
            }
        }

        private static void AlignChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InsertPictureBox self && self != null)
            {
                ICommand command = (ICommand)e.NewValue;
                self.alignLeftMenu.Command = command;
                self.alignRightMenu.Command = command;
                self.alignTopMenu.Command = command;
                self.alignBottomMenu.Command = command;
                self.alignHorizontalCenterMenu.Command = command;
                self.alignVerticalCenterMenu.Command = command;
            }
        }

        public static readonly DependencyProperty WhRatioProperty = DependencyProperty.Register("WhRatio", typeof(double), typeof(InsertPictureBox), new PropertyMetadata(0d));
        public static readonly DependencyProperty IsKeepRatioProperty = DependencyProperty.Register("IsKeepRatio", typeof(bool), typeof(InsertPictureBox), new PropertyMetadata(true, new PropertyChangedCallback(IsKeepRatioChanged)));
        public static readonly DependencyProperty RotateProperty = DependencyProperty.Register("Rotate", typeof(ICommand), typeof(InsertPictureBox), new PropertyMetadata(null, new PropertyChangedCallback(RotateChanged)));
        public static readonly DependencyProperty MirrorProperty = DependencyProperty.Register("Mirror", typeof(ICommand), typeof(InsertPictureBox), new PropertyMetadata(null, new PropertyChangedCallback(MirrorChanged)));
        public static readonly DependencyProperty AlignProperty = DependencyProperty.Register("Align", typeof(ICommand), typeof(InsertPictureBox), new PropertyMetadata(null, new PropertyChangedCallback(AlignChanged)));
    }
}
