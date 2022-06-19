using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Win32;
using PicEditor.Basic.Util;
using PicEditor.Interface;
using PicEditor.Model;
using PicEditor.Model.PictureData;
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
        private PictureStatus? status = null;
        private PictureSourceInfo? pictureSourceInfo = null;
        private RelayCommand<string>? openFileCommand = null; // openLink,openClipboard, drag in

        public PictureStatus Status => status ??= new PictureStatus();

        public PictureSourceInfo PictureSourceInfo => pictureSourceInfo ??= new PictureSourceInfo();

        public RelayCommand<string> OpenFileCommand => openFileCommand ??= new RelayCommand<string>(OpenFile);

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
                    pictureSource?.AddPictureSource(bitmap, !Status.IsPictureOpened);
                }
                if (Status.IsPictureOpened)
                {
                    Status.IsInsertingPicture = true;
                }
                else
                {
                    Status.IsPictureOpened = Status.IsFunctionOn = true;
                }
            }
        }
    }
}
