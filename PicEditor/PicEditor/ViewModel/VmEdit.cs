using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using PicEditor.Basic.Util;
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
    internal class VmEdit : IEdit, ILayerDisplay
    {
        private ILayerManage? layerManage;
        private EditArea? editArea = null;
        private Layers? layers = null;

        public EditArea EditArea => editArea ??= new EditArea();

        public Layers Layers => layers ??= new Layers();

        public void Initialize(ILayerManage layerManage)
        {
            this.layerManage = layerManage;
        }

        public void SetPicture(ImageData imageData)
        {
            if (Layers.PictureLayers.Count == 0)
            {
                Layers.PictureLayers.Clear();
                Layers.CanvasSize = new Size(imageData.Width, imageData.Height);
            }
            string guid = GuidUtil.GetGuid();
            var item = new PictureLayer(imageData) { Guid = guid };
            Layers.PictureLayers.Add(item);
            EditArea.Scale = 1.0;

            layerManage?.AddLayerPicture(item.GetVisualBrush(), guid);

        }
    }
}
