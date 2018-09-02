using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ParanoidDownloader.Helper;

namespace ParanoidDownloader
{
    /// <summary>
    /// UrlCreateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UrlCreateWindow : Window
    {
        public UrlCreateWindow()
        {
            InitializeComponent();
        }

        private void UrlCreateWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowBlurBackgroundHelper.BlurWindow(this);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) => Close();
    }
}
