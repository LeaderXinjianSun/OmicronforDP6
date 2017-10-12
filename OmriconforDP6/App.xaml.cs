using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using OmicronforDP6.View;

namespace OmriconforDP6
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        MainWindow ViewMainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            ViewMainWindow = new OmicronforDP6.View.MainWindow();
            base.OnStartup(e);
            ViewMainWindow.Show();
        }
    }
}
