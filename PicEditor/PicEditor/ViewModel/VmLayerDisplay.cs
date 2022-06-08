using PicEditor.Basic.Util;
using PicEditor.Interface;
using PicEditor.Layer;
using PicEditor.Model.Layer;
using PicEditor.View.Control;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PicEditor.ViewModel
{
    internal class VmLayerDisplay : IPictureSource, ILayerDisplay
    {
        private ILayerManage? layerManage = null;
        private LayerInfo? layerInfo = null;
        private readonly ObservableCollection<ILayer> pictureLayers = new();
        private readonly ObservableCollection<ILayer> upperLayers = new();

        public LayerInfo LayerInfo => layerInfo ??= new LayerInfo();

        public ObservableCollection<ILayer> PictureLayers => pictureLayers;

        public ObservableCollection<ILayer> UpperLayers => upperLayers;

        public void Initialize(ILayerManage layerManage)
        {
            this.layerManage = layerManage;
        }

        public void SetPictureSource(WriteableBitmap bitmap)
        {
            if (PictureLayers.Count == 0)
            {
                PictureLayers.Clear();
                LayerInfo.CanvasSize = new Size(bitmap.PixelWidth, bitmap.PixelHeight);
            }
            string guid = GuidUtil.GetGuid();
            var item = new PictureLayer(bitmap) { Guid = guid };
            PictureLayers.Add(item);
            LayerInfo.Scale = 1.0;

            layerManage?.AddLayer(guid, item.GetVisualBrush());
            layerManage?.SetLayerSize(guid, bitmap.PixelWidth, bitmap.PixelHeight);
        }

        public void SetLayerVisible(string guid, Visibility visibility)
        {
            foreach (PictureLayer layer in PictureLayers)
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
