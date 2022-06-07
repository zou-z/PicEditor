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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.ViewModel
{
    internal class VmLayer : ILayerManage
    {
        private const double unitLeft = 30;
        private ObservableCollection<LayerBase>? layers = null;
        private RelayCommand? addLayerGroupCommand = null;
        private RelayCommand? addLayerPictureCommand = null;
        private RelayCommand? deleteSelectedLayerCommand = null;

        public ObservableCollection<LayerBase> Layers => layers ??= new ObservableCollection<LayerBase>();

        public LayerBase? SelectedLayer { get; set; }

        public RelayCommand AddLayerGroupCommand => addLayerGroupCommand ??= new RelayCommand(AddLayerGroup);

        public RelayCommand AddLayerPictureCommand => addLayerPictureCommand ??= new RelayCommand(AddLayerPicture);

        public RelayCommand DeleteSelectedLayerCommand => deleteSelectedLayerCommand ??= new RelayCommand(DeleteSelectedLayer);

        public VmLayer()
        {

        }

        public void Relocation(LayerBase source, LayerBase target)
        {
            ObservableCollection<LayerBase>? sourceCollection = FindCollection(Layers, source);
            ObservableCollection<LayerBase>? targetCollection = FindCollection(Layers, target);
            if (sourceCollection == null || targetCollection == null)
            {
                LogUtil.Log.Error(new Exception("未完成拖拽"), $"{(sourceCollection == null ? "sourceCollection is null," : "")}{(targetCollection == null ? "targetCollection is null" : "")}");
                return;
            }

            if (source is LayerPicture picture)
            {
                if (target is LayerPicture)
                {
                    int index = targetCollection.IndexOf(target);
                    sourceCollection.Remove(source);
                    targetCollection.Insert(index, source);
                    source.MarginLeft = target.MarginLeft;
                }
                else if (target is LayerGroup group)
                {
                    sourceCollection.Remove(source);
                    source.MarginLeft = target.MarginLeft + unitLeft;
                    group.Children.Add(source);
                }
                picture.IsSelected = true;
            }
            else if (source is LayerGroup sourceGroup)
            {
                if (FindCollection(sourceGroup.Children, target) != null)
                {
                    MessageBox.Show("移动失败，不能将该组移动到其下面");
                    return;
                }
                if (target is LayerPicture)
                {
                    int index = targetCollection.IndexOf(target);
                    sourceCollection.Remove(source);
                    targetCollection.Insert(index, source);
                    source.MarginLeft = target.MarginLeft;
                    foreach (LayerBase layer in sourceGroup.Children)
                    {
                        layer.MarginLeft = target.MarginLeft + unitLeft;
                    }
                }
                else if (target is LayerGroup targetGroup)
                {
                    sourceCollection.Remove(source);
                    source.MarginLeft = target.MarginLeft + unitLeft;
                    double left = source.MarginLeft + unitLeft;
                    foreach (LayerBase layer in sourceGroup.Children)
                    {
                        layer.MarginLeft = left;
                    }
                    targetGroup.Children.Add(source);
                }
            }
        }

        public void Relocation(LayerBase source)
        {
            ObservableCollection<LayerBase>? sourceCollection = FindCollection(Layers, source);
            if (sourceCollection == null)
            {
                LogUtil.Log.Error(new Exception("未完成拖拽"), $"{(sourceCollection == null ? "sourceCollection is null" : "")}");
                return;
            }

            sourceCollection.Remove(source);
            source.MarginLeft = 0;
            if (source is LayerGroup group)
            {
                foreach (LayerBase layerBase in group.Children)
                {
                    layerBase.MarginLeft = unitLeft;
                }
            }
            Layers.Add(source);
        }

        private void SelectedChanged(LayerPicture picture)
        {
            if (!picture.Equals(SelectedLayer) && SelectedLayer != null && SelectedLayer is LayerPicture _picture && _picture != null)
            {
                _picture.SetIsSelected(false);
            }
            SelectedLayer = picture;
        }

        private void LayerGroupSelected()
        {
            if (SelectedLayer != null && SelectedLayer is LayerPicture picture && picture != null)
            {
                picture.SetIsSelected(true);
            }
        }

        int groupIndex = 0;
        private void AddLayerGroup()
        {
            var group = new LayerGroup
            {
                LayerName = $"Group {++groupIndex}",
                LayerType = LayerTypes.Group,
                MarginLeft = 0,
            };
            group.LayerGroupSelected += LayerGroupSelected;
            Layers.Add(group);
        }

        int pictureIndex = 0;
        private void AddLayerPicture()
        {
            var picture = new LayerPicture
            {
                LayerName = $"picture layer {++pictureIndex}",
                LayerType = LayerTypes.Picture,
                Thumbnail = null,
                MarginLeft = 0,
            };
            picture.SelectedChanged += SelectedChanged;
            Layers.Add(picture);
        }

        public void AddLayerPicture(VisualBrush brush, string guid)
        {
            var picture = new LayerPicture
            {
                Guid = guid,
                LayerName = $"picture layer {++pictureIndex}",
                LayerType = LayerTypes.Picture,
                Thumbnail = brush,
                MarginLeft = 0,
            };
            picture.SelectedChanged += SelectedChanged;
            Layers.Add(picture);
        }

        private void DeleteSelectedLayer()
        {
            if (SelectedLayer == null)
            {
                MessageBox.Show("当前未选择任何图层");
                return;
            }
            if (SelectedLayer is LayerPicture picture)
            {
                picture.SelectedChanged -= SelectedChanged;
            }
            DeleteLayer(SelectedLayer);
            SelectedLayer = null;
        }

        private void DeleteLayer(LayerBase layer)
        {
            ObservableCollection<LayerBase>? collection = FindCollection(Layers, layer);
            collection?.Remove(layer);
        }

        private ObservableCollection<LayerBase>? FindCollection(ObservableCollection<LayerBase> collection, LayerBase source)
        {
            for (int i = 0; i < collection.Count; ++i)
            {
                if (collection[i].Equals(source))
                {
                    return collection;
                }
                if (collection[i] is LayerGroup group && group != null)
                {
                    ObservableCollection<LayerBase>? _collection = FindCollection(group.Children, source);
                    if (_collection != null)
                    {
                        return _collection;
                    }
                }
            }
            return null;
        }
    }
}
