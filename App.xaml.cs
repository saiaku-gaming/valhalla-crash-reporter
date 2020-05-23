using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ValhallaCrashReporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            bool foundCrashGuid = false;
            foreach (var arg in e.Args)
            {
                if (arg.Contains("-CrashGUID"))
                {
                    foundCrashGuid = true;
                }

                if (arg.Contains("-Unattended"))
                {
                    Shutdown();
                }
            }
            if (!foundCrashGuid)
            {
                Shutdown();
            }

            MainWindow MainWindow = new MainWindow(e.Args);
            MainWindow.Show();
        }

    }
}
