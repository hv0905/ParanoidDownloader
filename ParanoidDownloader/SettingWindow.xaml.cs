using System;
using System.Collections.Generic;
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
using MaterialDesignThemes.Wpf;
using Ookii.Dialogs.Wpf;
using ParanoidDownloader.Helper;
using Path = System.IO.Path;

namespace ParanoidDownloader
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            DataContext = App.Current.mainSetting; //绑定设置上下文
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void SettingWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowBlurBackgroundHelper.BlurWindow(this);
        }

        private void CloseBtn_OnClick(object sender, RoutedEventArgs e) => Close();

        void Save()
        {
            App.Current.mainSetting.Save(Setting.DefaultFileName);
            //Apply theme update
            var ph = new PaletteHelper();
            ph.ReplacePrimaryColor(App.Current.mainSetting.Theme_MaterialPrimaryName);
            ph.ReplaceAccentColor(App.Current.mainSetting.Theme_MaterialAccentName);
        }

        private void SubmitBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Save();
            Close();
        }

        private void ResetBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("这将重置所有设置,继续?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            App.Current.mainSetting = new Setting();
            Save();
            Close();
        }

        private void ListBoxItem_OnSelected(object sender, RoutedEventArgs e)
        {
            download_SavePath_Input.Text = KnownFolders.GetPath(KnownFolder.Downloads);
        }

        private void ListBoxItem_OnSelected_1(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog() {
                Description = "选择默认下载文件夹",
                RootFolder = Environment.SpecialFolder.Desktop,
                ShowNewFolderButton = true,
            };
            if (dialog.ShowDialog() ?? false) {
                download_SavePath_Input.Text = dialog.SelectedPath;
            }
        }

        private void ListBoxItem_OnSelected_2(object sender, RoutedEventArgs e)
        {
            download_CachePath_Input.Text = string.Empty;
        }

        private void ListBoxItem_OnSelected_3(object sender, RoutedEventArgs e)
        {
            download_CachePath_Input.Text = Path.Combine(Path.GetTempPath(), "ParanoidDownloader");
        }

        private void ListBoxItem_OnSelected_4(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog() {
                Description = "选择下载缓存文件夹",
                RootFolder = Environment.SpecialFolder.Desktop,
                ShowNewFolderButton = true,
            };
            if (dialog.ShowDialog() ?? false) {
                download_CachePath_Input.Text = dialog.SelectedPath;
            }
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("如果你关闭最近删除功能,所有最近删除中的项目均会被永久删除.","提示",MessageBoxButton.OK,MessageBoxImage.Warning);
        }
    }
}
