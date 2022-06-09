﻿using PicEditor.Basic.Util;
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

        public void AddPictureSource(WriteableBitmap bitmap, bool isInit)
        {
            if (isInit || LayerInfo.CanvasSize.Width == 0 && LayerInfo.CanvasSize.Height == 0)
            {
                PictureLayers.Clear();
                LayerInfo.CanvasSize = new Size(bitmap.PixelWidth, bitmap.PixelHeight);
                LayerInfo.Scale = 1.0;
            }
            var item = new PictureLayer(bitmap)
            {
                Guid = GuidUtil.GetGuid(),
                ImageWidth = bitmap.PixelWidth,
                ImageHeight = bitmap.PixelHeight,
            };
            item.SetSize(LayerInfo.CanvasSize.Width * LayerInfo.Scale, LayerInfo.CanvasSize.Height * LayerInfo.Scale, LayerInfo.Scale);
            PictureLayers.Add(item);

            layerManage?.AddLayer(item.Guid, item.GetVisualBrush(), isInit);
            layerManage?.SetLayerSize(item.Guid, bitmap.PixelWidth, bitmap.PixelHeight);
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

        public void LayerAdded(string guid, string? previousGuid)
        {
            var bitmap = FileUtil.GetTransparentBitmap((int)LayerInfo.CanvasSize.Width, (int)LayerInfo.CanvasSize.Height);
            var item = new PictureLayer(bitmap)
            {
                Guid = guid,
                ImageWidth = bitmap.PixelWidth,
                ImageHeight = bitmap.PixelHeight,
            };
            item.SetSize(LayerInfo.CanvasSize.Width * LayerInfo.Scale, LayerInfo.CanvasSize.Height * LayerInfo.Scale, LayerInfo.Scale);
            if (previousGuid == null)
            {
                PictureLayers.Add(item);
            }
            else
            {
                for (int i = 0; i < PictureLayers.Count; ++i)
                {
                    if (PictureLayers[i] is PictureLayer layer && layer != null && layer.Guid == previousGuid)
                    {
                        PictureLayers.Insert(i + 1, item);
                        break;
                    }
                }
            }
            layerManage?.SetLayerThumbnail(guid, item.GetVisualBrush());
            layerManage?.SetLayerSize(guid, bitmap.PixelWidth, bitmap.PixelHeight);
        }

        public void LayerDeleted(string guid)
        {
            for (int i = 0; i < PictureLayers.Count; ++i)
            {
                if (PictureLayers[i] is PictureLayer layer && layer != null && layer.Guid == guid)
                {
                    PictureLayers.RemoveAt(i);
                    return;
                }
            }
            LogUtil.Log.Error(new Exception("删除图层失败"), "未找到该图层");
        }

        public void LayersChanged(List<string> layerList)
        {
            layerList.Reverse();
            for (int i = 0; i < layerList.Count; ++i)
            {
                if (PictureLayers[i] is PictureLayer layer && layer != null && layer.Guid != layerList[i])
                {
                    for (int j = i + 1; j < layerList.Count; ++j)
                    {
                        if (PictureLayers[j] is PictureLayer tempLayer && tempLayer != null && tempLayer.Guid == layerList[i])
                        {
                            PictureLayers.Remove(tempLayer);
                            PictureLayers.Insert(i, tempLayer);
                            PictureLayers.Remove(layer);
                            PictureLayers.Insert(j, layer);
                            break;
                        }
                    }
                }
            }
        }
    }
}
