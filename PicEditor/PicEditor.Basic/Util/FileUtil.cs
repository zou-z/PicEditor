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
    }
}
