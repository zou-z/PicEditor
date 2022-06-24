using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace PicEditor.Model.Layer
{
    internal class LayerInfo : ObservableObject
    {
        private string name = string.Empty;
        private bool isSelected = false;
        private bool isVisible = true;
        private Brush? thumbnail = null;
        private Size thumbnailSize = new(0, 0);

        public event Action<LayerInfo>? IsVisibleChanged = null;

        public string ID = string.Empty;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
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

        public Brush? Thumbnail
        {
            get => thumbnail;
            set => SetProperty(ref thumbnail, value);
        }

        public Size ThumbnailSize
        {
            get => thumbnailSize;
            set => SetProperty(ref thumbnailSize, value);
        }
    }
}
