using PicEditor.Basic.Util;
using PicEditor.Interface;
using PicEditor.Model.Layer;
using PicEditor.View.Control;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.ViewModel
{
    internal class VmLayer : ILayers, ILayerSource
    {
        private ILayerList? layerList = null;
        private CanvasInfo? canvasInfo = null;
        private ObservableCollection<UIElement>? pictureLayers = null;
        private ObservableCollection<UIElement>? upperLayers = null;

        public CanvasInfo CanvasInfo => canvasInfo ??= new CanvasInfo();

        public ObservableCollection<UIElement> PictureLayers => pictureLayers ??= new ObservableCollection<UIElement>();

        public ObservableCollection<UIElement> UpperLayers => upperLayers ??= new ObservableCollection<UIElement>();

        public void Initialize(ILayerList layerList)
        {
            this.layerList = layerList;
        }

        public void AddLayer(WriteableBitmap bitmap)
        {
            if (PictureLayers.Count == 0)
            {
                CanvasInfo.CanvasSize = new Size(bitmap.PixelWidth, bitmap.PixelHeight);
            }
            var image = new ImageEx(GuidUtil.GetGuid(), bitmap);
            PictureLayers.Add(image);

            layerList?.AddLayer(image.GetID(), image.GetVisualBrush());
            //layerManage?.SetLayerSize(image.GetID(), (int)LayerInfo.CanvasSize.Width, (int)LayerInfo.CanvasSize.Height);

            image.SetBinding(ImageEx.CanvasSizeProperty, new Binding("CanvasSize") { Source = CanvasInfo, Mode = BindingMode.OneWay });
            image.SetBinding(ImageEx.ScaleProperty, new Binding("Scale") { Source = CanvasInfo, Mode = BindingMode.OneWay });


        }

        public Brush? AddLayer(string id, string? previousID)
        {
            var bitmap = BitmapUtil.GetTransparentBitmap((int)CanvasInfo.CanvasSize.Width, (int)CanvasInfo.CanvasSize.Height);
            var image = new ImageEx(id, bitmap);
            image.SetBinding(ImageEx.CanvasSizeProperty, new Binding("CanvasSize") { Source = CanvasInfo, Mode = BindingMode.OneWay });
            image.SetBinding(ImageEx.ScaleProperty, new Binding("Scale") { Source = CanvasInfo, Mode = BindingMode.OneWay });
            if (previousID == null)
            {
                PictureLayers.Add(image);
            }
            else
            {
                var item = PictureLayers.FirstOrDefault(t => ((ImageEx)t).GetID() == previousID);
                if (item == null)
                {
                    LogUtil.Log.Error(new Exception("添加图层失败"), $"未找到id为{previousID}的图层");
                    return null;
                }
                PictureLayers.Insert(PictureLayers.IndexOf(item) + 1, image);
            }
            return image.GetVisualBrush();
        }

        public Size GetCanvasSize()
        {
            return CanvasInfo.CanvasSize;
        }
    }
}
