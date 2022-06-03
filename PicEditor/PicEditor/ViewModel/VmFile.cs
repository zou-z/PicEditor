using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Win32;
using PicEditor.Interface;
using PicEditor.Model;
using PicEditor.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PicEditor.ViewModel
{
    internal class VmFile
    {
        private IEdit? edit = null;
        private RelayCommand? openFileCommand = null; // openLink,openClipboard, drag in

        public RelayCommand OpenFileCommand => openFileCommand ??= new RelayCommand(OpenFile);

        public void Initialize(IEdit edit)
        {
            this.edit = edit;
        }

        private void OpenFile()
        {
            OpenFileDialog dialog = new()
            {
                Title = "打开本地文件",
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {
                byte[]? data = FileUtil.ReadFile(dialog.FileName);
                if (data != null)
                {
                    if (FileUtil.GetFileExtendName(dialog.SafeFileName)?.ToLower() == "ppm")
                    {



                    }
                    else
                    {
                        ImageData imageData = FileUtil.GetPixelsFromStream(data);
                        edit?.SetPicture(imageData);
                    }
#pragma warning disable IDE0059 // 不需要赋值
                    data = null;
#pragma warning restore IDE0059 // 不需要赋值
                    GC.Collect();
                }
            }
        }

  

    }
}
