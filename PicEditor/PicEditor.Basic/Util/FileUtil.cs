using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        //// ImageData转ImageSource
        //public static ImageSource ImageDataToImageSource(ImageData data)
        //{
        //    WriteableBitmap rb = new(data.Width, data.Height, data.DpiX, data.DpiY, data.PixelFormat, data.Palette);
        //    rb.Lock();
        //    int stride = data.Width * data.PixelFormat.BitsPerPixel / 8;
        //    Int32Rect rect = new(0, 0, data.Width, data.Height);
        //    rb.WritePixels(rect, data.Pixels, stride, 0);
        //    rb.AddDirtyRect(rect);
        //    rb.Unlock();
        //    return rb;
        //}
    }
}
