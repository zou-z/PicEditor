using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.IO;
using System.Windows.Threading;
using PicEditor.controller;
using PicEditor.window;
using PicEditor.core;
using IWshRuntimeLibrary;
using File = System.IO.File;
using System.Threading;
using System.Globalization;
using System.Security.RightsManagement;
using System.Security.Permissions;
using System.Drawing.Imaging;

namespace PicEditor
{
    public partial class MainWindow : Window
    {
        //基础变量
        private const int PicPadding = 150;
        private bool IsPressed = false;
        private Point p0;
        private string RectResizeMode = "NULL";
        public string StartupArgs = null;
        private string LeftPanelSelected = "查看";
        private BitmapSource SecPicture = null;
        public Setting setting = new Setting();
        //控件后台响应类
        private readonly PicControl picControl = null;
        private readonly SliderControl sliderControl = null;
        private readonly ListBoxControl listBoxControl = new ListBoxControl();
        private readonly TextControl textControl = new TextControl();
        private readonly PaintControl paintControl = new PaintControl();
        //事件
        private readonly MouseEventHandler look_MouseMove;
        private readonly MouseEventHandler rectangle_MouseMove;
        private readonly MouseButtonEventHandler rectangle_MouseLeftButtonUp;
        private readonly MouseEventHandler pixelposition_MouseMove;
        private readonly MouseButtonEventHandler pixelposition_MouseLeftButtonDown;
        private readonly MouseButtonEventHandler text_MouseLeftButtonDown;
        private readonly MouseEventHandler text_MouseMove;
        private readonly MouseButtonEventHandler fill_MouseLeftButtonDown;
        private readonly MouseEventHandler brush_MouseMove;
        private readonly MouseEventHandler ruler_MouseMove;

        public MainWindow()
        {
            InitializeComponent();
            picControl = new PicControl(Chart);
            sliderControl = new SliderControl(picControl,SViewer,PicPadding);
            //事件初始化
            look_MouseMove = new MouseEventHandler(Look_MouseMove);
            rectangle_MouseMove = new MouseEventHandler(Rectangle_MouseMove);
            rectangle_MouseLeftButtonUp = new MouseButtonEventHandler(Rectangle_MouseLeftButtonUp);
            pixelposition_MouseMove = new MouseEventHandler(PixelPosition_MouseMove);
            pixelposition_MouseLeftButtonDown = new MouseButtonEventHandler(PixelPosition_MouseLeftButtonDown);
            text_MouseLeftButtonDown = new MouseButtonEventHandler(Text_MouseLeftButtonDown);
            text_MouseMove = new MouseEventHandler(Text_MouseMove);
            fill_MouseLeftButtonDown = new MouseButtonEventHandler(Fill_MouseLeftButtonDown);
            brush_MouseMove = new MouseEventHandler(Brush_MouseMove);
            ruler_MouseMove = new MouseEventHandler(Ruler_MouseMove);
            setting.Get();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //数据绑定
            PicView.DataContext = LookRightPanel.DataContext = RectPositionPanel.DataContext = TitleText.DataContext = SecPicMsg.DataContext = RulerPanel.DataContext = Chart.DataContext = picControl;
            PicSlider.DataContext = PicSliderText.DataContext = sliderControl;
            HistoryPanel.DataContext = listBoxControl;
            PicTextBorder.DataContext = PicTextPanel.DataContext = textControl;
            inkCanvas.DataContext = PaintPanel.DataContext = paintControl;
            //添加事件
            PicLook.IsChecked = true;
            PicView.AddHandler(MouseMoveEvent, look_MouseMove);
            //处理本程序的启动参数
            if (StartupArgs != null && File.Exists(StartupArgs))
            {
                PicProcess picProcess = new PicProcess();
                string name = StartupArgs.Substring(StartupArgs.LastIndexOf('\\') + 1, StartupArgs.Length - StartupArgs.LastIndexOf('\\') - 1);
                if (!listBoxControl.AddHistory(picProcess.OpenLocalPicture(StartupArgs), "从本地打开图片", name, StartupArgs))
                {
                    MsgWindow mw = new MsgWindow("打开出错", "此文件不是图片!", "Confirm") { Owner = this };
                    mw.ShowDialog();
                    return;
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    if (setting.FitPicSize)
                    {
                        if (picControl.Pic == null)
                            return;
                        Width = setting.HideSidePanel ? picControl.Pic.PixelWidth + 14 : picControl.Pic.PixelWidth + 324;
                        Height = picControl.Pic.PixelHeight + 76;
                    }
                    sliderControl.SliderValue = -0.098249;
                    SViewer.ScrollToHorizontalOffset((picControl.Width - SViewer.ActualWidth) / 2 + PicPadding);
                    SViewer.ScrollToVerticalOffset((picControl.Height - SViewer.ActualHeight) / 2 + PicPadding);
                    picControl.LastPic = picControl.Pic;
                    PicLook.IsChecked = true;
                }));
            }
            if (setting.HideSidePanel)
            {
                LeftPanelWidth.Width = RightPanelWidth.Width = new GridLength(0);
                SizePanelSwitchButton.Content = "\xE011";
                SizePanelSwitchButton.ToolTip = "打开两侧面板";
            }
            else
            {
                LeftPanelWidth.Width = new GridLength(40);
                RightPanelWidth.Width = new GridLength(270);
                SizePanelSwitchButton.Content = "\xE010";
                SizePanelSwitchButton.ToolTip = "隐藏两侧面板";
            }
        }
        //自定义窗口相关的4个函数
        private void Window_Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Window_Maximize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        private void Window_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            WindowMaxButton.Content = WindowState == WindowState.Maximized ? "\xE923" : "\xE922";
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IfSavePicture();
        }
        /// <summary>
        /// 通过菜单处打开图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenPictureByMenu(object sender, RoutedEventArgs e)
        {
            IfSavePicture();
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "图像文件|*.jpg;*.png;*.jpeg;*.bmp;*.gif|Png图片|*.png|Jpeg图片|*.jpg;*.jpeg|Bmp图片|*.bmp|Gif图片|*.gif|所有文件|*.*"
            };
            if ((bool)ofd.ShowDialog())
            {
                PicProcess picProcess = new PicProcess();
                if(!listBoxControl.AddHistory(picProcess.OpenLocalPicture(ofd.FileName), "从本地打开图片", ofd.SafeFileName, ofd.FileName))
                {
                    MsgWindow mw = new MsgWindow("打开出错", "此文件不是图片!", "Confirm") { Owner = this };
                    mw.ShowDialog();
                    return;
                }
                AfterOpenPicture();
            }
        }
        /// <summary>
        /// 通过URL打开网络图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenPictureByURL(object sender,RoutedEventArgs e)
        {
            OpenPictureWindow opw = new OpenPictureWindow("OpenPictureByURL") { Owner = this };
            opw.ShowDialog();
        }
        public void OpenPictureByURL(string path)
        {
            IfSavePicture();
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.OpenWebPicture(path);
            if (bs == null)
            {
                OpenPictureWindow opw = new OpenPictureWindow("OpenPictureByURL") { Owner = this };
                opw.SetValue(path);
                opw.ShowDialog();
                return;
            }
            listBoxControl.AddHistory(bs, "从网络打开图片", path, path);
            AfterOpenPicture();
        }
        /// <summary>
        /// 通过拖拽打开图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenPictureByDragDrop(object sender, DragEventArgs e)
        {
            string path;
            PicProcess picProcess = new PicProcess();
            //从文件管理器拖拽图片到程序窗口
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0) as string;
                if (PicSynthesis.IsChecked == true && picControl.Pic != null)
                {
                    SecPicture = picProcess.OpenLocalPicture(path);
                    if (SecPicture == null)
                    {
                        MsgWindow mw = new MsgWindow("打开出错", "此文件不是图片!", "Confirm") { Owner = this };
                        mw.ShowDialog();
                    }
                    AfterAddSecPic();
                    return;
                }
                IfSavePicture();
                if (!listBoxControl.AddHistory(picProcess.OpenLocalPicture(path), "从本地打开图片", path.Substring(path.LastIndexOf('\\') + 1, path.Length - path.LastIndexOf('\\') - 1), path))
                {
                    MsgWindow mw = new MsgWindow("打开出错", "此文件不是图片!", "Confirm") { Owner = this };
                    mw.ShowDialog();
                    return;
                }
                AfterOpenPicture();
            }
            //从浏览器拖拽图片或推拽URL链接到程序窗口
            else if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                path = e.Data.GetData(DataFormats.StringFormat) as string;
                if (PicSynthesis.IsChecked == true && picControl.Pic != null)
                {
                    SecPicture = picProcess.OpenWebPicture(path);
                    if (SecPicture == null)
                    {
                        MsgWindow mw = new MsgWindow("打开出错", "此文件不是图片!", "Confirm") { Owner = this };
                        mw.ShowDialog();
                    }
                    AfterAddSecPic();
                    return;
                }
                IfSavePicture();
                if (!listBoxControl.AddHistory(picProcess.OpenWebPicture(path), "从网络打开图片", path, path))
                {
                    MsgWindow mw = new MsgWindow("打开出错", "此链接不是图片!", "Confirm") { Owner = this };
                    mw.ShowDialog();
                    return;
                }
                AfterOpenPicture();
            }
        }
        /// <summary>
        /// 通过剪切板打开图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenPictureByClipboard(object sender, RoutedEventArgs e)
        {
            IfSavePicture();
            BitmapSource bs = Clipboard.GetImage();
            if (!listBoxControl.AddHistory(bs, "从剪切板打开图片", "图片", "剪切板"))
            {
                MsgWindow mw = new MsgWindow("打开出错", "在剪切板里面没有图片!", "Confirm") { Owner = this };
                mw.ShowDialog();
                return;
            }
            AfterOpenPicture();
        }
        private void IfSavePicture()
        {
            if (picControl.LastPic == picControl.Pic)
                return;
            MsgWindow mw = new MsgWindow("保存", "是否保存当前图像？", "ConfirmCancel") { Owner = this };
            if (mw.ShowDialog() == true)
                SavePicture();
        }
        private void AfterOpenPicture()
        {
            sliderControl.SliderValue = -0.098249;
            SViewer.ScrollToHorizontalOffset((picControl.Width - SViewer.ActualWidth) / 2 + PicPadding);
            SViewer.ScrollToVerticalOffset((picControl.Height - SViewer.ActualHeight) / 2 + PicPadding);
            picControl.LastPic = picControl.Pic;
            PicLook.IsChecked = true;
        }
        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pic_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p1 = e.GetPosition(Pic);
            Point p2 = e.GetPosition(SViewer);
            sliderControl.SliderValue = sliderControl.GetScaleValue(e.Delta > 0 ? true : false);
            SViewer.ScrollToHorizontalOffset(p1.X * picControl.Scale - p2.X + PicPadding);
            SViewer.ScrollToVerticalOffset(p1.Y * picControl.Scale - p2.Y + PicPadding);
        }
        /// <summary>
        /// 左侧选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            switch (LeftPanelSelected)
            {
                case "查看":
                    PicView.RemoveHandler(MouseMoveEvent, look_MouseMove);
                    break;
                case "矩形":
                    PicView.RemoveHandler(MouseMoveEvent, rectangle_MouseMove);
                    PicView.RemoveHandler(MouseLeftButtonUpEvent, rectangle_MouseLeftButtonUp);
                    RectPositionTemp.Width = RectPositionTemp.Height = picControl.RectLeft = picControl.RectTop = picControl.RectWidth = picControl.RectHeight = 0;
                    break;
                case "吸管":
                    PicView.RemoveHandler(MouseMoveEvent, pixelposition_MouseMove);
                    PicView.RemoveHandler(MouseLeftButtonDownEvent, pixelposition_MouseLeftButtonDown);
                    PixelPosition.Width = PixelPosition.Height = 0;
                    Pic.Cursor = Cursors.Arrow;
                    break;
                case "文字":
                    PicView.RemoveHandler(MouseMoveEvent, text_MouseMove);
                    PicView.RemoveHandler(MouseLeftButtonDownEvent, text_MouseLeftButtonDown);
                    PicTextBorder.Visibility = Visibility.Collapsed;
                    break;
                case "填充":
                    PicView.RemoveHandler(MouseMoveEvent, pixelposition_MouseMove);
                    PicView.RemoveHandler(MouseLeftButtonDownEvent, fill_MouseLeftButtonDown);
                    PixelPosition.Width = PixelPosition.Height = 0;
                    Pic.Cursor = Cursors.Arrow;
                    break;
                case "画笔":
                    PicView.RemoveHandler(MouseMoveEvent, brush_MouseMove);
                    inkCanvas.Visibility = Visibility.Collapsed;
                    inkCanvas.Strokes.Clear();
                    break;
                case "图片合成":
                    RectPositionTemp.Width = RectPositionTemp.Height = picControl.RectLeft = picControl.RectTop = picControl.RectWidth = picControl.RectHeight = 0;
                    PicView.RemoveHandler(MouseMoveEvent, rectangle_MouseMove);
                    PicView.RemoveHandler(MouseLeftButtonUpEvent, rectangle_MouseLeftButtonUp);
                    SecPic.Source = null;
                    break;
                case "尺子":
                    PicView.RemoveHandler(MouseMoveEvent, ruler_MouseMove);
                    Ruler.Visibility = Visibility.Collapsed;
                    picControl.PointX1 = picControl.PointY1 = picControl.PointX2 = picControl.PointY2 = 0;
                    break;
            }
            if (picControl.Pic == null)
                return;
            switch ((sender as RadioButton).ToolTip.ToString())
            {
                case "查看":
                    PicView.AddHandler(MouseMoveEvent,look_MouseMove);
                    RightPanel.SelectedIndex = 0;LeftPanelSelected = "查看"; break;
                case "矩形":
                    PicView.AddHandler(MouseMoveEvent,rectangle_MouseMove);
                    PicView.AddHandler(MouseLeftButtonUpEvent, rectangle_MouseLeftButtonUp);
                    picControl.CanBeyondArea = false;
                    RightPanel.SelectedIndex = 1;LeftPanelSelected = "矩形"; break;
                case "吸管":
                    Pic.Cursor = Cursors.Cross;
                    PicView.AddHandler(MouseMoveEvent, pixelposition_MouseMove);
                    PicView.AddHandler(MouseLeftButtonDownEvent, pixelposition_MouseLeftButtonDown);
                    RightPanel.SelectedIndex = 2;LeftPanelSelected = "吸管"; break;
                case "文字":
                    PicView.AddHandler(MouseMoveEvent, text_MouseMove);
                    PicView.AddHandler(MouseLeftButtonDownEvent, text_MouseLeftButtonDown);
                    RightPanel.SelectedIndex = 3;LeftPanelSelected = "文字"; break;
                case "填充":
                    Pic.Cursor = Cursors.Cross;
                    PicView.AddHandler(MouseMoveEvent, pixelposition_MouseMove);
                    PicView.AddHandler(MouseLeftButtonDownEvent, fill_MouseLeftButtonDown);
                    RightPanel.SelectedIndex = 4;LeftPanelSelected = "填充"; break;
                case "画笔":
                    RectPositionTemp.Width = RectPositionTemp.Height = picControl.RectLeft = picControl.RectTop = picControl.RectWidth = picControl.RectHeight = 0;
                    PicView.AddHandler(MouseMoveEvent, brush_MouseMove);
                    inkCanvas.Visibility = Visibility.Visible;
                    RightPanel.SelectedIndex = 5;LeftPanelSelected = "画笔"; break;
                case "图片合成":
                    RectPositionTemp.Width = RectPositionTemp.Height = picControl.RectLeft = picControl.RectTop = picControl.RectWidth = picControl.RectHeight = 0;
                    PicView.AddHandler(MouseMoveEvent, rectangle_MouseMove);
                    PicView.AddHandler(MouseLeftButtonUpEvent, rectangle_MouseLeftButtonUp);
                    picControl.CanBeyondArea = true;
                    AddPicture.Visibility = Visibility;
                    SecPicMsg.Visibility = Visibility.Collapsed;
                    RightPanel.SelectedIndex = 6;LeftPanelSelected = "图片合成";break;
                case "尺子":
                    PicView.AddHandler(MouseMoveEvent, ruler_MouseMove);
                    RightPanel.SelectedIndex = 7; LeftPanelSelected = "尺子"; break;
            }
        }
        /// <summary>
        /// 移动图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Look_MouseMove(object sender,MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsPressed)
                {
                    Point p1 = e.GetPosition(SViewer);
                    if (p1 != p0)
                    {
                        SViewer.ScrollToHorizontalOffset(SViewer.HorizontalOffset + (p0.X - p1.X));
                        SViewer.ScrollToVerticalOffset(SViewer.VerticalOffset + (p0.Y - p1.Y));
                        p0 = p1;
                    }
                }
                else
                {
                    p0 = e.GetPosition(SViewer);
                    IsPressed = true;
                }
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                if (IsPressed)
                    IsPressed = false;
            }
        }
        /// <summary>
        /// 红框确定单个像素的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PixelPosition_MouseMove(object sender,MouseEventArgs e)
        {
            Point p1 = e.GetPosition(Pic);
            if (p1.X <= 0 || p1.Y <= 0 || p1.X >= Pic.ActualWidth || p1.Y >= Pic.ActualHeight)
            {
                PixelPosition.Width = PixelPosition.Height = 0;
                return;
            }
            Canvas.SetLeft(PixelPosition, (int)p1.X);
            Canvas.SetTop(PixelPosition, (int)p1.Y);
            PixelPosition.Width = PixelPosition.Height = 1;
        }
        private void PixelPosition_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p1 = e.GetPosition(Pic);
            PicProcess picProcess = new PicProcess();
            Color color = picProcess.GetPixelValue(picControl.Pic, (int)p1.X, (int)p1.Y);
            PicDropperColor.Fill = new SolidColorBrush(color);
            PicDropperR.Text = color.R.ToString();
            PicDropperG.Text = color.G.ToString();
            PicDropperB.Text = color.B.ToString();
            PicDropperA.Text = color.A.ToString();
            PicDropperHex.Text = picProcess.DecToHex(color.A) + picProcess.DecToHex(color.R) + picProcess.DecToHex(color.G) + picProcess.DecToHex(color.B);
        }
        private void SetFgColorByPicDropper(object sender, RoutedEventArgs e)
        {
            FgColor.Background = PicDropperColor.Fill;
        }
        //矩形选区
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsPressed)
                {
                    Point p1 = e.GetPosition(Pic);
                    if (RectResizeMode == "NULL")
                    {
                        if (PicSynthesis.IsChecked == true)
                            return;
                        int left = (int)((p0.X < p1.X ? p0.X : p1.X) + 0.5), top = (int)((p0.Y < p1.Y ? p0.Y : p1.Y) + 0.5),
                            width = Math.Abs((int)(p0.X + 0.5) - (int)(p1.X + 0.5)), height = Math.Abs((int)(p0.Y + 0.5) - (int)(p1.Y + 0.5));
                        //分别是(1)选区在图片右边、下边、右下角外(2)选区在左边、上边、左上角外(3)选区尺寸为零
                        if (PicRectangle.IsChecked == true && left >= picControl.Width || top >= picControl.Height || left + width <= 0 || top + height <= 0 || width == 0 || height == 0)
                            return;
                        Canvas.SetLeft(RectPositionTemp, left < 0 ? 0 : left);
                        Canvas.SetTop(RectPositionTemp, top < 0 ? 0 : top);
                        RectPositionTemp.Width = left < 0 ? (left + width > picControl.Width ? picControl.Width : (left + width)) : (left + width > picControl.Width ? (picControl.Width - left) : width);
                        RectPositionTemp.Height = top < 0 ? (top + height > picControl.Height ? picControl.Height : (top + height)) : (top + height > picControl.Height ? (picControl.Height - top) : height);
                    }
                    else if (RectResizeMode == "MD")
                    {
                        picControl.RectLeft = (int)(p1.X - p0.X + 0.5);
                        picControl.RectTop = (int)(p1.Y - p0.Y + 0.5);
                        RectPosition.Cursor = Cursors.SizeAll;
                    }
                    else
                    {
                        int x = (int)(p1.X < 0 ? 0 : p1.X + 0.5), y = (int)(p1.Y < 0 ? 0 : p1.Y + 0.5);
                        if (RectResizeMode != "T" && RectResizeMode != "B")
                        {
                            picControl.RectLeft = x < p0.X ? x : p0.X;
                            picControl.RectWidth = Math.Abs(p0.X - x);
                        }
                        if (RectResizeMode != "L" && RectResizeMode != "R")
                        {
                            picControl.RectTop = y < p0.Y ? y : p0.Y;
                            picControl.RectHeight = Math.Abs(p0.Y - y);
                        }
                    }
                }
                else
                {
                    if (RectResizeMode == "NULL")
                        p0 = e.GetPosition(Pic);
                    else if (RectResizeMode == "MD")
                        p0 = e.GetPosition(RectPosition);
                    IsPressed = true;
                }
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                if (IsPressed)
                    Rectangle_Mouse();
            }
        }
        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsPressed)
                Rectangle_Mouse();
        }
        private void Rectangle_Mouse()
        {
            IsPressed = false;
            RectPosition.Cursor = Cursors.Arrow;
            if (RectResizeMode == "NULL")
            {
                if (RectPositionTemp.Width > 0 && RectPositionTemp.Height > 0)
                {
                    picControl.RectLeft = Canvas.GetLeft(RectPositionTemp);
                    picControl.RectTop = Canvas.GetTop(RectPositionTemp);
                    picControl.RectWidth = RectPositionTemp.ActualWidth;
                    picControl.RectHeight = RectPositionTemp.ActualHeight;
                    //必要重复执行，前两句可能会读取错误
                    picControl.RectLeft = Canvas.GetLeft(RectPositionTemp);
                    picControl.RectTop = Canvas.GetTop(RectPositionTemp);
                }
            }
            else
            {
                RectResizeMode = "NULL";
                Canvas.SetLeft(RectPositionTemp, picControl.RectLeft);
                Canvas.SetTop(RectPositionTemp, picControl.RectTop);
                RectPositionTemp.Width = picControl.RectWidth;
                RectPositionTemp.Height = picControl.RectHeight;
            }
        }
        private void GetRectResizeMode(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            RectResizeMode = border.Tag.ToString();
            if (RectResizeMode == "LT" || RectResizeMode == "L" || RectResizeMode == "T")
            {
                p0.X = picControl.RectLeft + picControl.RectWidth;
                p0.Y = picControl.RectTop + picControl.RectHeight;
            }
            else if (RectResizeMode == "RB" || RectResizeMode == "R" || RectResizeMode == "B")
            {
                p0.X = picControl.RectLeft;
                p0.Y = picControl.RectTop;
            }
            else if (RectResizeMode == "RT")
            {
                p0.X = picControl.RectLeft;
                p0.Y = picControl.RectTop + picControl.RectHeight;
            }
            else if (RectResizeMode == "LB")
            {
                p0.X = picControl.RectLeft + picControl.RectWidth;
                p0.Y = picControl.RectTop;
            }
        }
        /// <summary>
        /// 显示鼠标位置像素坐标(数值从0开始)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pic_MousePosition(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(Pic);
            MousePosition.Text = (int)p.X + " " + (int)p.Y;
        }
        /// <summary>
        /// 设置几个常用的Slider值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetScale(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            sliderControl.SliderValue = double.Parse(mi.Tag.ToString());
        }
        /// <summary>
        /// 把图片移到正中间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovePicToCenter(object sender, RoutedEventArgs e)
        {
            SViewer.ScrollToHorizontalOffset((picControl.Width * picControl.Scale - SViewer.ActualWidth) / 2 + PicPadding);
            SViewer.ScrollToVerticalOffset((picControl.Height * picControl.Scale - SViewer.ActualHeight) / 2 + PicPadding);
        }
        /// <summary>
        /// 复制文本到剪切板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyText(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject((sender as MenuItem).Tag.ToString(), true);
        }
        /// <summary>
        /// 通过按键缩放图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PicScaleByButton(object sender, RoutedEventArgs e)
        {
            string type = (sender as Button).Content.ToString();
            double scale = picControl.Scale, offset_h = SViewer.HorizontalOffset, offset_v = SViewer.VerticalOffset;
            sliderControl.SliderValue = sliderControl.GetScaleValue(type=="放大" ? true : false);
            //以下代码复制自SliderControl类的SliderValue属性
            if ((scale * picControl.Width + 2 * PicPadding <= SViewer.ActualWidth) && (picControl.Scale * picControl.Width + 2 * PicPadding > SViewer.ActualWidth))
            {
                SViewer.ScrollToHorizontalOffset((picControl.Scale * picControl.Width - SViewer.ActualWidth) / 2 + PicPadding);
            }
            else
            {
                SViewer.ScrollToHorizontalOffset((offset_h - PicPadding + SViewer.ActualWidth / 2) * picControl.Scale / scale - SViewer.ActualWidth / 2 + PicPadding);
            }
            if ((scale * picControl.Height + 2 * PicPadding <= SViewer.ActualHeight) && (picControl.Scale * picControl.Height + 2 * PicPadding > SViewer.ActualHeight))
            {
                SViewer.ScrollToVerticalOffset((picControl.Scale * picControl.Height - SViewer.ActualHeight) / 2 + PicPadding);
            }
            else
            {
                SViewer.ScrollToVerticalOffset((offset_v - PicPadding + SViewer.ActualHeight / 2) * picControl.Scale / scale - SViewer.ActualHeight / 2 + PicPadding);
            }
        }
        /// <summary>
        /// 控制两侧面板的打开与关闭状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SizePanelSwitch(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Content.ToString() == "\xE010")
            {
                LeftPanelWidth.Width = RightPanelWidth.Width = new GridLength(0);
                button.Content = "\xE011";
                button.ToolTip = "打开两侧面板";
            }
            else
            {
                LeftPanelWidth.Width = new GridLength(40);
                RightPanelWidth.Width = new GridLength(270);
                button.Content = "\xE010";
                button.ToolTip = "隐藏两侧面板";
            }
        }
        /// <summary>
        /// 主窗口按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (PicRectangle.IsChecked == true && !IsPressed)
                {
                    RectPositionTemp.Width = RectPositionTemp.Height = picControl.RectLeft = picControl.RectTop = picControl.RectWidth = picControl.RectHeight = 0;
                }
                if (PicRuler.IsChecked == true && !IsPressed)
                {
                    Ruler.Visibility = Visibility.Collapsed;
                }
            }
            else if(e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.S)
                {
                    if (SavePicture())
                    {
                        MsgWindow mw = new MsgWindow("保存", "保存成功!", "Confirm") { Owner = this };
                        mw.ShowDialog();
                    }
                }
                else if (e.Key == Key.N)
                {
                    CreatePictureWindow cpw = new CreatePictureWindow() { Owner = this };
                    cpw.ShowDialog();
                }
                else if (e.Key == Key.Z)
                {
                    listBoxControl.Index = listBoxControl.Index > 0 ? listBoxControl.Index - 1 : 0;
                }
                else if (e.Key == Key.Y)
                {
                    listBoxControl.Index = listBoxControl.Index + 1 == listBoxControl.History.Count ? listBoxControl.Index : listBoxControl.Index + 1;
                }
            }
        }
        private void Help(object sender, RoutedEventArgs e)
        {
            HelpWindow hw = new HelpWindow() { Owner = this };
            hw.Show();
        }
        //矩形选区边框函数
        private void Rect_MouseEnter(object sender, MouseEventArgs e)
        {
            Border bd = sender as Border;
            bd.Background = new SolidColorBrush(Colors.RoyalBlue);
        }
        private void Rect_MouseLeave(object sender, MouseEventArgs e)
        {
            Border bd = sender as Border;
            bd.Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255));
        }
        //图片剪切
        private void PicCut(object sender, RoutedEventArgs e)
        {
            if (CheckPictureAndRect())
            {
                string color = (sender as MenuItem).Header.ToString();
                color = color == "以前景色填充" ? FgColor.Background.ToString() : "#00FFFFFF";
                PicProcess picProcess = new PicProcess();
                BitmapSource bs = picProcess.PicCut(picControl.Pic, (int)picControl.RectLeft, (int)picControl.RectTop, (int)picControl.RectWidth, (int)picControl.RectHeight, color);
                listBoxControl.AddHistory(bs, "剪切", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
            }
        }
        //图片裁剪
        private void PicCrop(object sender, RoutedEventArgs e)
        {
            if (CheckPictureAndRect())
            {
                PicProcess picProcess = new PicProcess();
                BitmapSource bs = picProcess.PicCrop(picControl.Pic, (int)picControl.RectLeft, (int)picControl.RectTop, (int)picControl.RectWidth, (int)picControl.RectHeight);
                listBoxControl.AddHistory(bs, "裁剪", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
            }
        }
        private bool CheckPictureAndRect()
        {
            if (picControl.Pic == null)
                return false;
            if (picControl.RectWidth == 0 || picControl.RectHeight == 0)
            {
                MsgWindow mw = new MsgWindow("失败", "请先选择一个区域!", "Confirm") { Owner = this };
                mw.ShowDialog();
                return false;
            }
            return true;
        }
        private void HistoryPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RectPositionTemp.Width = RectPositionTemp.Height = picControl.RectLeft = picControl.RectTop = picControl.RectWidth = picControl.RectHeight = 0;
            picControl.Pic = listBoxControl.History[HistoryPanel.SelectedIndex].Pic();
            picControl.PicFormat = listBoxControl.History[HistoryPanel.SelectedIndex].HistoryPic.Format.ToString();
            picControl.PicOrigin = listBoxControl.History[HistoryPanel.SelectedIndex].Path;
            picControl.PicName = listBoxControl.History[HistoryPanel.SelectedIndex].Name;
            picControl.RealWidth = picControl.Scale * picControl.Width;
            picControl.RealHeight = picControl.Scale * picControl.Height;
            HistoryPanel.ScrollIntoView(HistoryPanel.Items[listBoxControl.Index]);
        }
        private void Undo(object sender, RoutedEventArgs e)
        {
            listBoxControl.Index = listBoxControl.Index > 0 ? listBoxControl.Index - 1 : 0;
        }
        private void Redo(object sender, RoutedEventArgs e)
        {
            listBoxControl.Index = listBoxControl.Index + 1 == listBoxControl.History.Count ? listBoxControl.Index : listBoxControl.Index + 1;
        }
        private void CopyPicture(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            Clipboard.SetImage(listBoxControl.History[HistoryPanel.SelectedIndex].HistoryPic);
        }
        private void SetFgColor(object sender, RoutedEventArgs e)
        {
            ColorWindow colorWindow = new ColorWindow("设置前景色", FgColor.Background, "FgColor") { Owner = this };
            colorWindow.ShowDialog();
        }
        public void SetFgColor(Color color)
        {
            FgColor.Background = new SolidColorBrush(color);
            paintControl.DA.Color = color;
        }
        private void MovePicToEdge(object sender, RoutedEventArgs e)
        {
            switch((sender as MenuItem).Tag.ToString())
            {
                case "Top":SViewer.ScrollToVerticalOffset(0);break;
                case "Bottom":SViewer.ScrollToVerticalOffset(picControl.Height * picControl.Scale + PicPadding);break;
                case "Left":SViewer.ScrollToHorizontalOffset(0);break;
                case "Right":SViewer.ScrollToHorizontalOffset(picControl.Width * picControl.Scale + PicPadding);break;
            }
        }
        private void CleanRect(object sender, RoutedEventArgs e)
        {
            RectPositionTemp.Width = RectPositionTemp.Height = picControl.RectLeft = picControl.RectTop = picControl.RectWidth = picControl.RectHeight = 0;
        }
        private void Text_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PicTextBorder.Visibility == Visibility.Visible)
                return;
            TextContent.Text = "";
            Point p = e.GetPosition(Pic);
            //为了右下面板显示一致，取整数
            textControl.Left = (int)p.X;
            textControl.Top = (int)p.Y;
            PicTextBorder.Visibility = Visibility.Visible;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => { Keyboard.Focus(TextContent); }));
        }
        private void Text_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsPressed)
                {
                    Point p1 = e.GetPosition(Pic);
                    textControl.Left = (int)(p1.X - p0.X + 0.5);
                    textControl.Top = (int)(p1.Y - p0.Y + 0.5);
                }
                else
                {
                    p0 = e.GetPosition(TextContent);
                    if (p0.X > -10 && p0.X < TextContent.ActualWidth + 10 && p0.Y > -10 && p0.Y < TextContent.ActualHeight + 10)
                        IsPressed = true;
                }
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                if (IsPressed)
                    IsPressed = false;
            }
        }
        private void TextConfirm(object sender, RoutedEventArgs e)
        {
            if (PicTextBorder.Visibility == Visibility.Collapsed)
                return;
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.PicAddControl(picControl.Pic, TextContent, textControl.Left, textControl.Top);
            PicTextBorder.Visibility = Visibility.Collapsed;
            textControl.Left = textControl.Top = 0;
            listBoxControl.AddHistory(bs, "文字工具", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
        }
        private void TextCancel(object sender, RoutedEventArgs e)
        {
            PicTextBorder.Visibility = Visibility.Collapsed;
            textControl.Left = textControl.Top = 0;
        }
        private void SetTextBgColor(object sender, RoutedEventArgs e)
        {
            ColorWindow colorWindow = new ColorWindow("设置文本背景色", FgColor.Background, "TextBgColor") { Owner = this };
            colorWindow.ShowDialog();
        }
        public void SetTextBgColor(Color color)
        {
            TextContent.Background = TextBgColor.Background = new SolidColorBrush(color);
        }
        private void SetTextFontFamily(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            TextContent.FontFamily = new FontFamily(textControl.AllFontFamily[cb.SelectedIndex].FontFamily);
        }
        private void SetFontSize(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            TextContent.FontSize = textControl.AllFontSize[cb.SelectedIndex].FontSize * 4 / 3;
        }
        private void SetTextStyle(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch (cb.ToolTip.ToString())
            {
                case "粗体":
                    TextContent.FontWeight = cb.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;break;
                case "斜体":
                    TextContent.FontStyle = cb.IsChecked == true ? FontStyles.Italic : FontStyles.Normal;break;
                case "下划线":
                    TextContent.TextDecorations = cb.IsChecked == true ? TextDecorations.Underline : null;break;
            }
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            slider.Value = (int)slider.Value;
        }
        private void Fill_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(Pic);
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.PicFill(picControl.Pic, (int)p.X, (int)p.Y, FgColor.Background.ToString(), PicFillTolerance.Value / 100, PicFillContinuity.IsChecked == true ? true : false);
            listBoxControl.AddHistory(bs, "填充", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
        }
        private void Brush_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(Pic);
            InkCanvas.SetLeft(Elli, p.X - Elli.ActualWidth / 2);
            InkCanvas.SetTop(Elli, p.Y - Elli.ActualHeight / 2);
            if (PaintPen.IsChecked == true || PaintEraser.IsChecked == true)
                return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsPressed)
                {
                    List<Point> pointList = new List<Point>();
                    if (PaintLine.IsChecked == true)
                    {
                        pointList.Add(new Point(p0.X, p0.Y));
                        pointList.Add(new Point(p.X, p.Y));
                    }
                    else if (PaintRect.IsChecked == true)
                    {
                        pointList.Add(new Point(p0.X, p0.Y));
                        pointList.Add(new Point(p0.X, p.Y));
                        pointList.Add(new Point(p.X, p.Y));
                        pointList.Add(new Point(p.X, p0.Y));
                        pointList.Add(new Point(p0.X, p0.Y));
                    }
                    else if (PaintCircle.IsChecked == true)
                    {
                        double r = Math.Sqrt((p.X - p0.X) * (p.X - p0.X) + (p.Y - p0.Y) * (p.Y - p0.Y));
                        for (double i = 0; i <= 2 * Math.PI; i += 0.01)
                        {
                            pointList.Add(new Point(p0.X + r * Math.Cos(i), p0.Y + r * Math.Sin(i)));
                        }
                    }
                    else if (PaintEllipse.IsChecked == true)
                    {
                        double r1 = Math.Abs(p.X - p0.X), r2 = Math.Abs(p.Y - p0.Y);
                        for (double i = 0; i <= 2 * Math.PI; i += 0.01)
                        {
                            pointList.Add(new Point(p0.X + r1 * Math.Cos(i), p0.Y + r2 * Math.Sin(i)));
                        }
                    }
                    StylusPointCollection point = new StylusPointCollection(pointList);
                    if (paintControl.Delete)
                        inkCanvas.Strokes.Remove(inkCanvas.Strokes[inkCanvas.Strokes.Count - 1]);
                    else
                        paintControl.Delete = true;
                    inkCanvas.Strokes.Add(new System.Windows.Ink.Stroke(point){
                        DrawingAttributes = inkCanvas.DefaultDrawingAttributes.Clone()
                    });
                }
                else
                {
                    IsPressed = true;
                    p0 = e.GetPosition(Pic);
                }
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                if (IsPressed)
                    IsPressed = paintControl.Delete= false;
            }
        }
        private void PaintMode_Click(object sender, RoutedEventArgs e)
        {
            if (PaintPen.IsChecked == true)
                inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            else if (PaintEraser.IsChecked == true)
                inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            else
                inkCanvas.EditingMode = InkCanvasEditingMode.None;
            if (PaintRect.IsChecked == true)
                paintControl.DA.StylusTip = System.Windows.Ink.StylusTip.Rectangle;
            else
                paintControl.DA.StylusTip = System.Windows.Ink.StylusTip.Ellipse;
        }
        private void SavePaint(object sender, RoutedEventArgs e)
        {
            if (inkCanvas.Strokes.Count == 0)
                return;
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.PicAddControl(picControl.Pic, inkCanvas, 0, 0);
            listBoxControl.AddHistory(bs, "画笔工具", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
            inkCanvas.Strokes.Clear();
        }
        private void PaintUndo(object sender, RoutedEventArgs e)
        {
            if (inkCanvas.Strokes.Count == 0)
                return;
            inkCanvas.Strokes.Remove(inkCanvas.Strokes[inkCanvas.Strokes.Count - 1]);
        }
        private void AddSecPic(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "图像文件|*.jpg;*.png;*.jpeg;*.bmp;*.gif|Png图片|*.png|Jpeg图片|*.jpg;*.jpeg|Bmp图片|*.bmp|Gif图片|*.gif|所有文件|*.*"
            };
            if ((bool)ofd.ShowDialog())
            {
                PicProcess picProcess = new PicProcess();
                SecPicture = picProcess.OpenLocalPicture(ofd.FileName);
                if (SecPicture == null)
                {
                    MsgWindow mw = new MsgWindow("打开出错", "此文件不是图片!", "Confirm") { Owner = this };
                    mw.ShowDialog();
                    return;
                }
                AfterAddSecPic();
            }
        }
        private void AddSecPicByURL(object sender, RoutedEventArgs e)
        {
            OpenPictureWindow opw = new OpenPictureWindow("AddSecPicByURL") { Owner = this };
            opw.ShowDialog();
        }
        public void AddSecPicByURL(string path)
        {
            PicProcess picProcess = new PicProcess();
            SecPicture = picProcess.OpenWebPicture(path);
            if (SecPicture == null)
            {
                OpenPictureWindow opw = new OpenPictureWindow("AddSecPicByURL") { Owner = this };
                opw.SetValue(path);
                opw.ShowDialog();
                return;
            }
            AfterAddSecPic();
        }
        private void AddSecPicByClipboard(object sender, RoutedEventArgs e)
        {
            SecPicture = Clipboard.GetImage();
            if (SecPicture == null)
            {
                MsgWindow mw = new MsgWindow("打开出错", "在剪切板里面没有图片!", "Confirm") { Owner = this };
                mw.ShowDialog();
                return;
            }
            AfterAddSecPic();
        }
        private void AfterAddSecPic()
        {
            RectPositionTemp.Width = picControl.RectWidth = SecPicture.PixelWidth;
            RectPositionTemp.Height = picControl.RectHeight = SecPicture.PixelHeight;
            picControl.RectLeft = picControl.RectTop = 0;
            Canvas.SetLeft(RectPositionTemp, 0);
            Canvas.SetTop(RectPositionTemp, 0);
            SecPic.Source = SecPicture;
            AddPicture.Visibility = Visibility.Collapsed;
            SecPicMsg.Visibility = Visibility.Visible;
        }
        private void SetPicSynFillColor(object sender, RoutedEventArgs e)
        {
            ColorWindow colorWindow = new ColorWindow("设置文本背景色", FgColor.Background, "PicSynFillColor") { Owner = this };
            colorWindow.ShowDialog();
        }
        public void SetPicSynFillColor(Color color)
        {
            PicSynFillColor.Background= new SolidColorBrush(color);
        }
        private void PicSynthesisConfirm(object sender, RoutedEventArgs e)
        {
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.PicSynthesis(picControl.Pic, SecPicture, (int)picControl.RectLeft, (int)picControl.RectTop, (int)picControl.RectWidth, (int)picControl.RectHeight, PicSynFillColor.Background.ToString());
            listBoxControl.AddHistory(bs, "图片合成", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
            PicSynthesisCancel(sender, e);
        }
        private void PicSynthesisCancel(object sender, RoutedEventArgs e)
        {
            RectPositionTemp.Width = RectPositionTemp.Height = picControl.RectLeft = picControl.RectTop = picControl.RectWidth = picControl.RectHeight = 0;
            AddPicture.Visibility = Visibility.Visible;
            SecPicMsg.Visibility = Visibility.Collapsed;
        }
        private void RulerElli_MouseEnter(object sender, MouseEventArgs e)
        {
            Ellipse elli = sender as Ellipse;
            elli.Stroke = new SolidColorBrush(Colors.RoyalBlue);
        }
        private void RulerElli_MouseLeave(object sender, MouseEventArgs e)
        {
            Ellipse elli = sender as Ellipse;
            elli.Stroke = new SolidColorBrush(Colors.Black);
        }
        private void RulerElli_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Ellipse elli = sender as Ellipse;
                RectResizeMode = elli.Tag.ToString();
            }
            catch (Exception)
            {
                Line line = sender as Line;
                RectResizeMode = line.Tag.ToString();
            }
        }
        private void Ruler_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsPressed)
                {
                    Point p = e.GetPosition(Pic);
                    if (RectResizeMode == "NULL")
                    {
                        picControl.PointX2 = p.X;
                        picControl.PointY2 = p.Y;
                    }
                    else if (RectResizeMode == "MD")
                    {
                        double x1 = p.X - p0.X;
                        double y1 = p.Y - p0.Y;
                        picControl.PointX2 += x1 - picControl.PointX1;
                        picControl.PointY2 += y1 - picControl.PointY1;
                        picControl.PointX1 = x1;
                        picControl.PointY1 = y1;
                    }
                    else if (RectResizeMode == "Point1")
                    {
                        picControl.PointX1 = p.X;
                        picControl.PointY1 = p.Y;
                    }
                    else if (RectResizeMode == "Point2")
                    {
                        picControl.PointX2 = p.X;
                        picControl.PointY2 = p.Y;
                    }
                }
                else
                {
                    if (RectResizeMode == "NULL")
                    {
                        Ruler.Visibility = Visibility.Visible;
                        p0 = e.GetPosition(Pic);
                        picControl.PointX1 = picControl.PointX2 = p0.X;
                        picControl.PointY1 = picControl.PointY2 = p0.Y;
                    }
                    else if (RectResizeMode == "MD")
                    {
                        p0 = e.GetPosition(Ruler);
                        p0.X -= picControl.ElliSize / 2;
                        p0.Y -= picControl.ElliSize / 2;
                    }
                    IsPressed = true;
                }
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                if (IsPressed)
                {
                    IsPressed = false;
                    RectResizeMode = "NULL";
                }
            }
        }
        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            Panel.SetZIndex(sender as System.Windows.Shapes.Path, 4);
        }
        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Shapes.Path path = sender as System.Windows.Shapes.Path;
            Panel.SetZIndex(path, int.Parse(path.Tag.ToString()));
        }
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            tb.Opacity = 1;
            switch (tb.Text)
            {
                case "R ":Panel.SetZIndex(RedPath, 4);break;
                case "G ":Panel.SetZIndex(GreenPath, 4);break;
            }
        }
        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            tb.Opacity = 0.4;
            switch (tb.Text)
            {
                case "R ": Panel.SetZIndex(RedPath, 1); break;
                case "G ": Panel.SetZIndex(GreenPath, 2); break;
            }
        }
        private bool SavePicture()
        {
            if (picControl.Pic == null)
                return false;
            BitmapEncoder be;
            string dir = listBoxControl.History[listBoxControl.Index].Path;
            if(dir.LastIndexOf('\\')==-1||!Directory.Exists(dir.Substring(0, dir.LastIndexOf('\\'))))
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif|TIFF (*.tiff;*.tif)|*.tiff;*.tif|WMP (*.wmp)|*.wmp|所有文件|*.*",
                    RestoreDirectory = true,
                    AddExtension = true,
                    FileName = listBoxControl.History[listBoxControl.Index].Name
                };
                if (sfd.ShowDialog() == true)
                {
                    be = (sfd.SafeFileName.ToLower()) switch
                    {
                        "bmp" => new BmpBitmapEncoder(),
                        "gif" => new GifBitmapEncoder(),
                        "png" => new PngBitmapEncoder(),
                        "tiff" => new TiffBitmapEncoder(),
                        "wmp" => new WmpBitmapEncoder(),
                        _ => new JpegBitmapEncoder(),
                    };
                    be.Frames.Add(BitmapFrame.Create(picControl.Pic));
                    using var stream = new FileStream(sfd.FileName, FileMode.Create);
                    be.Save(stream);
                }
                else
                    return false;
            }
            else
            {
                string ext_name = dir.Substring(dir.LastIndexOf('.') + 1, dir.Length - dir.LastIndexOf('.') - 1);
                be = (ext_name.ToLower()) switch
                {
                    "bmp" => new BmpBitmapEncoder(),
                    "gif" => new GifBitmapEncoder(),
                    "png" => new PngBitmapEncoder(),
                    "tiff" => new TiffBitmapEncoder(),
                    "wmp" => new WmpBitmapEncoder(),
                    _ => new JpegBitmapEncoder(),
                };
                be.Frames.Add(BitmapFrame.Create(picControl.Pic));
                using var stream = new FileStream(listBoxControl.History[listBoxControl.Index].Path, FileMode.Create);
                be.Save(stream);
            }
            picControl.LastPic = picControl.Pic;
            return true;
        }
        private void SavePicture(object sender, RoutedEventArgs e)
        {
            if (SavePicture())
            {
                MsgWindow mw = new MsgWindow("保存", "保存成功!", "Confirm") { Owner = this };
                mw.ShowDialog();
            }
        }
        private void CreatePicture(object sender, RoutedEventArgs e)
        {
            CreatePictureWindow cpw = new CreatePictureWindow() { Owner = this };
            cpw.ShowDialog();
        }
        public void CreatePicture(int width,int height,double dpix,double dpiy,bool is_white_bg)
        {
            IfSavePicture();
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.CreatePicture(width, height, dpix, dpiy, is_white_bg);
            listBoxControl.AddHistory(bs, "新建图片", "图片", "新建图片");
            AfterOpenPicture();
        }
        private void SavePictureAs(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            MenuItem mi = sender as MenuItem;
            int p = listBoxControl.History[listBoxControl.Index].Name.LastIndexOf('.');
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = (mi.Header.ToString()) switch
                {
                    "Jpeg图片" => "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg",
                    "Png图片" => "PNG (*.png)|*.png",
                    "Bmp图片" => "BMP (*.bmp)|*.bmp",
                    "Wmp图片" => "WMP (*.wmp)|*.wmp",
                    "Gif图片" => "GIF (*.gif)|*.gif",
                    "Tiff图片" => "TIFF (*.tiff;*.tif)",
                    _ => "",
                },
                RestoreDirectory = true,
                AddExtension = true,
                FileName = p == -1 ? listBoxControl.History[listBoxControl.Index].Name : listBoxControl.History[listBoxControl.Index].Name.Substring(0, p),
            };
            if (sfd.ShowDialog() == true)
            {
                BitmapEncoder be = (mi.Header.ToString()) switch
                {
                    "Jpeg图片" => new JpegBitmapEncoder(),
                    "Png图片" => new PngBitmapEncoder(),
                    "Bmp图片" => new BmpBitmapEncoder(),
                    "Wmp图片" => new WmpBitmapEncoder(),
                    "Gif图片" => new GifBitmapEncoder(),
                    "Tiff图片" => new TiffBitmapEncoder(),
                    _ => new JpegBitmapEncoder(),
                };
                be.Frames.Add(BitmapFrame.Create(picControl.Pic));
                using var stream = new FileStream(sfd.FileName, FileMode.Create);
                be.Save(stream);
                picControl.LastPic = picControl.Pic;
            }
        }
        private void CopyRect(object sender, RoutedEventArgs e)
        {
            if (CheckPictureAndRect())
            {
                PicProcess picProcess = new PicProcess();
                Clipboard.SetImage(picProcess.PicCrop(picControl.Pic, (int)picControl.RectLeft, (int)picControl.RectTop, (int)picControl.RectWidth, (int)picControl.RectHeight));
            }
        }
        private void PicResize(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicResizeWindow prw = new PicResizeWindow(picControl.Pic.PixelWidth, picControl.Pic.PixelHeight) { Owner = this };
            prw.ShowDialog();
        }
        public void PicResize(int width,int height)
        {
            if (width == picControl.Pic.PixelWidth && height == picControl.Pic.PixelHeight)
                return;
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.PicResize(picControl.Pic, width, height);
            listBoxControl.AddHistory(bs, "修改图像尺寸", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);

        }
        private void PicRotate(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.PicRotate(picControl.Pic, (sender as MenuItem).Tag.ToString() == "左" ? -90 : 90);
            listBoxControl.AddHistory(bs, (sender as MenuItem).Tag.ToString() == "左" ? "向左旋转90度" : "向右旋转90度", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
        }
        private void PicMirror(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.PicMirror(picControl.Pic, (sender as MenuItem).Tag.ToString());
            listBoxControl.AddHistory(bs, (sender as MenuItem).Tag.ToString() == "Horizontal" ? "水平镜像" : "垂直镜像", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
        }

        private void PicColorReverse(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.PicColorReverse(picControl.Pic);
            listBoxControl.AddHistory(bs, "反色", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
        }
        private void PicToGray(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicProcess picProcess = new PicProcess();
            BitmapSource bs = picProcess.PicToGray(picControl.Pic, setting.Precision);
            listBoxControl.AddHistory(bs, "灰度图", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
        }
        private void PicToBinary(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => 
            {
                PicProcess picProcess = new PicProcess();
                BitmapSource bs = picProcess.PicToBinary(picControl.Pic, setting.Precision, out int threshold);
                listBoxControl.AddHistory(bs, "二值图 (" + threshold + ")", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
            }));
        }
        private void SetTopMost(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
            TopMostSetting1.Header = TopMostSetting2.Header = Topmost ? "取消窗口置顶" : "窗口置顶";
        }
        private bool RunCmd(string cmd, bool administrator = false)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            try
            {
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = "/c " + cmd;
                if (administrator)
                    process.StartInfo.Verb = "runas";
                process.StartInfo.UseShellExecute = administrator ? true : false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
            catch (Exception)
            {
                MsgWindow mw = new MsgWindow("失败", "未允许程序对你的设备进行更改", "Confirm") { Owner = this };
                mw.ShowDialog();
                return false;
            }
            return true;
        }
        private void CreateShortCut(object sender, RoutedEventArgs e)
        {
            string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (File.Exists(DesktopPath+ "\\PicEditor.lnk"))
            {
                MsgWindow mw = new MsgWindow("失败", "桌面已存在快捷方式！", "Confirm") { Owner = this };
                mw.ShowDialog();
                return;
            }
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(DesktopPath + "\\PicEditor.lnk");
            shortcut.TargetPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            shortcut.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            shortcut.WindowStyle = 1; //目标应用程序的窗口状态分为普通、最大化、最小化 [1,3,7]
            shortcut.Description = "PicEditor";
            shortcut.IconLocation = AppDomain.CurrentDomain.BaseDirectory + "PicEditor.ico";
            shortcut.Arguments = "";
            shortcut.Hotkey = "";
            shortcut.Save();
        }
        private void AddToSystemContextMenu(object sender, RoutedEventArgs e)
        {
            string exe_path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string dir = Environment.CurrentDirectory;
            if ((sender as MenuItem).Tag.ToString() == "Add")
            {
                string[] cmd ={
                        "reg add HKEY_CLASSES_ROOT\\*\\shell\\PicEditor\\command /t REG_SZ /d \"" + exe_path + " %1\"",
                        "reg add HKEY_CLASSES_ROOT\\*\\shell\\PicEditor /t REG_SZ /d 使用PicEditor打开",
                        "reg add HKEY_CLASSES_ROOT\\*\\shell\\PicEditor /v Icon /t REG_SZ /d " + dir + "\\PicEditor.ico"};
                if (RunCmd(cmd[0] + "&" + cmd[1] + "&" + cmd[2], true))
                {
                    MsgWindow mw = new MsgWindow("完成", "已将程序添加到系统右键菜单", "Confirm") { Owner = this };
                    mw.ShowDialog();
                }
            }
            else if ((sender as MenuItem).Tag.ToString() == "Delete")
            {
                if (RunCmd("reg delete HKEY_CLASSES_ROOT\\*\\shell\\PicEditor /f", true))
                {
                    MsgWindow mw = new MsgWindow("完成", "已将程序从系统右键菜单移除", "Confirm") { Owner = this };
                    mw.ShowDialog();
                }
            }
        }
        private void OpenSetting(object sender, RoutedEventArgs e)
        {
            SettingWindow sw = new SettingWindow(setting) { Owner = this };
            sw.Show();
        }
        private void About(object sender, RoutedEventArgs e)
        {
            AboutWindow aw = new AboutWindow() { Owner = this };
            aw.ShowDialog();
        }
        private void RectTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(RectPositionTemp, picControl.RectLeft);
            Canvas.SetTop(RectPositionTemp, picControl.RectTop);
            RectPositionTemp.Width = picControl.RectWidth;
            RectPositionTemp.Height = picControl.RectHeight;
        }
        public void PicColorCallback(BitmapSource bs = null, string name = null)
        {
            if (bs != null && name == null)
            {
                picControl.Pic = bs;
            }
            else if (bs == null && name == null)
                picControl.Pic = listBoxControl.History[listBoxControl.Index].HistoryPic;
            else if (bs == null && name != null)
                listBoxControl.AddHistory(picControl.Pic, name, listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
        }
        private void PicBrightnessAndContrast(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicBACWindow pbacw = new PicBACWindow(picControl.Pic) { Owner = this };
            pbacw.ShowDialog();
        }
        private void PicColorScale(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicColorScaleWindow pcsw = new PicColorScaleWindow(picControl.Pic) { Owner = this };
            pcsw.ShowDialog();
        }
        private void PicHS(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicHSWindow phsw = new PicHSWindow(picControl.Pic) { Owner = this };
            phsw.ShowDialog();
        }
        private void PicTonalSeparation(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicTSWindow ptsw = new PicTSWindow(picControl.Pic) { Owner = this };
            ptsw.ShowDialog();
        }
        private void PicCurve(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicCurveWindow pcw = new PicCurveWindow(picControl.Pic) { Owner = this };
            pcw.ShowDialog();
        }

        private void PicSharpen(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicSharpenWindow psw = new PicSharpenWindow(picControl.Pic) { Owner = this };
            psw.ShowDialog();
        }
        private void PicMeanFilter(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicMeanFilterWindow pmfw = new PicMeanFilterWindow(picControl.Pic) { Owner = this };
            pmfw.ShowDialog();
        }

        private void PicWhiteBalance(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            BitmapBaseData bbd = new BitmapBaseData(picControl.Pic);
            new Thread(() => {
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicWhiteBalance(bbd.buffer, bbd.width, bbd.height);
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => {
                    listBoxControl.AddHistory(bbd.Get(), "白平衡", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
                }));
            }).Start();
        }
        private void PicPixelated(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicPixelatedWindow ppw = new PicPixelatedWindow(picControl.Pic) { Owner = this };
            ppw.ShowDialog();
        }

        private void PicChannelMixer(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            PicChannelMixerWindow pcmw = new PicChannelMixerWindow(picControl.Pic) { Owner = this };
            pcmw.ShowDialog();
        }
        private void PicExtractChannel(object sender, RoutedEventArgs e)
        {
            if (picControl.Pic == null)
                return;
            BitmapBaseData bbd = new BitmapBaseData(picControl.Pic);
            string channel = (sender as MenuItem).Tag.ToString();
            new Thread(() => {
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicExtractChannel(bbd.buffer, channel);
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => {
                    listBoxControl.AddHistory(bbd.Get(), "提取分量 (" + (sender as MenuItem).Header.ToString() + ")", listBoxControl.History[HistoryPanel.SelectedIndex].Name, listBoxControl.History[HistoryPanel.SelectedIndex].Path);
                }));
            }).Start();
        }
        //以下函数用于子窗口调用，实现颜色选项中各功能的图像处理
        public void PicMeanFilter(BitmapSource bs, PicMeanFilterWindow pmfw, int radius)
        {
            BitmapBaseData bbd = new BitmapBaseData(bs);
            new Thread(()=>{
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicMeanFilter(bbd.buffer, radius, bbd.width, bbd.height, bbd.stride);
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>{
                    picControl.Pic = bbd.Get();
                    pmfw.Back();
                }));
            }).Start();
        }
        public void PicBAC(BitmapSource bs, PicBACWindow pbacw, double brightness, double contrast)
        {
            BitmapBaseData bbd = new BitmapBaseData(bs);
            new Thread(()=> {
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicBAC(bbd.buffer, brightness, contrast);
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>{
                    picControl.Pic = bbd.Get();
                    pbacw.Back();
                }));
            }).Start();
        }
        public void PicHS(BitmapSource bs,PicHSWindow phsw, int mode, double hue, double saturation, double value)
        {
            BitmapBaseData bbd = new BitmapBaseData(bs);
            new Thread(() =>{
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicHS(bbd.buffer, mode, hue, saturation, value);
                Dispatcher.BeginInvoke(DispatcherPriority.Background,(Action)(()=> {
                    picControl.Pic = bbd.Get();
                    phsw.Back();
                }));
            }).Start();
        }
        public void PicColorScale(BitmapSource bs, PicColorScaleWindow pcsw, int mode, double iblack, double igray, double iwhite, double oblack, double owhite)
        {
            BitmapBaseData bbd = new BitmapBaseData(bs);
            new Thread(() => {
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicColorScale(bbd.buffer, mode, iblack, igray, iwhite, oblack, owhite);
                Dispatcher.BeginInvoke(DispatcherPriority.Background,(Action)(()=> {
                    picControl.Pic = bbd.Get();
                    pcsw.Back();
                }));
            }).Start();
        }
        public void PicCurve(BitmapSource bs,PicCurveWindow pcw,int[][] path_data)
        {
            BitmapBaseData bbd = new BitmapBaseData(bs);
            new Thread(()=> {
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicCurve(bbd.buffer, path_data);
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => {
                    picControl.Pic = bbd.Get();
                    pcw.Back();
                }));
            }).Start();
        }
        public void PicTS(BitmapSource bs, PicTSWindow ptsw, int num)
        {
            BitmapBaseData bbd = new BitmapBaseData(bs);
            new Thread(()=> {
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicTS(bbd.buffer, num);
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => {
                    picControl.Pic = bbd.Get();
                    ptsw.Back();
                }));
            }).Start();
        }
        public void PicSharpen(BitmapSource bs, PicSharpenWindow psw, double coe)
        {
            BitmapBaseData bbd = new BitmapBaseData(bs);
            new Thread(() => {
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicSharpen(bbd.buffer, coe, bbd.width, bbd.height);
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => {
                    picControl.Pic = bbd.Get();
                    psw.Back();
                }));
            }).Start();
        }
        public void PicPixelated(BitmapSource bs, PicPixelatedWindow ppw, int size)
        {
            BitmapBaseData bbd = new BitmapBaseData(bs);
            new Thread(() => {
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicPixelated(bbd.buffer, size, bbd.width, bbd.height, bbd.stride);
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => {
                    picControl.Pic = bbd.Get();
                    ppw.Back();
                }));
            }).Start();
        }
        public void PicChannelMixer(BitmapSource bs,PicChannelMixerWindow pcmw, double[] red, double[] green, double[] blue)
        {
            BitmapBaseData bbd = new BitmapBaseData(bs);
            new Thread(() => {
                PicProcess picProcess = new PicProcess();
                bbd.buffer = picProcess.PicChannelMixer(bbd.buffer, red, green, blue);
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => {
                    picControl.Pic = bbd.Get();
                    pcmw.Back();
                }));
            }).Start();
        }
    }
    class BitmapBaseData
    {
        public byte[] buffer;
        public int width = 0;
        public int height = 0;
        public int stride = 0;
        private WriteableBitmap WB;
        public BitmapBaseData(BitmapSource bs = null)
        {
            if (bs != null)
                SetData(bs);
        }
        protected void SetData(BitmapSource bs)
        {
            WB = new WriteableBitmap(bs);
            buffer = new byte[WB.BackBufferStride * WB.PixelHeight];
            WB.CopyPixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            width = WB.PixelWidth;
            height = WB.PixelHeight;
            stride = WB.BackBufferStride;
        }
        public BitmapSource Get()
        {
            WB.Lock();
            WB.WritePixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            WB.Unlock();
            return WB;
        }
    }
}
/*
 * 
 1.直方图RGB改成RadioButton
 1.1吸管工具添加坐标显示(即该RGB是那个坐标下的像素值)
 1.2鼠标悬浮在滚动条时有背景色
 1.3直方图：三通道的最大值设置成同一个
 2.所有子窗体按键背景色改成#353535(按键背景色太白了)
 3.用背景线程解决由listbox引发的在画图时保存墨迹时的闪烁（有可能是没有用Dispatcher调用History.Pic()引发的）
     
     */

