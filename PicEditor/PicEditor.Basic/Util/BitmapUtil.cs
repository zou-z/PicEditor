using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.Basic.Util
{
    public class BitmapUtil
    {
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
    }
}
