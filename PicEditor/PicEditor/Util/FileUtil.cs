using System;
using System.IO;
using PicEditor.Model;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace PicEditor.Util
{
    internal class FileUtil
    {
        // 获取文件扩展名
        public static string? GetFileExtendName(string fileName)
        {
            int p = fileName.LastIndexOf('.');
            return p == -1 ? null : fileName[(p + 1)..];
        }

        // 读取文件
        public static byte[]? ReadFile(string path)
        {
            byte[]? data = null;
            try
            {
                using FileStream fs = new(path, FileMode.Open);
                if (fs.Length > 1024 * 1024 * 100)
                {
                    throw new Exception("不支持打开超过100M的图片");
                }
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"打开文件失败\r\n{ex.Message}");
            }
            return data;
        }

        // 从字节流获取图像数据
        public static ImageData GetPixelsFromStream(byte[] stream)
        {
            ImageData imageData = new();
            BitmapImage? bi = new();
            try
            {
                using MemoryStream ms = new(stream);
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
                imageData.Width = bi.PixelWidth;
                imageData.Height = bi.PixelHeight;
                int stride = bi.PixelWidth * bi.Format.BitsPerPixel / 8;
                imageData.Pixels = new byte[stride * bi.PixelHeight];
                bi.CopyPixels(imageData.Pixels, stride, 0);
                imageData.DpiX = bi.DpiX;
                imageData.DpiY = bi.DpiY;
                imageData.PixelFormat = bi.Format;
                imageData.Palette = bi.Palette;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"获取图片像素失败\r\n{ex.Message}");
            }
            return imageData;
        }

        // ImageData转ImageSource
        public static ImageSource ImageDataToImageSource(ImageData data)
        {
            WriteableBitmap rb = new(data.Width, data.Height, data.DpiX, data.DpiY, data.PixelFormat, data.Palette);
            rb.Lock();
            int stride = data.Width * data.PixelFormat.BitsPerPixel / 8;
            Int32Rect rect = new(0, 0, data.Width, data.Height);
            rb.WritePixels(rect, data.Pixels, stride, 0);
            rb.AddDirtyRect(rect);
            rb.Unlock();
            return rb;
        }
    }
}
