using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;

namespace PicEditor.controller
{
    class TextControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int left;
        private int top;

        public int Left
        {
            get { return left; }
            set
            {
                left = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Left"));
            }
        }
        public int Top
        {
            get { return top; }
            set
            {
                top = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Top"));
            }
        }
        public List<FontFamilyClass> AllFontFamily { get; } = new List<FontFamilyClass>();
        public List<FontSizeClass> AllFontSize
        {
            get
            {
                int[] fontSize = new int[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72, 94, 130 };
                List<FontSizeClass> list = new List<FontSizeClass>();
                for(int i = 0; i < fontSize.Length; i++)
                {
                    list.Add(new FontSizeClass(fontSize[i]));
                }
                return list;
            }
        }

        public TextControl()
        {
            List<FontFamilyClass> ZhFontFamily = new List<FontFamilyClass>();
            foreach (FontFamily fontfamily in Fonts.SystemFontFamilies)
            {
                LanguageSpecificStringDictionary fontdics = fontfamily.FamilyNames;
                string fontfamilyname;
                //添加英文字体
                if (fontdics.ContainsKey(XmlLanguage.GetLanguage("en-us")))
                {
                    if (fontdics.TryGetValue(XmlLanguage.GetLanguage("en-us"), out fontfamilyname))
                    {
                        AllFontFamily.Add(new FontFamilyClass(fontfamilyname));
                    }
                }
                //添加中文字体
                if (fontdics.ContainsKey(XmlLanguage.GetLanguage("zh-cn")))
                {
                    if (fontdics.TryGetValue(XmlLanguage.GetLanguage("zh-cn"), out fontfamilyname))
                    {
                        ZhFontFamily.Add(new FontFamilyClass(fontfamilyname));
                    }
                }
            }
            AllFontFamily.AddRange(ZhFontFamily);
        }
    }
    public class FontFamilyClass
    {
        public string FontFamily { get; set; }
        public FontFamilyClass(string FontFamily)
        {
            this.FontFamily = FontFamily;
        }
    }
    public class FontSizeClass
    {
        public int FontSize { get; set; }
        public FontSizeClass(int FontSize)
        {
            this.FontSize = FontSize;
        }
    }
}
