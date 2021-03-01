using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.core
{
    class PicProcess
    {
        /// <summary>
        /// 打开本地图片
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource OpenLocalPicture(string path)
        {
            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                using (Stream ms = new MemoryStream(File.ReadAllBytes(path)))
                {
                    bi.StreamSource = ms;
                    bi.EndInit();
                    bi.Freeze();
                }
                return bi;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 打开网络图片
        /// </summary>
        /// <param name="path">链接URL</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource OpenWebPicture(string path)
        {
            try
            {
                System.Net.WebRequest request = System.Net.WebRequest.Create(path);
                System.Net.WebResponse response = request.GetResponse();
                byte[] array;
                using (Stream stream = response.GetResponseStream())
                {
                    using MemoryStream ms = new MemoryStream();
                    Byte[] buffer = new Byte[1024];
                    int current = 0;
                    do
                    {
                        ms.Write(buffer, 0, current);
                    } while ((current = stream.Read(buffer, 0, buffer.Length)) != 0);
                    array = ms.ToArray();
                }
                MemoryStream memoryStream = new MemoryStream(array);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = memoryStream;
                bi.EndInit();
                return bi;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 创建图片
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="dpix">DpiX</param>
        /// <param name="dpiy">DpiY</param>
        /// <param name="is_white_bg">true为白色背景,false为透明背景</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource CreatePicture(int width, int height, double dpix, double dpiy, bool is_white_bg)
        {
            int rawStride = (width * 32 + 7) / 8;
            BitmapSource bs = BitmapSource.Create(width, height, dpix, dpiy, PixelFormats.Bgra32, null, new byte[rawStride * height], rawStride);
            WriteableBitmap WB = new WriteableBitmap(bs);
            WB.Lock();
            byte[] buffer = new byte[WB.BackBufferStride * WB.PixelHeight];
            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i + 3] = (byte)(is_white_bg ? 255 : 0);
                buffer[i + 2] = buffer[i + 1] = buffer[i] = 255;
            }
            WB.WritePixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            WB.Unlock();
            return WB;
        }
        /// <summary>
        /// 图片剪切 去除选区内部分
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="left">左边距</param>
        /// <param name="top">上边距</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="color">填充颜色</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicCut(BitmapSource origin, int left, int top, int width, int height,string color)
        {
            byte a = (byte)HexToDec(color.Substring(1, 2));
            byte r = (byte)HexToDec(color.Substring(3, 2));
            byte g = (byte)HexToDec(color.Substring(5, 2));
            byte b = (byte)HexToDec(color.Substring(7, 2));
            WriteableBitmap WB = new WriteableBitmap(origin);
            WB.Lock();
            int stride = WB.Format.BitsPerPixel * width / 8;
            byte[] buffer = new byte[stride * height];
            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i + 3] = a;
                buffer[i + 2] = r;
                buffer[i + 1] = g;
                buffer[i] = b;
            }
            WB.WritePixels(new Int32Rect(left, top, width, height), buffer, stride, 0);
            WB.AddDirtyRect(new Int32Rect(left, top, width, height));
            WB.Unlock();
            return WB;
        }
        /// <summary>
        /// 图片裁剪 保留选区内部分
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="left">左边距</param>
        /// <param name="top">上边距</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicCrop(BitmapSource origin,int left,int top,int width,int height)
        {
            BitmapSource bs = new CroppedBitmap(origin, new Int32Rect(left, top, width, height));
            return bs;
        }
        /// <summary>
        /// 把控件添加到图像
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="fe">控件</param>
        /// <param name="left">左边距</param>
        /// <param name="top">上边距</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicAddControl(BitmapSource origin, FrameworkElement fe,int left,int top)
        {
            //把控件转化成RenderTargetBitmap
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)(fe.ActualWidth + 0.5), (int)(fe.ActualHeight + 0.5), 96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(fe);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            //RenderTargetBitmap转化成BitmapImage
            BitmapImage bitmapImage = new BitmapImage();
            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }
            //如果控件超出图片则舍弃
            if (left + fe.ActualWidth <= 0 || left >= origin.PixelWidth || top + fe.ActualHeight <= 0 || top >= origin.PixelHeight)
                return origin;
            //将BitmapImage合成到origin上，超出部分舍去
            WriteableBitmap WB = new WriteableBitmap(origin);
            WB.Lock();
            int width = (int)(left + fe.ActualWidth > origin.PixelWidth ? origin.PixelWidth : left + fe.ActualWidth) - (left < 0 ? 0 : left);
            int height = (int)(top + fe.ActualHeight > origin.PixelHeight ? origin.PixelHeight : top + fe.ActualHeight) - (top < 0 ? 0 : top);
            int stride = origin.Format.BitsPerPixel * width / 8;
            byte[] Top = new byte[stride * height];
            byte[] Bottom = new byte[stride * height];
            bitmapImage.CopyPixels(new Int32Rect(left < 0 ? -left : 0, top < 0 ? -top : 0, width, height), Top, stride, 0);
            origin.CopyPixels(new Int32Rect(left > 0 ? left : 0, top > 0 ? top : 0, width, height), Bottom, stride, 0);
            for (int i = 0; i < Top.Length; i += 4)
            {
                Bottom[i + 2] = (byte)((Top[i + 2] * Top[i + 3] + Bottom[i + 2] * Bottom[i + 3]) / 255 - Bottom[i + 2] * Bottom[i + 3] * Top[i + 3] / 65025);
                Bottom[i + 1] = (byte)((Top[i + 1] * Top[i + 3] + Bottom[i + 1] * Bottom[i + 3]) / 255 - Bottom[i + 1] * Bottom[i + 3] * Top[i + 3] / 65025);
                Bottom[i] = (byte)((Top[i] * Top[i + 3] + Bottom[i] * Bottom[i + 3]) / 255 - Bottom[i] * Bottom[i + 3] * Top[i + 3] / 65025);
                Bottom[i + 3] = Bottom[i + 3] > Top[i + 3] ? Bottom[i + 3] : Top[i + 3];
            }
            WB.WritePixels(new Int32Rect(left > 0 ? left : 0, top > 0 ? top : 0, width, height), Bottom, stride, 0);
            WB.AddDirtyRect(new Int32Rect(left > 0 ? left : 0, top > 0 ? top : 0, width, height));
            WB.Unlock();
            return WB;
        }
        /// <summary>
        /// 图像填充
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="x">点的x坐标</param>
        /// <param name="y">点的y坐标</param>
        /// <param name="color">填充颜色</param>
        /// <param name="tolerance">容差</param>
        /// <param name="continuity">是否连续</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicFill(BitmapSource origin, int x, int y, string color, double tolerance, bool continuity)
        {
            byte a = (byte)HexToDec(color.Substring(1, 2));
            byte r = (byte)HexToDec(color.Substring(3, 2));
            byte g = (byte)HexToDec(color.Substring(5, 2));
            byte b = (byte)HexToDec(color.Substring(7, 2));
            int[] dx = new int[] { 1, -1, 0, 0 };
            int[] dy = new int[] { 0, 0, 1, -1 };
            Stack<PPoint> stack = new Stack<PPoint>();
            stack.Push(new PPoint(x, y));
            WriteableBitmap WB = new WriteableBitmap(origin);
            WB.Lock();
            byte[] buffer = new byte[WB.BackBufferStride * WB.PixelHeight];
            origin.CopyPixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            int p = (WB.PixelWidth * y + x) * 4;
            double u = (buffer[p] + buffer[p + 1] + buffer[p + 2]) / 3.0;
            double[] p1 = new double[] { buffer[p + 2] - u, buffer[p + 1] - u, buffer[p] - u, u };
            double[] p2;
            buffer[p + 3] = a;
            buffer[p + 2] = r;
            buffer[p + 1] = g;
            buffer[p] = b;
            if (continuity)
            {
                while (stack.Count > 0)
                {
                    PPoint pp = stack.Pop();
                    for (int i = 0; i < 4; i++)
                    {
                        int x1 = pp.X + dx[i], y1 = pp.Y + dy[i];
                        if (x1 < 0 || x1 >= origin.PixelWidth || y1 < 0 || y1 >= origin.PixelHeight)
                            continue;
                        p = (WB.PixelWidth * y1 + x1) * 4;
                        if (buffer[p] == b && buffer[p + 1] == g && buffer[p + 2] == r && buffer[p + 3] == a)
                            continue;
                        u = (buffer[p] + buffer[p + 1] + buffer[p + 2]) / 3.0;
                        p2 = new double[] { buffer[p + 2] - u, buffer[p + 1] - u, buffer[p] - u, u };
                        if (PixelTolerance(p1, p2) <= tolerance)
                        {
                            buffer[p + 3] = a;
                            buffer[p + 2] = r;
                            buffer[p + 1] = g;
                            buffer[p] = b;
                            stack.Push(new PPoint(x1, y1));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < buffer.Length; i += 4)
                {
                    if (buffer[i] == b && buffer[i + 1] == g && buffer[i + 2] == r && buffer[i + 3] == a)
                        continue;
                    u = (buffer[i] + buffer[i + 1] + buffer[i + 2]) / 3.0;
                    p2 = new double[] { buffer[i + 2] - u, buffer[i + 1] - u, buffer[i] - u, u };
                    if (PixelTolerance(p1, p2) <= tolerance)
                    {
                        buffer[i + 3] = a;
                        buffer[i + 2] = r;
                        buffer[i + 1] = g;
                        buffer[i] = b;
                    }
                }
            }
            WB.WritePixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            WB.Unlock();
            return WB;
        }
        /// <summary>
        /// 更改图像尺寸
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="width">目标宽度</param>
        /// <param name="height">目标高度</param>
        /// <returns>BitmapSource</returns>
        public  BitmapSource PicResize(BitmapSource origin, int width, int height)
        {
            TransformedBitmap tb = new TransformedBitmap();
            tb.BeginInit();
            tb.Source = origin;
            ScaleTransform st = new ScaleTransform((double)width / origin.PixelWidth, (double)height / origin.PixelHeight);
            tb.Transform = st;
            tb.EndInit();
            return tb;
        }
        /// <summary>
        /// 图像旋转（只能向左或右旋转90度）
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="angle">角度（只能90或-90）</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicRotate(BitmapSource origin, double angle)
        {
            TransformedBitmap tb = new TransformedBitmap();
            tb.BeginInit();
            tb.Source = origin;
            RotateTransform rt = new RotateTransform(angle);
            tb.Transform = rt;
            tb.EndInit();
            return tb;
        }
        /// <summary>
        /// 图像镜像
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="mode">模式(水平：Horizontal 垂直：Vertical)</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicMirror(BitmapSource origin, string mode)
        {
            WriteableBitmap WB = new WriteableBitmap(origin);
            WB.Lock();
            byte[] buffer = new byte[WB.BackBufferStride * WB.PixelHeight];
            origin.CopyPixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            int p1, p2;
            byte temp;
            if (mode == "Horizontal")
            {
                for(int j = 0; j < WB.PixelHeight; j++)
                {
                    for(int i = 0; i < (WB.PixelWidth + 1) / 2; i++)
                    {
                        p1 = (WB.PixelWidth * j + i) * 4;
                        p2 = (WB.PixelWidth * j + WB.PixelWidth - i - 1) * 4;
                        for(int k = 0; k < 4; k++)
                        {
                            temp = buffer[p1 + k];
                            buffer[p1 + k] = buffer[p2 + k];
                            buffer[p2 + k] = temp;
                        }
                    }
                }
            }
            else if (mode == "Vertical")
            {
                for (int i = 0; i < WB.PixelWidth; i++)
                {
                    for (int j = 0; j < (WB.PixelHeight + 1) / 2; j++)
                    {
                        p1 = (WB.PixelWidth * j + i) * 4;
                        p2 = (WB.PixelWidth * j + WB.PixelWidth - i - 1) * 4;
                        for (int k = 0; k < 4; k++)
                        {
                            temp = buffer[p1 + k];
                            buffer[p1 + k] = buffer[p2 + k];
                            buffer[p2 + k] = temp;
                        }
                    }
                }
            }
            WB.WritePixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            WB.Unlock();
            return WB;
        }
        /// <summary>
        /// 图像反色
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicColorReverse(BitmapSource origin)
        {
            WriteableBitmap WB = new WriteableBitmap(origin);
            WB.Lock();
            byte[] buffer = new byte[WB.BackBufferStride * WB.PixelHeight];
            origin.CopyPixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i] = (byte)(255 - buffer[i]);
                buffer[i + 1] = (byte)(255 - buffer[i + 1]);
                buffer[i + 2] = (byte)(255 - buffer[i + 2]);
            }
            WB.WritePixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            WB.Unlock();
            return WB;
        }
        /// <summary>
        /// 灰度图
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="coe">灰度图精度</param>
        /// <param name="p">每个灰度级像素的个数</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicToGray(BitmapSource origin, int coe, double[] p = null)
        {
            UInt32[,] coes =
            {
                {1,2,1}, //2
                {2,5,1}, //3
                {4,10,2}, //4
                {9,19,4}, //5
                {19,37,8}, //6
                {38,75,15}, //7
                {76,150,30}, //8
                {153,300,59}, //9
                {306,601,117}, //10
                {612,1202,234}, //11
                {1224,2405,467}, //12
                {2449,4809,934}, //13
                {4898,9618,1868}, //14
                {9797,19235,3736}, //15
                {19595,38469,7472}, //16
                {39190,76939,14943}, //17
                {78381,153878,29885}, //18
                {156762,307757,59769}, //19
                {313524,615514,119538}, //20
            };
            WriteableBitmap WB = new WriteableBitmap(origin);
            WB.Lock();
            byte[] buffer = new byte[WB.BackBufferStride * WB.PixelHeight];
            origin.CopyPixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            if (p == null)
            {
                for (int i = 0; i < buffer.Length; i+=4)
                {
                    buffer[i] = (byte)((buffer[i + 2] * coes[coe - 2, 0] + buffer[i + 1] * coes[coe - 2, 1] + buffer[i] * coes[coe - 2, 2]) >> coe);
                    buffer[i + 2] = buffer[i + 1] = buffer[i];
                }
            }
            else
            {
                for (int i = 0; i < buffer.Length; i+=4)
                {
                    buffer[i] = (byte)((buffer[i + 2] * coes[coe - 2, 0] + buffer[i + 1] * coes[coe - 2, 1] + buffer[i] * coes[coe - 2, 2]) >> coe);
                    buffer[i + 2] = buffer[i + 1] = buffer[i];
                    p[buffer[i]]++;
                }
            }
            WB.WritePixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            WB.Unlock();
            return WB;
        }
        /// <summary>
        /// 二值图(最大类间方差阈值选择法)
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="coe">灰度图精度</param>
        /// <param name="threshold">阈值</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicToBinary(BitmapSource origin, int coe, out int threshold)
        {
            double[] p = new double[256];
            origin = PicToGray(origin, coe, p);
            WriteableBitmap WB = new WriteableBitmap(origin);
            WB.Lock();
            for (int i = 0; i < 256; i++)
                p[i] = p[i] / (WB.PixelWidth * WB.PixelHeight);
            double variance = -1;
            threshold = 0;
            for (int k = 0; k < 256; k++)
            {
                double w0 = 0, u0 = 0;
                for (int i = 0; i < k - 1; i++)
                {
                    w0 += p[i];
                    u0 += i * p[i];
                }
                double w1 = 1 - w0, u1 = 0;
                for (int i = k; i < 256; i++)
                    u1 += i * p[i];
                u0 /= w0;
                u1 /= w1;
                double v = w0 * w1 * (u0 - u1) * (u0 - u1);
                if (v > variance)
                {
                    variance = v;
                    threshold = k;
                }
            }
            byte[] buffer = new byte[WB.BackBufferStride * WB.PixelHeight];
            origin.CopyPixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            for(int i = 0; i < buffer.Length; i += 4)
                buffer[i + 2] = buffer[i + 1] = buffer[i] = (byte)(buffer[i] < threshold ? 0 : 255);
            WB.WritePixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            WB.Unlock();
            return WB;
        }
        /// <summary>
        /// 图片合成
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="sec">第二张图片</param>
        /// <param name="left">左边距</param>
        /// <param name="top">上边距</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="color">填充颜色</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicSynthesis(BitmapSource origin, BitmapSource sec, int left, int top, int width, int height, string color)
        {
            UInt32 Color = UInt32.Parse(color[1..], System.Globalization.NumberStyles.HexNumber);
            int W = (width + left > origin.PixelWidth ? (width + left) : origin.PixelWidth) - (left < 0 ? left : 0);
            int H = (height + top > origin.PixelHeight ? (height + top) : origin.PixelHeight) - (top < 0 ? top : 0);
            byte a = (byte)HexToDec(color.Substring(1, 2));
            byte r = (byte)HexToDec(color.Substring(3, 2));
            byte g = (byte)HexToDec(color.Substring(5, 2));
            byte b = (byte)HexToDec(color.Substring(7, 2));
            int stride = origin.Format.BitsPerPixel * W / 8, p1, p2;
            byte[] Bottom = new byte[stride * H];
            WriteableBitmap WB = new WriteableBitmap(BitmapSource.Create(W, H, origin.DpiX, origin.DpiY, PixelFormats.Bgra32, null, Bottom, stride));
            WB.Lock();
            for (int i = 0; i < Bottom.Length; i+=4)
            {
                Bottom[i + 3] = a;
                Bottom[i + 2] = r;
                Bottom[i + 1] = g;
                Bottom[i] = b;
            }
            byte[] Top = new byte[origin.Format.BitsPerPixel * origin.PixelWidth / 8 * origin.PixelHeight];
            origin.CopyPixels(new Int32Rect(0, 0, origin.PixelWidth, origin.PixelHeight), Top, origin.Format.BitsPerPixel * origin.PixelWidth / 8, 0);
            for (int j = 0, x = left < 0 ? left : 0, y = top < 0 ? top : 0; j < origin.PixelHeight; j++)
            {
                for (int i = 0; i < origin.PixelWidth; i++)
                {
                    p1 = (WB.PixelWidth * (j - y) + i - x) * 4;
                    p2 = (origin.PixelWidth * j + i) * 4;
                    Bottom[p1 + 2] = (byte)((Top[p2 + 2] * Top[p2 + 3] + Bottom[p1 + 2] * Bottom[p1 + 3]) / 255 - Bottom[p1 + 2] * Bottom[p1 + 3] * Top[p2 + 3] / 65025);
                    Bottom[p1 + 1] = (byte)((Top[p2 + 1] * Top[p2 + 3] + Bottom[p1 + 1] * Bottom[p1 + 3]) / 255 - Bottom[p1 + 1] * Bottom[p1 + 3] * Top[p2 + 3] / 65025);
                    Bottom[p1] = (byte)((Top[p2] * Top[p2 + 3] + Bottom[p1] * Bottom[p1 + 3]) / 255 - Bottom[p1] * Bottom[p1 + 3] * Top[p2 + 3] / 65025);
                    Bottom[p1 + 3] = Bottom[p1 + 3] > Top[p2 + 3] ? Bottom[p1 + 3] : Top[p2 + 3];
                }
            }
            sec = PicResize(sec, width, height);
            Top = new byte[sec.Format.BitsPerPixel * sec.PixelWidth / 8 * sec.PixelHeight];
            sec.CopyPixels(new Int32Rect(0, 0, sec.PixelWidth, sec.PixelHeight), Top, sec.Format.BitsPerPixel * sec.PixelWidth / 8, 0);
            for (int j = 0, x = left < 0 ? 0 : left, y = top < 0 ? 0 : top; j < sec.PixelHeight; j++)
            {
                for (int i = 0; i < sec.PixelWidth; i++)
                {
                    p1 = (WB.PixelWidth * (j + y) + i + x) * 4;
                    p2 = (sec.PixelWidth * j + i) * 4;
                    Bottom[p1 + 2] = (byte)((Top[p2 + 2] * Top[p2 + 3] + Bottom[p1 + 2] * Bottom[p1 + 3]) / 255 - Bottom[p1 + 2] * Bottom[p1 + 3] * Top[p2 + 3] / 65025);
                    Bottom[p1 + 1] = (byte)((Top[p2 + 1] * Top[p2 + 3] + Bottom[p1 + 1] * Bottom[p1 + 3]) / 255 - Bottom[p1 + 1] * Bottom[p1 + 3] * Top[p2 + 3] / 65025);
                    Bottom[p1] = (byte)((Top[p2] * Top[p2 + 3] + Bottom[p1] * Bottom[p1 + 3]) / 255 - Bottom[p1] * Bottom[p1 + 3] * Top[p2 + 3] / 65025);
                    Bottom[p1 + 3] = Bottom[p1 + 3] > Top[p2 + 3] ? Bottom[p1 + 3] : Top[p2 + 3];
                }
            }
            WB.WritePixels(new Int32Rect(0, 0, W, H), Bottom, stride, 0);
            WB.Unlock();
            return WB;
        }
        /// <summary>
        /// 对比度/亮度
        /// </summary>
        /// <param name="buffer">源图像数据</param>
        /// <param name="brightness">亮度</param>
        /// <param name="contrast">对比度</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicBAC(byte[] buffer,double brightness,double contrast)
        {
            double B = brightness / 127;
            double K = Math.Tan((45 + 44 * contrast / 127) / 180 * Math.PI);
            double CR, CG, CB;
            for (int i = 0; i < buffer.Length; i += 4)
            {
                CR = (buffer[i + 2] - 127.5 * (1 - B)) * K + 127.5 * (1 + B);
                CG = (buffer[i + 1] - 127.5 * (1 - B)) * K + 127.5 * (1 + B);
                CB = (buffer[i] - 127.5 * (1 - B)) * K + 127.5 * (1 + B);
                buffer[i + 2] = (byte)(CR > 255 ? 255 : (CR < 0 ? 0 : CR));
                buffer[i + 1] = (byte)(CG > 255 ? 255 : (CG < 0 ? 0 : CG));
                buffer[i] = (byte)(CB > 255 ? 255 : (CB < 0 ? 0 : CB));
            }
            return buffer;
        }
        /// <summary>
        /// 色阶
        /// </summary>
        /// <param name="buffer">源图像数据</param>
        /// <param name="mode">模式:RGB,R,G,B</param>
        /// <param name="iblack">黑色输入值</param>
        /// <param name="igray">灰色输入值</param>
        /// <param name="iwhite">白色输入值</param>
        /// <param name="oblack">黑色输出值</param>
        /// <param name="owhite">白色输出值</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicColorScale(byte[] buffer, int mode, double iblack, double igray, double iwhite, double oblack, double owhite)
        {
            for (int i = 0; i < buffer.Length; i += 4)
            {
                if (mode == 0 || mode == 1)
                {
                    if (buffer[i + 2] < iblack)
                        buffer[i + 2] = (byte)oblack;
                    else if (buffer[i + 2] > iwhite)
                        buffer[i + 2] = (byte)owhite;
                    else
                        buffer[i + 2] = (byte)(oblack + (owhite - oblack) * Math.Pow((buffer[i + 2] - iblack) / (iwhite - iblack), 1 / igray));
                }
                if (mode == 0 || mode == 2)
                {
                    if (buffer[i + 1] < iblack)
                        buffer[i + 1] = (byte)oblack;
                    else if (buffer[i + 1] > iwhite)
                        buffer[i + 1] = (byte)owhite;
                    else
                        buffer[i + 1] = (byte)(oblack + (owhite - oblack) * Math.Pow((buffer[i + 1] - iblack) / (iwhite - iblack), 1 / igray));
                }
                if (mode == 0 || mode == 3)
                {
                    if (buffer[i] < iblack)
                        buffer[i] = (byte)oblack;
                    else if (buffer[i] > iwhite)
                        buffer[i] = (byte)owhite;
                    else
                        buffer[i] = (byte)(oblack + (owhite - oblack) * Math.Pow((buffer[i] - iblack) / (iwhite - iblack), 1 / igray));
                }
            }
            return buffer;
        }
        /// <summary>
        /// 色相/饱和度
        /// </summary>
        /// <param name="buffer">源图像数据</param>
        /// <param name="mode">模式(0:HSV,1:HSL)</param>
        /// <param name="hue">色相</param>
        /// <param name="saturation">饱和度</param>
        /// <param name="value">明度/亮度</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicHS(byte[] buffer, int mode, double hue, double saturation, double value)
        {
            double[] hs;
            int[] rgb;
            for (int i = 0; i < buffer.Length; i += 4)
            {
                hs = mode == 0 ? RGB_HSV(buffer[i + 2], buffer[i + 1], buffer[i]) : RGB_HSL(buffer[i + 2], buffer[i + 1], buffer[i]);
                hs[0] += hue;
                hs[1] *= (1 + saturation);
                hs[2] += value;
                hs[0] = (hs[0] < 0) ? (hs[0] + 360) : (hs[0] >= 360 ? (hs[0] % 360) : hs[0]);
                hs[1] = hs[1] < 0 ? 0 : (hs[1] > 1 ? 1 : hs[1]);
                hs[2] = hs[2] < 0 ? 0 : (hs[2] > 1 ? 1 : hs[2]);
                rgb = mode == 0 ? HSV_RGB(hs[0], hs[1], hs[2]) : HSL_RGB(hs[0], hs[1], hs[2]);
                buffer[i + 2] = (byte)rgb[0];
                buffer[i + 1] = (byte)rgb[1];
                buffer[i] = (byte)rgb[2];
            }
            return buffer;
        }
        /// <summary>
        /// 色调分离
        /// </summary>
        /// <param name="buffer">源图像数据</param>
        /// <param name="num">色调数</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicTS(byte[] buffer,int num)
        {
            int[] value = new int[num];
            double len = 255.0 / (num - 1);
            for (int i = 0; i < num; i++)
                value[i] = (int)(i * len + 0.5);
            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i + 2] = (byte)value[(int)(buffer[i + 2] / len + 0.5)];
                buffer[i + 1] = (byte)value[(int)(buffer[i + 1] / len + 0.5)];
                buffer[i] = (byte)value[(int)(buffer[i] / len + 0.5)];
            }
            return buffer;
        }
        /// <summary>
        /// 曲线
        /// </summary>
        /// <param name="origin">源图像</param>
        /// <param name="data">各通道曲线数据</param>
        /// <returns>BitmapSource</returns>
        public BitmapSource PicCurve(BitmapSource origin, int[][] data)
        {
            WriteableBitmap WB = new WriteableBitmap(origin);
            WB.Lock();
            byte[] buffer = new byte[WB.BackBufferStride * origin.PixelHeight];
            origin.CopyPixels(new Int32Rect(0, 0, origin.PixelWidth, origin.PixelHeight), buffer, WB.BackBufferStride, 0);
            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i + 2] = (byte)data[0][buffer[i + 2]];
                buffer[i + 1] = (byte)data[1][buffer[i + 1]];
                buffer[i] = (byte)data[2][buffer[i]];
            }
            WB.WritePixels(new Int32Rect(0, 0, WB.PixelWidth, WB.PixelHeight), buffer, WB.BackBufferStride, 0);
            WB.Unlock();
            return WB;
        }
        /// <summary>
        /// 曲线
        /// </summary>
        /// <param name="buffer">源图像数据</param>
        /// <param name="data">各通道曲线数据</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicCurve(byte[] buffer,int[][] data)
        {
            for (int i = 0; i < buffer.Length; i += 4)
            {
                buffer[i + 2] = (byte)data[0][buffer[i + 2]];
                buffer[i + 1] = (byte)data[1][buffer[i + 1]];
                buffer[i] = (byte)data[2][buffer[i]];
            }
            return buffer;
        }
        /// <summary>
        /// 锐化(属于高通滤波)
        /// </summary>
        /// <param name="buffer">源图像</param>
        /// <param name="coe">调节系数(范围[0,1])</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicSharpen(byte[] buffer, double coe,int width,int height)
        {
            byte[] buffer2 = buffer.Clone() as byte[];
            int[,] op = new int[,] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    double[] sum = new double[3] { 0, 0, 0 };
                    int p;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            p = ((y + j - 1) * width + x + i - 1) * 4;
                            sum[0] += buffer2[p + 2] * op[i, j] * coe;
                            sum[1] += buffer2[p + 1] * op[i, j] * coe;
                            sum[2] += buffer2[p] * op[i, j] * coe;
                        }
                    }
                    p = (y * width + x) * 4;
                    sum[0] += buffer[p + 2];
                    sum[1] += buffer[p + 1];
                    sum[2] += buffer[p];
                    buffer[p + 2] = (byte)(sum[0] < 0 ? 0 : (sum[0] > 255 ? 255 : sum[0]));
                    buffer[p + 1] = (byte)(sum[1] < 0 ? 0 : (sum[1] > 255 ? 255 : sum[1]));
                    buffer[p] = (byte)(sum[2] < 0 ? 0 : (sum[2] > 255 ? 255 : sum[2]));
                }
            }
            return buffer;
        }
        /// <summary>
        /// 均值滤波
        /// </summary>
        /// <param name="buffer">源图像数据</param>
        /// <param name="radius">半径(目标像素周围的像素圈数)</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="stride">一行所占字节数</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicMeanFilter(byte[] buffer, int radius,int width,int height,int stride)
        {
            byte[] buffer2 = buffer.Clone() as byte[];
            int p, s0, s1, s2;
            for (int y = 0; y < height; y++)
            {
                int[] sum = new int[3] { 0, 0, 0 };
                for (int x = 0; x < width; x++)
                {
                    if (x == 0)
                    {
                        for (int i = -radius; i <= radius; i++)
                        {
                            int X = i < 0 ? -i : i;
                            p = y * stride + X * 4;
                            sum[0] += buffer2[p + 2];
                            sum[1] += buffer2[p + 1];
                            sum[2] += buffer2[p];
                        }
                    }
                    else
                    {
                        int X = x - radius - 1;
                        X = X < 0 ? -X : X;
                        p = y * stride + X * 4;
                        sum[0] -= buffer2[p + 2];
                        sum[1] -= buffer2[p + 1];
                        sum[2] -= buffer2[p];
                        X = x + radius;
                        X = X >= width ? (width - 1 - X % (width - 1)) : X;
                        p = y * stride + X * 4;
                        sum[0] += buffer2[p + 2];
                        sum[1] += buffer2[p + 1];
                        sum[2] += buffer2[p];
                    }
                    s0 = sum[0] / (2 * radius + 1);
                    s1 = sum[1] / (2 * radius + 1);
                    s2 = sum[2] / (2 * radius + 1);
                    p = y * stride + x * 4;
                    buffer[p + 2] = (byte)(s0 < 0 ? 0 : (s0 > 255 ? 255 : s0));
                    buffer[p + 1] = (byte)(s1 < 0 ? 0 : (s1 > 255 ? 255 : s1));
                    buffer[p] = (byte)(s2 < 0 ? 0 : (s2 > 255 ? 255 : s2));
                }
            }
            for (int x = 0; x < width; x++)
            {
                int[] sum = new int[3] { 0, 0, 0 };
                for (int y = 0; y < height; y++)
                {
                    if (y == 0)
                    {
                        for (int j = -radius; j <= radius; j++)
                        {
                            int Y = j < 0 ? -j : j;
                            p = Y * stride + x * 4;
                            sum[0] += buffer[p + 2];
                            sum[1] += buffer[p + 1];
                            sum[2] += buffer[p];
                        }
                    }
                    else
                    {
                        int Y = y - radius - 1;
                        Y = Y < 0 ? -Y : Y;
                        p = Y * stride + x * 4;
                        sum[0] -= buffer[p + 2];
                        sum[1] -= buffer[p + 1];
                        sum[2] -= buffer[p];
                        Y = y + radius;
                        Y = Y >= height ? (height - 1 - Y % (height - 1)) : Y;
                        p = Y * stride + x * 4;
                        sum[0] += buffer[p + 2];
                        sum[1] += buffer[p + 1];
                        sum[2] += buffer[p];
                    }
                    s0 = sum[0] / (2 * radius + 1);
                    s1 = sum[1] / (2 * radius + 1);
                    s2 = sum[2] / (2 * radius + 1);
                    p = y * stride + x * 4;
                    buffer2[p + 2] = (byte)(s0 < 0 ? 0 : (s0 > 255 ? 255 : s0));
                    buffer2[p + 1] = (byte)(s1 < 0 ? 0 : (s1 > 255 ? 255 : s1));
                    buffer2[p] = (byte)(s2 < 0 ? 0 : (s2 > 255 ? 255 : s2));
                }
            }
           return buffer2;
        }
        /// <summary>
        /// 白平衡
        /// </summary>
        /// <param name="buffer">源图像数据</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicWhiteBalance(byte[] buffer,int width,int height)
        {
            double r_ave = 0, g_ave = 0, b_ave = 0;
            for (int i = 0; i < width * height; i++)
            {
                int p = i * 4;
                r_ave = (r_ave * i + buffer[p + 2]) / (i + 1);
                g_ave = (g_ave * i + buffer[p + 1]) / (i + 1);
                b_ave = (b_ave * i + buffer[p]) / (i + 1);
            }
            double gray = (r_ave + g_ave + b_ave) / 3;
            double kr = gray / r_ave, kg = gray / g_ave, kb = gray / b_ave;
            for (int i = 0; i < buffer.Length; i += 4)
            {
                double r = buffer[i + 2] * kr;
                double g = buffer[i + 1] * kg;
                double b = buffer[i] * kb;
                buffer[i + 2] = (byte)(r > 255 ? 255 : r);
                buffer[i + 1] = (byte)(g > 255 ? 255 : g);
                buffer[i] = (byte)(b > 255 ? 255 : b);
            }
            return buffer;
        }
        /// <summary>
        /// 像素化
        /// </summary>
        /// <param name="buffer">源图像数据</param>
        /// <param name="size">像素块大小</param>
        /// <param name="width">宽度 </param>
        /// <param name="height">高度</param>
        /// <param name="stride">一行所占字节数</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicPixelated(byte[] buffer, int size, int width, int height, int stride)
        {
            for (int y = 0; y < height;)
            {
                int ye = (y + size) > height ? height : (y + size);
                for (int x = 0; x < width;)
                {
                    int xe = (x + size) > width ? width : (x + size);
                    int[] sum = new int[3] { 0, 0, 0 };
                    for (int j = y; j < ye; j++)
                    {
                        for (int i = x; i < xe; i++)
                        {
                            int p = j * stride + i * 4;
                            sum[0] += buffer[p + 2];
                            sum[1] += buffer[p + 1];
                            sum[2] += buffer[p];
                        }
                    }
                    sum[0] /= ((ye - y) * (xe - x));
                    sum[1] /= ((ye - y) * (xe - x));
                    sum[2] /= ((ye - y) * (xe - x));
                    for (int j = y; j < ye; j++)
                    {
                        for (int i = x; i < xe; i++)
                        {
                            int p = j * stride + i * 4;
                            buffer[p + 2] = (byte)sum[0];
                            buffer[p + 1] = (byte)sum[1];
                            buffer[p] = (byte)sum[2];
                        }
                    }
                    x = xe;
                }
                y = ye;
            }
            return buffer;
        }
        /// <summary>
        /// 通道混合器
        /// </summary>
        /// <param name="buffer">源图像数据</param>
        /// <param name="red">红色通道数据</param>
        /// <param name="green">绿色通道数据</param>
        /// <param name="blue">蓝色通道数据</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicChannelMixer(byte[] buffer, double[] red, double[] green, double[] blue)
        {
            for (int i = 0; i < buffer.Length; i += 4)
            {
                double r = buffer[i + 2] * red[0] + buffer[i + 1] * red[1] + buffer[i] * red[2];
                double g = buffer[i + 2] * green[0] + buffer[i + 1] * green[1] + buffer[i] * green[2];
                double b = buffer[i + 2] * blue[0] + buffer[i + 1] * blue[1] + buffer[i] * blue[2];
                buffer[i + 2] = (byte)(r < 0 ? 0 : (r > 255 ? 255 : r));
                buffer[i + 1] = (byte)(g < 0 ? 0 : (g > 255 ? 255 : g));
                buffer[i] = (byte)(b < 0 ? 0 : (b > 255 ? 255 : b));
            }
            return buffer;
        }
        /// <summary>
        /// 提取分量
        /// </summary>
        /// <param name="buffer">源图像数据</param>
        /// <param name="channel">提取的通道</param>
        /// <returns>处理后的数据</returns>
        public byte[] PicExtractChannel(byte[] buffer,string channel)
        {
            if (channel == "red")
            {
                for (int i = 0; i < buffer.Length; i += 4)
                {
                    buffer[i + 1] = buffer[i + 2];
                    buffer[i] = buffer[i + 2];
                }
            }
            else if (channel == "green")
            {
                for (int i = 0; i < buffer.Length; i += 4)
                {
                    buffer[i + 2] = buffer[i + 1];
                    buffer[i] = buffer[i + 1];
                }
            }
            else if (channel == "blue")
            {
                for (int i = 0; i < buffer.Length; i += 4)
                {
                    buffer[i + 1] = buffer[i];
                    buffer[i + 2] = buffer[i];
                }
            }
            return buffer;
        }
        /// <summary>
        /// 获取(x,y)处像素值 (针对BitmapSource不能直接读取像素值)
        /// </summary>
        /// <param name="bs">源图像</param>
        /// <param name="x">x坐标(左边距)</param>
        /// <param name="y">y坐标(上边距)</param>
        /// <returns>System.Windows.Media.Color</returns>
        public Color GetPixelValue(BitmapSource bs, int x, int y)
        {
            //一个字节(byte)8位存储一个通道，四个通道需要4字节，按顺序分别为BGRA
            //对四通道图像截取宽为w，高为h后byte长度为w*h*4
            Int32Rect cut = new Int32Rect(x, y, 1, 1);
            int stride = bs.Format.BitsPerPixel * cut.Width / 8;
            byte[] data = new byte[cut.Height * stride];
            bs.CopyPixels(cut, data, stride, 0);
            return Color.FromArgb(data[3], data[2], data[1], data[0]);
        }
        /// <summary>
        /// 获取各通道0到255各个像素的个数
        /// </summary>
        /// <param name="bs">源图像</param>
        /// <param name="red">红色通道</param>
        /// <param name="green">绿色通道</param>
        /// <param name="blue">蓝色通道</param>
        /// <returns>三通道个数的最大值</returns>
        public int GetPixelCount(BitmapSource bs, int[] red, int[] green, int[] blue)
        {
            int max = 0;
            int stride = bs.Format.BitsPerPixel * bs.PixelWidth / 8;
            byte[] buffer = new byte[stride * bs.PixelHeight];
            bs.CopyPixels(new Int32Rect(0, 0, bs.PixelWidth, bs.PixelHeight), buffer, stride, 0);
            for (int i = 0; i < buffer.Length; i += 4)
            {
                red[buffer[i + 2]]++;
                green[buffer[i + 1]]++;
                blue[buffer[i]]++;
                max = red[buffer[i + 2]] > max ? red[buffer[i + 2]] : max;
                max = green[buffer[i + 2]] > max ? green[buffer[i + 2]] : max;
                max = blue[buffer[i + 2]] > max ? blue[buffer[i + 2]] : max;
            }
            return max;
        }
        /// <summary>
        /// 专用于[0,255]转两位十六进制的函数
        /// </summary>
        /// <param name="dec">十进制</param>
        /// <returns>string</returns>
        public string DecToHex(int dec)
        {
            char[] num = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            string hex = "";
            while (dec > 0)
            {
                hex = num[dec % 16] + hex;
                dec /= 16;
            }
            return hex == "" ? "00" : (hex.Length == 1 ? "0" + hex : hex);
        }
        /// <summary>
        /// 专用于两位十六进制转十进制的函数
        /// </summary>
        /// <param name="hex">十六进制</param>
        /// <returns>int</returns>
        private int HexToDec(string hex)
        {
            int sum = 0;
            for(int i = 1; i >= 0; i--)
            {
                if (hex[i] == 'A')
                    sum += 10 * (int)Math.Pow(16, 1 - i);
                else if (hex[i] == 'B')
                    sum += 11 * (int)Math.Pow(16, 1 - i);
                else if (hex[i] == 'C')
                    sum += 12 * (int)Math.Pow(16, 1 - i);
                else if (hex[i] == 'D')
                    sum += 13 * (int)Math.Pow(16, 1 - i);
                else if (hex[i] == 'E')
                    sum += 14 * (int)Math.Pow(16, 1 - i);
                else if (hex[i] == 'F')
                    sum += 15 * (int)Math.Pow(16, 1 - i);
                else
                    sum += int.Parse(hex[i].ToString()) * (int)Math.Pow(16, 1 - i);
            }
            return sum;
        }
        /// <summary>
        /// 基于特征向量的容差
        /// </summary>
        /// <param name="p1">像素1的特征向量，长度为4</param>
        /// <param name="p2">像素2的特征向量，长度为4</param>
        /// <returns>double</returns>
        private double PixelTolerance(double[] p1, double[] p2)
        {
            double temp = p1[0] * p2[0] + p1[1] * p2[1] + p1[2] * p2[2] + p1[3] * p2[3];
            if (temp == 0)
                return Math.Abs(p1[3] - p2[3]);
            double angle = Math.Sqrt(p1[0] * p1[0] + p1[1] * p1[1] + p1[2] * p1[2] + p1[3] * p1[3]) * Math.Sqrt(p2[0] * p2[0] + p2[1] * p2[1] + p2[2] * p2[2] + p2[3] * p2[3]);
            angle = Math.Acos(temp / angle) * 255 * 2 / Math.PI;
            return Math.Pow(2, Math.Abs((p1[3] > p2[3] ? p1[3] : p2[3]) / (p1[3] < p2[3] ? p1[3] : p2[3]) - 1)) * angle / 255;
        }
        /// <summary>
        /// RGB转HSV
        /// </summary>
        /// <param name="r">红色通道</param>
        /// <param name="g">绿色通道</param>
        /// <param name="b">蓝色通道</param>
        /// <returns>HSV</returns>
        private double[] RGB_HSV(int r, int g, int b)
        {
            double max = (r > g ? r : g) > b ? (r > g ? r : g) : b;
            double min = (r < g ? r : g) < b ? (r < g ? r : g) : b;
            double h = 0;
            if (max != min)
            {
                if (max == r)
                    h = 60 * (g - b) / (max - min) + (g >= b ? 0 : 360);
                else if (max == g)
                    h = 60 * (b - r) / (max - min) + 120;
                else if (max == b)
                    h = 60 * (r - g) / (max - min) + 240;
            }
            return new double[] { h, (max == 0 ? 0 : (1 - (double)min / max)), max / 255.0 };
        }
        /// <summary>
        /// HSV转RGB
        /// </summary>
        /// <param name="h">色相</param>
        /// <param name="s">饱和度</param>
        /// <param name="v">明度</param>
        /// <returns>RGB</returns>
        private int[] HSV_RGB(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;
            if (0 <= h && h < 60)
                return new int[] { (int)((c + m) * 255 + 0.5), (int)((x + m) * 255 + 0.5), (int)(m * 255 + 0.5) };
            else if (60 <= h && h < 120)
                return new int[] { (int)((x + m) * 255 + 0.5), (int)((c + m) * 255 + 0.5), (int)(m * 255 + 0.5) };
            else if (120 <= h && h < 180)
                return new int[] { (int)(m * 255 + 0.5), (int)((c + m) * 255 + 0.5), (int)((x + m) * 255 + 0.5) };
            else if (180 <= h && h < 240)
                return new int[] { (int)(m * 255 + 0.5), (int)((x + m) * 255 + 0.5), (int)((c + m) * 255 + 0.5) };
            else if (240 <= h && h < 300)
                return new int[] { (int)((x + m) * 255 + 0.5), (int)(m * 255 + 0.5), (int)((c + m) * 255 + 0.5) };
            else if (300 <= h && h < 360)
                return new int[] { (int)((c + m) * 255 + 0.5), (int)(m * 255 + 0.5), (int)((x + m) * 255 + 0.5) };
            return new int[] { 0, 0, 0 };
        }
        /// <summary>
        /// RGB转HSL
        /// </summary>
        /// <param name="r">红色通道</param>
        /// <param name="g">绿色通道</param>
        /// <param name="b">蓝色通道</param>
        /// <returns>HSL</returns>
        private double[] RGB_HSL(int r,int g,int b)
        {
            double max = (r > g ? r : g) > b ? (r > g ? r : g) : b;
            double min = (r < g ? r : g) < b ? (r < g ? r : g) : b;
            double h = 0, l = (max + min) / 510;
            if (max != min)
            {
                if (max == r)
                    h = 60 * (g - b) / (max - min) + (g >= b ? 0 : 360);
                else if (max == g)
                    h = 60 * (b - r) / (max - min) + 120;
                else if (max == b)
                    h = 60 * (r - g) / (max - min) + 240;
            }
            double s = l == 0 ? 0 : 0 < l && l <= 0.5 ? (max - min) / l * 0.5 : (max - min) / (2 - 2 * l);
            return new double[] { h, s / 255, l };
        }
        /// <summary>
        /// HSL转RGB
        /// </summary>
        /// <param name="h">色相</param>
        /// <param name="s">饱和度</param>
        /// <param name="l">亮度</param>
        /// <returns>RGB</returns>
        private int[] HSL_RGB(double h,double s,double l)
        {
            double q = l < 0.5 ? (l * (1 + s)) : (l + s - (l * s));
            double p = 2 * l - q;
            double hk = h / 360.0;
            double[] t = new double[] { hk + 0.33333333, hk, hk - 0.33333333 };
            for (int i = 0; i < 3; i++)
            {
                t[i] = t[i] < 0 ? (t[i] + 1) : (t[i] > 1 ? (t[i] - 1) : t[i]);
                if (t[i] < 0.16666666)
                    t[i] = p + ((q - p) * 6 * t[i]);
                else if (t[i] >= 0.16666666 && t[i] < 0.5)
                    t[i] = q;
                else if (t[i] >= 0.5 && t[i] < 0.66666666)
                    t[i] = p + ((q - p) * 6 * (0.66666666 - t[i]));
                else
                    t[i] = p;
            }
            return new int[] { (int)(t[0] * 255 + 0.5), (int)(t[1] * 255 + 0.5), (int)(t[2] * 255 + 0.5) };
        }
    }
    class PPoint
    {
        public int X;
        public int Y;
        public PPoint(int X,int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
