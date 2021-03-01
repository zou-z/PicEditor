using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace PicEditor.window
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        private readonly Setting setting;
        public SettingWindow(Setting setting)
        {
            InitializeComponent();
            this.setting = setting;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 19; i++)
            {
                comboBox.Items.Add(i + 2);
                if (i + 2 == setting.Precision)
                    comboBox.SelectedIndex = i;
            }
            HideSidePanel.IsChecked = setting.HideSidePanel;
            FitPicSize.IsChecked = setting.FitPicSize;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Confirm();
            }
            else if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DefaultSetting(object sender, RoutedEventArgs e)
        {
            comboBox.SelectedIndex = 14;
            HideSidePanel.IsChecked = false;
            FitPicSize.IsChecked = false;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            Confirm();
        }
        public void Confirm()
        {
            int precision = comboBox.SelectedIndex + 2;
            bool hideSidePanel = HideSidePanel.IsChecked == true ? true : false;
            bool fitPicSize = FitPicSize.IsChecked == true ? true : false;
            this.Close();
            if (precision == setting.Precision && hideSidePanel == setting.HideSidePanel && fitPicSize == setting.FitPicSize)
                return;
            setting.Precision = precision;
            setting.HideSidePanel = hideSidePanel;
            setting.FitPicSize = fitPicSize;
            setting.Alter();
        }
    }
    public class Setting
    {
        public int Precision;
        public bool HideSidePanel;
        public bool FitPicSize;
        private readonly string file_name;
        public Setting()
        {
            Precision = 16;
            HideSidePanel = FitPicSize = false;
            file_name = AppDomain.CurrentDomain.BaseDirectory + "setting.xml";
        }
        public void Get()
        {
            if (!File.Exists(file_name))
            {
                Create();
                return;
            }
            XmlDocument xml = new XmlDocument();
            xml.Load(file_name);
            XmlNode root = xml.SelectSingleNode("Setting");
            XmlNodeList nodes = root.ChildNodes;
            for (int i = 0; i < 3 && i < nodes.Count; i++)
            {
                if (nodes[i].Name == "Precision")
                {
                    try
                    {
                        Precision = int.Parse(nodes[i].InnerText);
                    }
                    catch (Exception)
                    {
                        Precision = 16;
                    }
                }
                else if (nodes[i].Name == "HideSidePanel")
                {
                    HideSidePanel = nodes[i].InnerText == "True" ? true : false;
                }
                else if (nodes[i].Name == "FitPicSize")
                {
                    FitPicSize = nodes[i].InnerText == "True" ? true : false;
                }
            }
        }
        public void Create()
        {
            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("Setting");
            xml.AppendChild(root);
            XmlElement node = xml.CreateElement("Precision");
            node.InnerText = Precision.ToString();
            root.AppendChild(node);
            node = xml.CreateElement("HideSidePanel");
            node.InnerText = HideSidePanel.ToString();
            root.AppendChild(node);
            node = xml.CreateElement("FitPicSize");
            node.InnerText = FitPicSize.ToString();
            root.AppendChild(node);
            xml.Save(file_name);
        }
        public void Alter()
        {
            if (!File.Exists(file_name))
            {
                Create();
                return;
            }
            XmlDocument xml = new XmlDocument();
            xml.Load(file_name);
            XmlNode root = xml.SelectSingleNode("Setting");
            XmlNodeList nodes = root.ChildNodes;
            for (int i = 0; i < 3 && i < nodes.Count; i++)
            {
                if (nodes[i].Name == "Precision")
                {
                    nodes[i].InnerText = Precision.ToString();
                }
                else if (nodes[i].Name == "HideSidePanel")
                {
                    nodes[i].InnerText = HideSidePanel.ToString();
                }
                else if (nodes[i].Name == "FitPicSize")
                {
                    nodes[i].InnerText = FitPicSize.ToString();
                }
            }
            xml.Save(file_name);
        }
    }
}
