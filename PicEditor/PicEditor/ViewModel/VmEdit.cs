using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using PicEditor.Interface;
using PicEditor.Model;
using PicEditor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicEditor.ViewModel
{
    internal class VmEdit : IEdit
    {
        private ImageData? imageData = null;
        private Picture? picture = null;
        private EditArea? editArea = null;

        public Picture Picture => picture ??= new Picture();

        public EditArea EditArea => editArea ??= new EditArea();

        public void Initialize()
        {

        }

        public void SetPicture(ImageData imageData)
        {
            Picture.Width = imageData.Width * EditArea.Scale;
            Picture.Height = imageData.Height * EditArea.Scale;
            Picture.Source = FileUtil.ImageDataToImageSource(imageData);
            EditArea.Scale = 1.0;
            if (this.imageData != null && this.imageData.Pixels != null)
            {
                this.imageData.Pixels = null;
            }
            this.imageData = imageData;
        }
    }
}
