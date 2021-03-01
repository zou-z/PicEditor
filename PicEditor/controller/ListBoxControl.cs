using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicEditor.controller
{
    class ListBoxControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<HistoryRecord> hr=new ObservableCollection<HistoryRecord>();
        private int index;
     
        public ObservableCollection<HistoryRecord> History
        {
            get { return hr; }
            set
            {
                hr = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("History"));
            }
        }
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Index"));
            }
        }

        public ListBoxControl()
        {
            Index = -1;
        }
        //需要控制数据集的大小
        public bool AddHistory(BitmapSource bs,string history_name,string name,string path)
        {
            if (bs == null)
                return false;
            for (int i = hr.Count - 1; i > index; i--)
                hr.Remove(hr[i]);
            hr.Add(new HistoryRecord(bs, history_name,name,path));
            Index++;
            return true;
        }
    }
    public class HistoryRecord
    {
        public BitmapSource HistoryPic { get; set; }
        public string HistoryName { get; set; }
        public string Name;
        public string Path;
        public HistoryRecord(BitmapSource HistoryPic, string HistoryName, string Name, string Path)
        {
            this.HistoryPic = HistoryPic;
            this.HistoryName = HistoryName;
            this.Name = Name;
            this.Path = Path;
        }
        /// <summary>
        /// 将三通道图片转化成四通道图片(在Pic控件上显示图片时使用)
        /// </summary>
        /// <returns>BitmapSource</returns>
        public BitmapSource Pic()
        {
            if (HistoryPic.Format == PixelFormats.Bgra32)
                return HistoryPic;
            FormatConvertedBitmap fcb = new FormatConvertedBitmap();
            fcb.BeginInit();
            fcb.Source = HistoryPic;
            fcb.DestinationFormat = PixelFormats.Bgra32;
            fcb.EndInit();
            return fcb;
        }
    }
}
