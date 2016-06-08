using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Nuage.VSDClient.Main
{
    /// <summary>
    /// App.xaml Logical
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            Login loginWindow = new Login(null);
            if (!loginWindow.IsLoginAutomatically())
            {
                loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                loginWindow.Show();
            }
        }
    }
}
