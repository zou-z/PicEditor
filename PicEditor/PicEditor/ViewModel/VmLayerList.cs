using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PicEditor.Basic.Util;
using PicEditor.Interface;
using PicEditor.Model.Layer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PicEditor.ViewModel
{
    internal class VmLayerList : ObservableObject, ILayerList
    {
        private ILayers? layers = null;
        private ObservableCollection<LayerInfo>? allLayers = null;
        private LayerInfo? selectedLayer = null;
        private int layerIndex = 0;
        private const double thumbnailSize = 28;
        private RelayCommand<LayerInfo>? addLayerCommand = null;

        public ObservableCollection<LayerInfo> Layers => allLayers ??= new ObservableCollection<LayerInfo>();

        public LayerInfo? SelectedLayer
        {
            get => selectedLayer;
            set => SetProperty(ref selectedLayer, value);
        }

        public RelayCommand<LayerInfo> AddLayerCommand => addLayerCommand ??= new RelayCommand<LayerInfo>(AddLayer);

        public void Initialize(ILayers layers)
        {
            this.layers = layers;
        }

        public void AddLayer(string id, VisualBrush brush)
        {
            var layer = new LayerInfo
            {
                ID = id,
                Name = $"图层 {++layerIndex}",
                Thumbnail = brush,
                ThumbnailSize = GetThumbnailSize(),
            };
            layer.IsVisibleChanged += Layer_IsVisibleChanged;
            Layers.Insert(0, layer);
        }

        private void AddLayer(LayerInfo? layerInfo)
        {
            var layer = new LayerInfo
            {
                ID = GuidUtil.GetGuid(),
                Name = $"图层 {++layerIndex}",
                Thumbnail = null,
                ThumbnailSize = GetThumbnailSize(),
            };
            layer.IsVisibleChanged += Layer_IsVisibleChanged;
            int index = layerInfo == null ? 0 : Layers.IndexOf(layerInfo);
            Layers.Insert(index < 0 ? 0 : index, layer);
            layer.Thumbnail = layers?.AddLayer(layer.ID, null);
        }

        private void Layer_IsVisibleChanged(LayerInfo layerInfo)
        {
            
        }

        private Size GetThumbnailSize()
        {
            if (layers == null)
            {
                return new Size(0, 0);
            }
            Size size = layers.GetCanvasSize();
            if (size.Width >= size.Height)
            {
                return new Size(thumbnailSize, thumbnailSize * size.Height / size.Width);
            }
            else
            {
                return new Size(thumbnailSize * size.Width / size.Height, thumbnailSize);
            }
        }
    }
}
