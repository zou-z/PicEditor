using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace PicEditor.Model.Layer
{
    public enum LayerTypes
    {
        Group,
        Picture
    }

    internal class LayerBase : ObservableObject
    {
        private string layerName = string.Empty;
        private LayerTypes layerType;
        private bool isVisible = true;
        private double marginLeft = 0;

        public event Action<LayerBase>? IsVisibleChanged = null;

        public string LayerName
        {
            get => layerName;
            set => SetProperty(ref layerName, value);
        }

        public LayerTypes LayerType
        {
            get => layerType;
            set => SetProperty(ref layerType, value);
        }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                SetProperty(ref isVisible, value);
                IsVisibleChanged?.Invoke(this);
            }
        }

        public double MarginLeft
        {
            get => marginLeft;
            set => SetProperty(ref marginLeft, value);
        }
    }
}
