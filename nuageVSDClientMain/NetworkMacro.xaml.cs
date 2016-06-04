using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nuage.VSDClient.Main
{
    /// <summary>
    /// NetworkMacro.xaml 的交互逻辑
    /// </summary>
    public partial class NetworkMacro : Window
    {
        INuageClient rest_client;
        string ent_id;
        ListBox parent;
        public string MacroName { set; get; }
        public string Cidr { set; get; }

        public NetworkMacro(INuageClient client, string ent_id, ListBox parent)
        {
            this.rest_client = client;
            this.parent = parent;
            this.ent_id = ent_id;

            InitializeComponent();
            _nameBox.DataContext = this;
            _cidr.DataContext = this;
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string cidr = this._cidr.Text.Replace(" ", "");

            IPNetwork ipnetwork;
            if (!IPNetwork.TryParse(cidr, out ipnetwork))
            {
                MessageBox.Show("Invalid network ip address.", "Error");
                return;
            }

            try
            {

                NuageEnterpriseNetworks network_macro = rest_client.CreateNetworkMacro(this.ent_id, _nameBox.Text, 
                                                                                       ipnetwork.Network.ToString(), ipnetwork.Netmask.ToString());
                if (network_macro != null)
                {
                    this.parent.Items.Add(network_macro);
                    this.Close();
                }

            }
            catch (NuageException)
            {
                string error = string.Format("Network name conflict or Network overlaps with existing network.");
                MessageBox.Show(error);
                return;
            }
        }
        private void _name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Windows.Controls.Validation.GetHasError(_nameBox) == true
                || System.Windows.Controls.Validation.GetHasError(_cidr) == true)
                _Create.IsEnabled = false;
            else
                _Create.IsEnabled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _Create.IsEnabled)
                Create_Click(sender, e);
        }
    }
}
