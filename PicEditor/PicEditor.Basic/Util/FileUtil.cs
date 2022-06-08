using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.Basic.Util
{
    public class FileUtil
    {
        // 获取文件扩展名
        public static string? GetFileExtendName(string fileName)
        {
            int p = fileName.LastIndexOf('.');
            return p == -1 ? null : fileName[(p + 1)..];
        }

        // 打开本地文件
        public static WriteableBitmap ReadLocalFile(string path)
        {
            try
            {
                using FileStream fs = new(path, FileMode.Open);
                if (fs.Length > 1024 * 1024 * 100)
                {
                    throw new Exception("不支持打开超过100M的图片");
                }
                BitmapImage bi = new();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = fs;
                bi.EndInit();
                return new WriteableBitmap(bi);
            }
            catch
            {
                throw;
            }
        }

        // 获取透明图像
        public static WriteableBitmap GetTransparentBitmap(int width, int height)
        {
            PixelFormat format = PixelFormats.Bgra32;
            var bitmap = new WriteableBitmap(width, height, 72, 72, format, null);
            bitmap.Lock();
            int stride = width * format.BitsPerPixel / 8;
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
            bitmap.WritePixels(rect, pixels, stride, 0);
            bitmap.AddDirtyRect(rect);
            bitmap.Unlock();
            return bitmap;
        }
    }
}
