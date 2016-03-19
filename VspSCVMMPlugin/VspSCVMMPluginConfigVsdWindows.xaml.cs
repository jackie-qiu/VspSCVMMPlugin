using System;
using System.IO;
using System.Reflection;
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
using log4net;
using log4net.Config;
using Nuage.VSDClient;

namespace Microsoft.VirtualManager.UI.AddIns.NuageVSP
{
    /// <summary>
    /// VspSCVMMPluginConfigVsdWindows.xaml 的交互逻辑
    /// </summary>
    public partial class VspSCVMMPluginConfigVsdWindows : Window
    {
        private string addinPath = "";
        private static readonly ILog logger = LogManager.GetLogger(typeof(VspSCVMMPluginConfigVsdWindows));

        public VspSCVMMPluginConfigVsdWindows(string baseUrl, string username, string organization)
        {
            InitializeComponent();

            this.vsdUrl.Text = baseUrl;
            this.vsdUserName.Text = username;
            this.vsdOrganization.Text = organization;

        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.vsdUrl.Text) || string.IsNullOrEmpty(this.vsdUserName.Text)
                || string.IsNullOrEmpty(this.vsdPassword.Password) || string.IsNullOrEmpty(this.vsdOrganization.Text))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            if(!(Uri.IsWellFormedUriString(this.vsdUrl.Text, UriKind.Absolute)))
            {
                MessageBox.Show("Invalid Url address.");
                return;
            }

            NuageVSDPowerShellSession nuSession = new NuageVSDPowerShellSession(this.vsdUserName.Text, this.vsdPassword.Password,
                                                               this.vsdOrganization.Text, new Uri(this.vsdUrl.Text), this.vsdVersion.Text);

            if (!nuSession.LoginVSD())
            {
                string result = string.Format("Connect to vsd {0} failed.", this.vsdUrl.Text);
                MessageBox.Show(result);
                return;
            }

            addinPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                string version = this.vsdVersion.Text;
                File.WriteAllText(addinPath + "\\vsd.conf", string.Empty);
                File.AppendAllText(addinPath + "\\vsd.conf", 
                                       "Url=" + this.vsdUrl.Text + Environment.NewLine
                                        + "Username=" + this.vsdUserName.Text + Environment.NewLine
                                        + "Password=" + this.vsdPassword.Password + Environment.NewLine
                                        + "Organization=" + this.vsdOrganization.Text + Environment.NewLine
                                        + "Version=" + version.Replace(".","_") + Environment.NewLine);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Write to VSD config file failed {0}.", ex.Message);
                MessageBox.Show("Write to VSD config file failed.");
            }
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
