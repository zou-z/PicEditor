using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using PicEditor.Interface;
using PicEditor.Model;
using PicEditor.Util;
using PicEditor.View.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PicEditor.ViewModel
{
    internal class VmEdit : IEdit
    {
        private EditArea? editArea = null;
        private Layers? layers = null;

        public EditArea EditArea => editArea ??= new EditArea();

        public Layers Layers => layers ??= new Layers();

        public void SetPicture(ImageData imageData)
        {
            Layers.PictureLayers.Clear();
            Layers.PictureLayers.Add(new PictureLayer(imageData));
            Layers.CanvasSize = new Size(imageData.Width, imageData.Height);
            EditArea.Scale = 1.0;



        }
    }
}
