using Microsoft.Toolkit.Mvvm.ComponentModel;
using PicEditor.Layer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PicEditor.Model
{
    internal class Layers : ObservableObject
    {
        private readonly ObservableCollection<ILayer> pictureLayers = new();
        private readonly ObservableCollection<ILayer> upperLayers = new();
        private ILayer? selectedLayer = null;
        private Size canvasSize = new Size(0, 0);

        public ObservableCollection<ILayer> PictureLayers => pictureLayers;

        public ObservableCollection<ILayer> UpperLayers => upperLayers;

        public Size CanvasSize
        {
            get => canvasSize;
            set => SetProperty(ref canvasSize, value);
        }

        public ILayer? SelectedLayer
        {
            get => selectedLayer;
            set => SetProperty(ref selectedLayer, value);
        }
    }
}
