using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PicEditor.controller
{
    class PaintControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private System.Windows.Ink.DrawingAttributes da;
        private int size;
        public bool Delete = false;

        public System.Windows.Ink.DrawingAttributes DA
        {
            get { return da; }
            set
            {
                da = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DA"));
            }
        }
        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                DA.Width = DA.Height = size;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Size"));
            }
        }

        public PaintControl()
        {
            DA = new System.Windows.Ink.DrawingAttributes
            {
                Color = System.Windows.Media.Colors.Black
            };
            Size = 5;
        }
    }
}
