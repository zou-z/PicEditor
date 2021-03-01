using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PicEditor.window
{
    /// <summary>
    /// HelpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            test1.Text = "(1) BlackWhite：用于显示黑白两种色值的像素格式（非黑即白）。\r\n" +
                "(2) Gray2：2BPP（Bits Per Pixel，位 / 像素）的灰色通道。允许四种灰阶。\r\n" +
                "(3) Gray4：4BPP的灰度通道，允许16种灰阶值表示灰色。\r\n" +
                "(4) Gray8：显示8BPP的灰度通道，允许256种灰阶值表示灰色。\r\n" +
                "(5) Gray16：16BPP的灰色通道，最多允许65536种灰阶值表示灰色。这种格式的Gamma是1.0。\r\n" +
                "(6) Gray32Float：32BPP的灰度通道，允许超过40亿灰阶。此格式的Gamma值是1.0。\r\n" +
                "(7) Indexed1：指定2种颜色作为调色板的像素格式。\r\n" +
                "(8) Indexed2：指定4种颜色作为调色板的像素格式。\r\n" +
                "(9) Indexed4：指定16种颜色作为调色板的像素格式。\r\n" +
                "(10) Indexed8：指定256种颜色作为调色板的像素格式。\r\n" +
                "(11) Bgr24：Bgr24像素格式是一种采用24BPP的sRGB格式。每个颜色通道（蓝色blue, 绿色green, 红色red)各占8BPP（位 / 像素）。\r\n" +
                "(12) Bgra32：Bgra32像素格式是一种32BPP的sRGB格式。每个颜色通道（蓝色blue, 绿色green, 红色red)各占8BPP（位 / 像素），与Bgr24不同的是，它还有用于表现不透明度的alpha通道（8BPP）。\r\n" +
                "(13) Bgr101010：Bgr101010像素格式是一种采用32BPP（位 / 像素）的sRGB格式。每个颜色通道（蓝色blue, 绿色green, 红色red)各占10BPP（位 / 像素）。\r\n" +
                "(14) Bgr32：Bgr32像素格式是一种采用32BPP（位 / 像素）的sRGB格式。与Bgr101010格式不同的是，它的每个颜色通道（蓝色blue, 绿色green, 红色red)各占8BPP（位 / 像素）。\r\n" +
                "(15) Bgr555：Bgr555也是一种sRGB格式，它采用16BPP（位 / 像素）. 它的每个颜色通道（蓝色blue, 绿色green, 红色red)各占5BPP（位 / 像素）。\r\n" +
                "(16) Bgr565：Bgr565像素格式是一种16BPP（位 / 像素）的sRGB格式。它的每个颜色通道（蓝色blue, 绿色green, 红色red)分别占5BPP，6BPP，5BPP（位 / 像素）。\r\n" +
                "(17) Pbgra32：采用32BPP的一种基于sRGB的像素格式。每个颜色通道(蓝色blue, 绿色green, 红色red，Alpha通道)各占8BPP（位 / 像素）。每种颜色通道是经过与Alpha值预处理之后的。\r\n" +
                "(18) Prgba64：是一种基于sRGB格式，采用64BPP。每个颜色通道(蓝色blue, 绿色green, 红色red，Alpha通道)各占32BPP（位 / 像素）。每种颜色通道是经过与Alpha值预处理之后的。这种格式的Gamma是1.0。\r\n" +
                "(19) Rgb24：是一种基于sRGB格式，采用24BPP。每个颜色通道(蓝色blue, 绿色green, 红色red)各占8BPP（位 / 像素）。\r\n" +
                "(20) Rgb48：是一种基于sRGB格式，采用48BPP。每个颜色通道(蓝色blue, 绿色green, 红色red)各占16BPP（位 / 像素）。这种格式的Gamma是1.0。\r\n" +
                "(21) Rgba64：是一种基于sRGB格式，采用64BPP。每个颜色通道(蓝色blue, 绿色green, 红色red，Alpha通道)各占16BPP（位 / 像素）。这种格式的Gamma是1.0。\r\n" +
                "(22) Rgb128Float：是一种基于ScRGB格式，采用128BPP。每个颜色通道各占32BPP（位 / 像素）。这种格式的Gamma是1.0。\r\n" +
                "(23) Rgba128Float：是一种基于ScRGB格式，采用128BPP。每个颜色通道(蓝色blue, 绿色green, 红色red，Alpha通道)各占32BPP（位 / 像素）。这种格式的Gamma是1.0。\r\n" +
                "(24) Prgba128Float：是一种基于ScRGB格式，采用128BPP。每个颜色通道(蓝色blue, 绿色green, 红色red，Alpha通道)各占32BPP（位 / 像素）。每种颜色通道是经过与Alpha值预处理之后的。这种格式的Gamma是1.0。\r\n" +
                "(25) Cmyk32：用于表现印刷色格式，采用32BPP，共四个颜色通道即C、M、Y、K（青色Cyan, 品红Magenta, 黄色Yellow和黑色blacK)，各占8PP。";
        }
    }
}
