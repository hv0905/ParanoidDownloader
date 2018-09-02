using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using ParanoidDownloader.Annotations;

namespace ParanoidDownloader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<MissionListDisplayInfo> _missionInfos = new List<MissionListDisplayInfo>();


        public MainWindow()
        {
            InitializeComponent();
            _missionInfos.Clear();
            _missionInfos.Add(new MissionListDisplayInfo() {
                FileName = "Saber",
                ProgressPercent = 50d,
                FileSize = 1024*1024*1024,
            });

            _missionInfos.Add(new MissionListDisplayInfo() {
                FileName = "Archer",
                ProgressPercent = 36d,
                FileSize = 1024*1024*1024,
            });

            missionList.ItemsSource = _missionInfos;
        }

        private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaxButton_OnClick(object sender, RoutedEventArgs e) =>
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;

        private void MinButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void DragTitle(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void NewBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new UrlCreateWindow {Owner = this};
            win.ShowDialog();
        }

        private void CancelNewBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //buildinDialog.IsOpen = false;
        }

        public class MissionListDisplayInfo : INotifyPropertyChanged, IComparable<MissionListDisplayInfo>
        {
            private string _fileName;
            private string _url;
            private long _fileSize;
            private double _progressPercent;
            private TimeSpan _timeLeft;

            public string FileName
            {
                get => _fileName;
                set {
                    if (value == _fileName) return;
                    _fileName = value;
                    OnPropertyChanged();
                }
            }

            public string Url
            {
                get => _url;
                set {
                    if (value == _url) return;
                    _url = value;
                    OnPropertyChanged();
                }
            }

            public long FileSize
            {
                get => _fileSize;
                set {
                    if (value == _fileSize) return;
                    _fileSize = value;
                    OnPropertyChanged();
                }
            }

            public double ProgressPercent
            {
                get => _progressPercent;
                set {
                    if (value.Equals(_progressPercent)) return;
                    _progressPercent = value;
                    OnPropertyChanged();
                }
            }

            public TimeSpan TimeLeft
            {
                get => _timeLeft;
                set {
                    if (value.Equals(_timeLeft)) return;
                    _timeLeft = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                Debug.WriteLine("A property changed");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public int CompareTo(MissionListDisplayInfo other) =>
                string.Compare(FileName, other.FileName, StringComparison.CurrentCulture);
        }

        private void MissionList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (missionList.SelectedIndex == -1) return;
            infoTrans.SelectedIndex = 1;
            toolbarTrans.SelectedIndex = 1;
        }

        private void StartBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _missionInfos[0].ProgressPercent = 100;
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            new SettingWindow().Show();
        }

        private void PauseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var ph = new PaletteHelper();
            ph.ReplacePrimaryColor("blue");
        }

        private void MenuInfo_OnClick(object sender, RoutedEventArgs e)
        {
            infoTrans.SelectedIndex = 1;
            toolbarTrans.SelectedIndex = 1;
        }

        private void DetailBack_OnClick(object sender, RoutedEventArgs e)
        {
            toolbarTrans.SelectedIndex = 0;
            infoTrans.SelectedIndex = 0;
        }
    }


    #region Converters
    public class FileSizeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long byteCount = (long)value;

            if (byteCount < 1024) {
                //B
                return $"{byteCount} B";
            }
            else if (byteCount < 1024 * 1024) {
                //KB
                return $"{Math.Round(byteCount / 1024d, 2)} KB";
            }
            else if (byteCount < 1024 * 1024 * 1024) {
                //MB
                return $"{Math.Round(byteCount / 1024d / 1024, 2)} MB";
            }
            else {
                //GB
                return $"{Math.Round(byteCount / 1024d / 1024 / 1024, 2)} GB";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double)) throw new ArgumentException(nameof(value));
            return $"{Math.Round((double)value, 2)}%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new ArgumentException();
    }


    public class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var arg = parameter as string;
            var speed = (long)value;
            switch (arg) {
                case "Bs":
                    if (speed < 1024) {
                        return $"{speed}B/S";
                    }
                    else if (speed < 1024 * 1024) {
                        return $"{Math.Round(speed / 1024d, 2)}KB/S";
                    }
                    else {
                        return $"{Math.Round(speed / 1024d / 1024d, 2)}MB/S";
                    }
                case "bps":
                    if (speed < 1024 / 8) {
                        return $"{speed * 8}bps";
                    }
                    else if (speed < 1024 * 1024 / 8) {
                        return $"{Math.Round(speed / 1024d * 8, 2)}kbps";
                    }
                    else {
                        return $"{Math.Round(speed / 1024d / 1024d * 8, 2)}Mbps";
                    }
                default:
                    throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new ArgumentException();
    } 
    #endregion
}
