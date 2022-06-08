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
    internal class VmLayerManage : ILayerManage
    {
        private ILayerDisplay? layerDisplay = null;
        private const double unitLeft = 30;
        private int groupIndex = 0;
        private int layerIndex = 0;
        private readonly CollectionUtil collectionUtil = new();
        private ObservableCollection<LayerBase>? layers = null;
        private RelayCommand? addLayerGroupCommand = null;
        private RelayCommand<LayerBase>? addLayerPictureCommand = null;
        private RelayCommand? deleteSelectedLayerCommand = null;

        public ObservableCollection<LayerBase> Layers => layers ??= new ObservableCollection<LayerBase>();

        public LayerBase? SelectedLayer { get; set; }

        public RelayCommand AddLayerGroupCommand => addLayerGroupCommand ??= new RelayCommand(AddLayerGroup);

        public RelayCommand<LayerBase> AddLayerPictureCommand => addLayerPictureCommand ??= new RelayCommand<LayerBase>(AddLayer);

        public RelayCommand DeleteSelectedLayerCommand => deleteSelectedLayerCommand ??= new RelayCommand(DeleteSelectedLayer);

        public VmLayerManage()
        {
        }

        public void Initialize(ILayerDisplay layerDisplay)
        {
            this.layerDisplay = layerDisplay;
        }

        #region 拖拽改变位置
        public void Relocation(LayerBase source, LayerBase target)
        {
            ObservableCollection<LayerBase>? sourceCollection = collectionUtil.FindCollection(Layers, source);
            ObservableCollection<LayerBase>? targetCollection = collectionUtil.FindCollection(Layers, target);
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
                if (collectionUtil.FindCollection(sourceGroup.Children, target) != null)
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
            ObservableCollection<LayerBase>? sourceCollection = collectionUtil.FindCollection(Layers, source);
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
        #endregion

        #region 图层选中回调
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
        #endregion

        public void AddLayer(string guid, VisualBrush? brush)
        {
            var picture = new LayerPicture
            {
                Guid = guid,
                LayerName = $"图层 {++layerIndex}",
                LayerType = LayerTypes.Picture,
                Thumbnail = brush,
                MarginLeft = 0,
            };
            picture.SelectedChanged += SelectedChanged;
            picture.IsVisibleChanged += IsVisibleChanged;
            Layers.Clear();
            Layers.Add(picture);
        }

        private void AddLayer(LayerBase? layerBase)
        {
            string guid = GuidUtil.GetGuid();
            var picture = new LayerPicture
            {
                Guid = guid,
                LayerName = $"图层 {++layerIndex}",
                LayerType = LayerTypes.Picture,
                Thumbnail = null,
                MarginLeft = 0,
            };
            picture.SelectedChanged += SelectedChanged;
            picture.IsVisibleChanged += IsVisibleChanged;

            string? previousGuid = null;
            if (layerBase == null)
            {
                Layers.Insert(0, picture);
            }
            else if (layerBase is LayerGroup layerGroup)
            {
                layerGroup.Children.Insert(0, picture);
                previousGuid = collectionUtil.FindPreviousGuid(Layers, guid);
            }
            else if (layerBase is LayerPicture layerPicture)
            {
                ObservableCollection<LayerBase>? collection = collectionUtil.FindCollection(Layers, layerPicture);
                if (collection == null)
                {
                    LogUtil.Log.Error(new Exception("collection为空"), "AddLayer未找到collection");
                    picture.SelectedChanged -= SelectedChanged;
                    picture.IsVisibleChanged -= IsVisibleChanged;
                    return;
                }
                collection.Insert(collection.IndexOf(layerPicture), picture);
                previousGuid = collectionUtil.FindPreviousGuid(Layers, guid);
            }
            layerDisplay?.LayerAdded(guid, previousGuid);
            //layerDisplay?.LayerIndexChanged();
        }

        public void SetLayerSize(string guid, int width, int height)
        {
            LayerPicture? picture = collectionUtil.FindLayerPicture(Layers, guid);
            if (picture != null)
            {
                picture.ThumbnailHeight = picture.ThumbnailWidth * height / width;
            }
        }

        public void SetLayerThumbnail(string guid, VisualBrush brush)
        {
            LayerPicture? picture = collectionUtil.FindLayerPicture(Layers, guid);
            if (picture != null)
            {
                picture.Thumbnail = brush;
            }
        }





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
                picture.IsVisibleChanged -= IsVisibleChanged;
            }
            DeleteLayer(SelectedLayer);
            SelectedLayer = null;
        }

        private void DeleteLayer(LayerBase layer)
        {
            ObservableCollection<LayerBase>? collection = collectionUtil.FindCollection(Layers, layer);
            collection?.Remove(layer);
        }








        private void IsVisibleChanged(LayerBase layerBase)
        {
            if (layerBase is LayerPicture picture)
            {
                layerDisplay?.SetLayerVisible(picture.Guid, picture.IsVisible ? Visibility.Visible : Visibility.Collapsed);
            }
        }


        private class CollectionUtil
        {
            // 找出LayerBase所在的Collection
            public ObservableCollection<LayerBase>? FindCollection(ObservableCollection<LayerBase> collection, LayerBase source)
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

            // 通过guid找出LayerPicture
            public LayerPicture? FindLayerPicture(ObservableCollection<LayerBase> collection, string guid)
            {
                for (int i = 0; i < collection.Count; ++i)
                {
                    if (collection[i] is LayerPicture picture && picture != null && picture.Guid == guid)
                    {
                        return picture;
                    }
                    if (collection[i] is LayerGroup group && group != null)
                    {
                        return FindLayerPicture(group.Children, guid);
                    }
                }
                return null;
            }

            // 树状结构转列表结构
            public void TreeToList(ObservableCollection<LayerBase> collection, ref List<string> list)
            {
                for (int i = 0; i < collection.Count; ++i)
                {
                    if (collection[i] is LayerPicture picture && picture != null)
                    {
                        list.Add(picture.Guid);
                    }
                    else if (collection[i] is LayerGroup group && group != null)
                    {
                        TreeToList(group.Children, ref list);
                    }
                }
            }

            public string? FindPreviousGuid(ObservableCollection<LayerBase> collection, string guid)
            {
                var list = new List<string>();
                TreeToList(collection, ref list);
                for(int i= 0; i < list.Count; ++i)
                {
                    if (list[i] == guid)
                    {
                        if (i + 1 >= list.Count)
                        {
                            return null;
                        }
                        return list[i + 1];
                    }
                }
                return null;
            }
        }
    }
}
