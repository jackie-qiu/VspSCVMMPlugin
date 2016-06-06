using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Nuage.VSDClient;
using Microsoft.SystemCenter.VirtualMachineManager;
using Microsoft.SystemCenter.VirtualMachineManager.Cmdlets;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.PowerShell;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.ContextTypes;
using Microsoft.SystemCenter.VirtualMachineManager.Remoting;

namespace Nuage.VSDClient.Main
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public string _url { set; get; }
        public string _user_name { set; get; }
        public string _org { set; get; }
        public string _version { set; get; }
        private PowerShellContext psConext;


        public Login(PowerShellContext psConext)
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(OnLoaded);
            vsdUrl.DataContext = this;
            vsdUserName.DataContext = this;
            vsdOrganization.DataContext = this;
            vsdVersion.DataContext = this;

            this.psConext = psConext;
        }

        public bool IsLoginAutomatically()
        {
            Dictionary<string, string> config;

            if ((config = readVSDConfig()) != null)
            {
                if (config.Count > 0)
                {
                    string baseUrl, username, organization, password, version, auto_login;
                    config.TryGetValue("Url", out baseUrl);
                    config.TryGetValue("Username", out username);
                    config.TryGetValue("Password", out password);
                    config.TryGetValue("Organization", out organization);
                    config.TryGetValue("Version", out version);
                    config.TryGetValue("Auto", out auto_login);

                    if (auto_login != null && auto_login.Equals("True"))
                    {
                        INuageClient rest_client = login(baseUrl, username, password, organization, version);
                        if (rest_client != null)
                        {
                            MainWindow mainWindow = new MainWindow(rest_client, this.psConext);
                            mainWindow.Show();
                            return true;
                        }
                    }

                    this.vsdUrl.Text = baseUrl;
                    this.vsdUserName.Text = username;
                    this.vsdPassword.Password = password;
                    this.vsdOrganization.Text = organization;
                    this.vsdVersion.Text = version;

                }
            }

            return false;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {

        }


        private Dictionary<string, string> readVSDConfig()
        {
            var config = new Dictionary<string, string>();

            try
            {
                string config_path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                foreach (var row in File.ReadLines(System.IO.Path.Combine(config_path + "\\vsd.conf")))
                {
                    config.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
                }
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

            return config;
        }
        private INuageClient login(string baseUrl, string username, string password, string organization, string version)
        {
            INuageClient rest_client;
            try
            {
                rest_client = new NuageClient(username, password, organization, new Uri(baseUrl), version);
                if (!rest_client.LoginVSD())
                    return null;

            }
            catch (NuageException ex)
            {
                return null;
            }

            return rest_client;
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            INuageClient rest_client = login(this.vsdUrl.Text, this.vsdUserName.Text, this.vsdPassword.Password,
                        this.vsdOrganization.Text, this.vsdVersion.Text);
            if (rest_client == null)
            {
                MessageBox.Show("Login VSD failed, please check the username and password.", "Failed");
                return;
            }
 
            string config_path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                string version = this.vsdVersion.Text;
                File.WriteAllText(config_path + "\\vsd.conf", string.Empty);
                File.AppendAllText(config_path + "\\vsd.conf", 
                                       "Url=" + this.vsdUrl.Text + Environment.NewLine
                                        + "Username=" + this.vsdUserName.Text + Environment.NewLine
                                        + "Password=" + this.vsdPassword.Password + Environment.NewLine
                                        + "Organization=" + this.vsdOrganization.Text + Environment.NewLine
                                        + "Version=" + version.Replace(".","_") + Environment.NewLine
                                        + "Auto=" + this._isAutoLogin.IsChecked + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Write to VSD config file failed {0}.",ex.Message));
                return;
            }

            MainWindow mainWindow = new MainWindow(rest_client, this.psConext);
            mainWindow.Show();
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void vsdUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Windows.Controls.Validation.GetHasError(vsdUrl) == true
                || System.Windows.Controls.Validation.GetHasError(vsdUserName) == true
                || System.Windows.Controls.Validation.GetHasError(vsdOrganization) == true
                || System.Windows.Controls.Validation.GetHasError(vsdVersion) == true)
                okButton.IsEnabled = false;
            else
                okButton.IsEnabled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && okButton.IsEnabled)
                connectButton_Click(sender, e);
        }

        private void Run_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._isAutoLogin.IsChecked = !(this._isAutoLogin.IsChecked);
        }
    }
}
