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
using NetTools;
namespace Nuage.VSDClient.Main
{
    /// <summary>
    /// Subnet.xaml 的交互逻辑
    /// </summary>
    public partial class Subnet : Window
    {
        INuageClient rest_client;
        string zone_id;
        ListBox parent;
        public string SubnetName { set; get; }
        public string Cidr { set; get; }
        public string Gateway { set; get; }
        public Subnet(INuageClient client, string zone_id, ListBox parent)
        {
            this.rest_client = client;
            this.parent = parent;
            this.zone_id = zone_id;

            InitializeComponent();
            _nameBox.DataContext = this;
            _cidr.DataContext = this;
            _gateway.DataContext = this;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string gateway = this._gateway.Text.Replace(" ", "");
            string cidr = this._cidr.Text.Replace(" ", "");

            IPNetwork ipnetwork;
            if(!IPNetwork.TryParse(cidr, out ipnetwork))
            {
                MessageBox.Show("Invalid network ip address.", "Error");
                return;
            }

            IPAddress ipAddress;
            if (!IPAddress.TryParse(gateway, out ipAddress))
            {
                MessageBox.Show("Invalid gateway ip address.", "Error");
                return;
            }

            var ipRange = IPAddressRange.Parse(cidr);
            if (!ipRange.Contains(IPAddress.Parse(gateway)))
            {
                string error = string.Format("Network Gateway IP Address {0} is out of range.", gateway);
                MessageBox.Show(error);
                return;
            }
            string desc = null;

            if (!string.IsNullOrEmpty(this._description.Text))
            {
                desc = this._description.Text;
            }

            try
            {

                NuageSubnet subnet = rest_client.CreateSubnet(this.zone_id, _nameBox.Text, desc,
                                                 ipnetwork.Network.ToString(), ipnetwork.Netmask.ToString(), gateway);
                if (subnet != null)
                {
                    this.parent.Items.Add(subnet);
                    this.Close();
                }

            }
            catch (NuageException)
            {
                string error = string.Format("Network name({0}) conflict or overlaps with existing network.", this._nameBox.Text);
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

    }
}
