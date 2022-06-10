using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Win32;
using PicEditor.Basic.Util;
using PicEditor.Interface;
using PicEditor.Model;
using PicEditor.Model.PictureInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PicEditor.ViewModel
{
    internal class VmFile : ObservableObject
    {
        private IPictureSource? pictureSource = null;
        private PictureSourceInfo? pictureSourceInfo = null;
        private RelayCommand<string>? openFileCommand = null; // openLink,openClipboard, drag in
        private bool isFunctionOn = false;

        public PictureSourceInfo PictureSourceInfo => pictureSourceInfo ??= new PictureSourceInfo();

        public RelayCommand<string> OpenFileCommand => openFileCommand ??= new RelayCommand<string>(OpenFile);

        public bool IsFunctionOn
        {
            get => isFunctionOn;
            set => SetProperty(ref isFunctionOn, value);
        }

        public void Initialize(IPictureSource? pictureSource)
        {
            this.pictureSource = pictureSource;
        }

        private void OpenFile(string? mode)
        {
            OpenFileDialog dialog = new()
            {
                Title = "打开本地文件",
                Multiselect = false,
            };
            if (dialog.ShowDialog() == true)
            {
                if (FileUtil.GetFileExtendName(dialog.SafeFileName)?.ToLower() == "ppm")
                {

                }
                else
                {
                    var bitmap = FileUtil.ReadLocalFile(dialog.FileName);
                    pictureSource?.AddPictureSource(bitmap, GetIsInit(mode));
                }
                IsFunctionOn = GetIsInit(mode);
            }
        }

        private static bool GetIsInit(string? mode)
        {
            return mode == "Open";
        }
    }
}
