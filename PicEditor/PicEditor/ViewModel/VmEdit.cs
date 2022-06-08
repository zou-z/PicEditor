using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using PicEditor.Basic.Util;
using PicEditor.Interface;
using PicEditor.Model;
using PicEditor.View.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PicEditor.ViewModel
{
    internal class VmEdit : IEdit, ILayerDisplay
    {
        private ILayerManage? layerManage = null;
        private EditArea? editArea = null;
        private Layers? layers = null;

        public EditArea EditArea => editArea ??= new EditArea();

        public Layers Layers => layers ??= new Layers();

        public void Initialize(ILayerManage layerManage)
        {
            this.layerManage = layerManage;
        }

        public void SetPicture(WriteableBitmap wb)
        {
            if (Layers.PictureLayers.Count == 0)
            {
                Layers.PictureLayers.Clear();
                Layers.CanvasSize = new Size(wb.PixelWidth, wb.PixelHeight);
            }
            string guid = GuidUtil.GetGuid();
            var item = new PictureLayer(wb) { Guid = guid };
            Layers.PictureLayers.Add(item);
            EditArea.Scale = 1.0;

            layerManage?.AddLayer(guid, item.GetVisualBrush());
            layerManage?.SetLayerSize(guid, wb.PixelWidth, wb.PixelHeight);
        }

        public void SetLayerVisible(string guid, Visibility visibility)
        {
            foreach (PictureLayer layer in Layers.PictureLayers)
            {
                if (layer.Guid == guid)
                {
                    layer.Visibility = visibility;
                    return;
                }
            }
        }
    }
}
