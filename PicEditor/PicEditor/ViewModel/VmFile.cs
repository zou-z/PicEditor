using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Win32;
using PicEditor.Interface;
using PicEditor.Model;
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
        private IPictureSource? pictureSource = null;
        private RelayCommand? openFileCommand = null; // openLink,openClipboard, drag in

        public RelayCommand OpenFileCommand => openFileCommand ??= new RelayCommand(OpenFile);

        public void Initialize(IPictureSource? pictureSource)
        {
            this.pictureSource = pictureSource;
        }

        private void OpenFile()
        {
            OpenFileDialog dialog = new()
            {
                Title = "打开本地文件",
                Multiselect = false,
            };
            if (dialog.ShowDialog() == true)
            {
                if (Basic.Util.FileUtil.GetFileExtendName(dialog.SafeFileName)?.ToLower() == "ppm")
                {

                }
                else
                {
                    var image = Basic.Util.FileUtil.ReadLocalFile(dialog.FileName);
                    pictureSource?.SetPictureSource(image);

                }
            }
        }

  

    }
}
