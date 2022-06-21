using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.Basic.Util
{
    public class BitmapUtil
    {
        // 获取透明图像
        public static WriteableBitmap GetTransparentBitmap(int width, int height, double dpiX = 96, double dpiY = 96)
        {
            var bitmap = new WriteableBitmap(width, height, dpiX, dpiY, PixelFormats.Bgra32, null);
            int stride = width * bitmap.Format.BitsPerPixel / 8;
            byte[] pixels = new byte[stride * height];
            //for (int i = 0; i < pixels.Length; ++i)
            //{
            //    pixels[i] = (byte)(i % 4 == 3 ? 0 : 255);
            //}
            int p = 0;
            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = (byte)((++p) % 255);
            }
            var rect = new Int32Rect(0, 0, width, height);
            bitmap.Lock();
            bitmap.WritePixels(rect, pixels, stride, 0);
            bitmap.Unlock();
            return bitmap;
        }

        public static WriteableBitmap Rotate(WriteableBitmap bitmap, int rotateDirection)
        {
            int stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
            byte[] pixels = new byte[bitmap.PixelHeight * stride];
            bitmap.CopyPixels(pixels, stride, 0);
            int num = bitmap.Format.BitsPerPixel / 8;

            byte[] pixelsTarget = new byte[pixels.Length];
            if (rotateDirection == -90)
            {
                for (int x = bitmap.PixelWidth - 1; x >= 0; --x)
                {
                    for (int y = 0; y < bitmap.PixelHeight; ++y)
                    {
                        int p1 = (y * bitmap.PixelWidth + x) * num;
                        int p2 = ((bitmap.PixelWidth - 1 - x) * bitmap.PixelHeight + y) * num;
                        for (int i = 0; i < num; ++i)
                        {
                            pixelsTarget[p2 + i] = pixels[p1 + i];
                        }
                    }
                }
            }
            else if (rotateDirection == 90)
            {
                for (int y = bitmap.PixelHeight - 1; y >= 0; --y)
                {
                    for (int x = 0; x < bitmap.PixelWidth; ++x)
                    {
                        int p1 = (y * bitmap.PixelWidth + x) * num;
                        int p2 = (x * bitmap.PixelHeight + bitmap.PixelHeight - 1 - y) * num;
                        for (int i = 0; i < num; ++i)
                        {
                            pixelsTarget[p2 + i] = pixels[p1 + i];
                        }
                    }
                }
            }

            bitmap = new WriteableBitmap(bitmap.PixelHeight, bitmap.PixelWidth, bitmap.DpiX, bitmap.DpiY, bitmap.Format, bitmap.Palette);
            stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
            var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            bitmap.Lock();
            bitmap.WritePixels(rect, pixelsTarget, stride, 0);
            bitmap.AddDirtyRect(rect);
            bitmap.Unlock();
            return bitmap;
        }

        // 1水平,2垂直
        public static void Mirror(WriteableBitmap bitmap, int mode)
        {
            int stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
            byte[] pixels = new byte[bitmap.PixelHeight * stride];
            bitmap.CopyPixels(pixels, stride, 0);
            int num = bitmap.Format.BitsPerPixel / 8;

            if (mode == 1)
            {
                for (int y = 0; y < bitmap.PixelHeight; ++y)
                {
                    for (int x = 0; x < (bitmap.PixelWidth + 1) / 2; ++x)
                    {
                        int p1 = (bitmap.PixelWidth * y + x) * num;
                        int p2 = (bitmap.PixelWidth * y + bitmap.PixelWidth - 1 - x) * num;
                        for (int i = 0; i < num; ++i)
                        {
                            (pixels[p1 + i], pixels[p2 + i]) = (pixels[p2 + i], pixels[p1 + i]);
                        }
                    }
                }
            }
            else if (mode == 2)
            {
                for (int x = 0; x < bitmap.PixelWidth; ++x)
                {
                    for (int y = 0; y < (bitmap.PixelHeight + 1) / 2; ++y)
                    {
                        int p1 = (bitmap.PixelWidth * y + x) * num;
                        int p2 = (bitmap.PixelWidth * (bitmap.PixelHeight - 1 - y) + x) * num;
                        for (int i = 0; i < num; ++i)
                        {
                            (pixels[p1 + i], pixels[p2 + i]) = (pixels[p2 + i], pixels[p1 + i]);
                        }
                    }
                }
            }

            var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            bitmap.Lock();
            bitmap.WritePixels(rect, pixels, stride, 0);
            bitmap.AddDirtyRect(rect);
            bitmap.Unlock();
        }

        public static BitmapSource Resize(BitmapSource origin, int width, int height)
        {
            var tb = new TransformedBitmap();
            tb.BeginInit();
            tb.Source = origin;
            var st = new ScaleTransform((double)width / origin.PixelWidth, (double)height / origin.PixelHeight);
            tb.Transform = st;
            tb.EndInit();
            return tb;
        }

        public static BitmapSource Crop(BitmapSource origin, int left, int top, int width, int height)
        {
            BitmapSource bs = new CroppedBitmap(origin, new Int32Rect(left, top, width, height));
            return bs;
        }

        public static WriteableBitmap MoveAndResize(int targetWidth, int targetHeight, WriteableBitmap bitmap, int left, int top)
        {
            WriteableBitmap targetBitmap = GetTransparentBitmap(targetWidth, targetHeight, bitmap.DpiX, bitmap.DpiY);
            int targetLeft = left < 0 ? 0 : left, targetTop = top < 0 ? 0 : top;
            int sourceLeft = left < 0 ? -left : 0, sourceTop = top < 0 ? -top : 0;
            int sourceWidth = left + bitmap.PixelWidth > targetWidth ? (targetWidth - Math.Abs(left)) : (bitmap.PixelWidth - Math.Abs(left));
            int sourceHeight = top + bitmap.PixelHeight > targetHeight ? (targetHeight - Math.Abs(top)) : (bitmap.PixelHeight - Math.Abs(top));
            if (sourceWidth <= 0 || sourceHeight <= 0)
            {
                return targetBitmap;
            }

            int num = bitmap.Format.BitsPerPixel / 8;
            int stride = sourceWidth * num;
            byte[] pixels = new byte[sourceHeight * stride];
            bitmap.CopyPixels(new Int32Rect(sourceLeft, sourceTop, sourceWidth, sourceHeight), pixels, stride, 0);

            var rect = new Int32Rect(targetLeft, targetTop, sourceWidth, sourceHeight);
            targetBitmap.Lock();
            targetBitmap.WritePixels(rect, pixels, stride, 0);
            targetBitmap.AddDirtyRect(rect);
            targetBitmap.Unlock();
            return targetBitmap;
        }

        public static void ShowImage(byte[] pixels, int width, int height)
        {
            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            bitmap.Lock();
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * bitmap.Format.BitsPerPixel / 8, 0);
            bitmap.Unlock();
            ShowImage(bitmap);
        }

        public static void ShowImage(WriteableBitmap bitmap)
        {
            var window = new Window
            {
                Title = $"{bitmap.PixelWidth} × {bitmap.PixelHeight} [{DateTime.Now}]",
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.WidthAndHeight,
                Content = new Image
                {
                    Width = bitmap.PixelWidth,
                    Height = bitmap.PixelHeight,
                    Source = bitmap,
                },
            };
            window.Show();
        }
    }
}
