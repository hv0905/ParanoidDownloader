using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using MaterialDesignThemes.Wpf;
using _WinForm = System.Windows.Forms;

namespace ParanoidDownloader
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static new App Current => (App)Application.Current;

        public Setting mainSetting;


        protected override void OnStartup(StartupEventArgs e)
        {
            mainSetting = Setting.LoadOrNew(Setting.DefaultFileName);
            var ph = new PaletteHelper();
            if (mainSetting.Theme_MaterialPrimaryName != "blue") {
                ph.ReplacePrimaryColor(mainSetting.Theme_MaterialPrimaryName);
            }

            if (mainSetting.Theme_MaterialAccentName != "deeppurple") {
                ph.ReplaceAccentColor(mainSetting.Theme_MaterialAccentName);
            }

        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            new ExceptionWindowHandler(e.Exception).ShowDialog();
            e.Handled = true;
        }
    }
}
