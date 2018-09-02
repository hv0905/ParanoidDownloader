using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Serialization;
using ParanoidDownloader.Helper;

namespace ParanoidDownloader
{
    /// <summary>
    /// 使用Xml序列化存储应用相关设置等
    /// </summary>
    [Serializable]
    public class Setting
    {
        public static readonly string DefaultFileName = Path.Combine(System.Windows.Forms.Application.UserAppDataPath,
            ParanoidDownloader.Properties.Resources.settingFileName);


        public Size Hidden_MainWindowSize { get; set; } = new Size(0,0);
        public bool Hidden_AcceptEula { get; set; } = false;
        public DateTime Hidden_LastUpdateCheckTime { get; set; } = DateTime.Now;

        public int Download_DownloadMissionCount { get; set; } = 4;
        public int Download_DownloadMaxParaPerMission { get; set; } = 32;
        public int Download_DownloadMaxParaGlobal { get; set; } = 512;
        public int Download_DownloadBufferSize { get; set; } = 4;
        public bool Download_SmallFileList { get; set; } = true;
        public bool Download_SmallFileSingleThread { get; set; } = false;
        public string Download_DefaultLocation { get; set; } = KnownFolders.GetPath(KnownFolder.Downloads);
        public string Download_CachePath { get; set; }

        public bool Theme_UsingBackgroundFile { get; set; }
        public string Theme_BackgroundFile { get; set; }
        public bool Theme_OpacityBackground { get; set; } = true;
        public string Theme_MaterialPrimaryName { get; set; } = "blue";
        public string Theme_MaterialAccentName { get; set; } = "deeppurple";

        public bool Notify_DownloadFailed { get; set; } = true;
        public bool Notify_DownloadCompleted { get; set; } = true;
        public bool Notify_DownloadAllCompleted { get; set; } = true;
        public bool Notify_AlertWhenExecFile { get; set; } = true;

        public bool Binding_Clipboard { get; set; } = true;


        public string Agent_Address { get; set; }
        public string Agent_Port { get; set; }
        public string Agent_Exception { get; set; }


        public UpdateCheckInterval Other_UpdateCheckInterval { get; set; } = UpdateCheckInterval.EveryWeek;


        public Setting() { }


        public static Setting Load(string fileName)
        {
            using (var set = File.Open(fileName, FileMode.Open, FileAccess.Read)) {
                try {
                    return new XmlSerializer(typeof(Setting)).Deserialize(set) as Setting;
                }
                catch { //文件异常
                    set.Close();
                    File.Delete(fileName);
                    MessageBox.Show("There is something wrong with the setting.xml.\n All settings were reseted to default.\nThis may cause by a bug, or some third-part program change the setting.xml", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return new Setting();
                }
            }
        }

        public static Setting LoadOrNew(string fileName)
        {
            if (File.Exists(fileName))
                return Load(fileName);
            return new Setting();
        }


        public void Save(string fileName)
        {
            using (var set = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite)) {
                var xs = new XmlSerializer(typeof(Setting));
                xs.Serialize(set, this);
            }
        }
    }


    public class UpdateCheckIntervalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (UpdateCheckInterval)value;
    }

    public class MaterialDesignThemeDemoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var key = (string)parameter == "0" ? "primaryColorPreview" : "accentColorPreview";
            return ((MaterialDesignColorDemo[])App.Current.Resources[key]).FirstOrDefault(x => x.Name == (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ((MaterialDesignColorDemo)value).Name;
    }


    

    public enum UpdateCheckInterval : int
    {
        Startup = 0,
        Everyday = 1,
        ThreeDays =2,
        EveryWeek =3,
        Every2Weeks = 4,
        EveryMonth =5,
        Never = 6,
    }


    public class MaterialDesignColorDemo
    {
        public string Name { get; set; }

        public Color ColorDemo { get; set; }
    }
}
