using System;
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
using System.Windows.Controls.Primitives;

namespace Nuage.VSDClient.Main
{
    /// <summary>
    /// ACLRule.xaml Logical
    /// </summary>
    public partial class ACLRule : Window
    {
        INuageClient rest_client;
        string acl_id;
        string ent_id;
        string domain_id;
        ListBox parent;
        string direction;
        List<string> NETWORK_TYPE = new List<string>() { "Any", "Endpoint's Domain", "Endpoint's Zone", "Endpoint's Subnet", "Zone", "Subnet", "Network Macro", "Network Macro Group", "Policy Group" };
        List<string> LOCATION_TYPE = new List<string>() { "Any","Zone","Subnet","Policy Group" };
        List<string> ETHER_TYPE = new List<string>() { "IPv4 - 0x0800" };
        List<string> ACTION = new List<string>() { "Allow", "Drop" };
        List<string> PROTOCOL_TYPE = new List<string>() { "ANY", "TCP - 6", "UDP - 17", "ICMP - 1" };
        private Dictionary<string, string> PROTO_NAME_TO_NUM = new Dictionary<string, string>();
        private Dictionary<string, string> NETWORK_TYPE_MAP = new Dictionary<string, string>();
        public string addrOverride { set; get; }
        public ACLRule(string direction, INuageClient client, string ent_id, string domain_id, string acl_id, ListBox parent)
        {
            PROTO_NAME_TO_NUM.Add("ANY", "ANY");
            PROTO_NAME_TO_NUM.Add("TCP - 6", "6");
            PROTO_NAME_TO_NUM.Add("UDP - 17", "17");
            PROTO_NAME_TO_NUM.Add("ICMP - 1", "1");

            NETWORK_TYPE_MAP.Add("Any", "ANY");
            NETWORK_TYPE_MAP.Add("Endpoint's Domain", "ENDPOINT_DOMAIN");
            NETWORK_TYPE_MAP.Add("Endpoint's Zone", "ENDPOINT_ZONE");
            NETWORK_TYPE_MAP.Add("Endpoint's Subnet", "ENDPOINT_SUBNET");
            NETWORK_TYPE_MAP.Add("Zone", "ZONE");
            NETWORK_TYPE_MAP.Add("Subnet", "SUBNET");
            NETWORK_TYPE_MAP.Add("Network Macro", "ENTERPRISE_NETWORK");
            NETWORK_TYPE_MAP.Add("Network Macro Group", "NETWORK_MACRO_GROUP");
            NETWORK_TYPE_MAP.Add("Policy Group", "POLICYGROUP");

            this.direction = direction;
            this.rest_client = client;
            this.parent = parent;
            this.acl_id = acl_id;
            this.ent_id = ent_id;
            this.domain_id = domain_id;

            InitializeComponent();
            _addressOverride.DataContext = this;
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            _etherType.ItemsSource = ETHER_TYPE;
            _etherType.SelectedIndex = 0;
            _action.ItemsSource = ACTION;
            _action.SelectedIndex = 0;
            _protocol.ItemsSource = PROTOCOL_TYPE;
            _protocol.SelectedIndex = 1;
            if (direction.Equals("ingress"))
            {
                _ACLRuleWin.Title = "Ingress" + _ACLRuleWin.Title;
                _OriginType.ItemsSource = LOCATION_TYPE;
                _DestinationType.ItemsSource = NETWORK_TYPE;
            }
            else
            {
                _ACLRuleWin.Title = "Egress" + _ACLRuleWin.Title;
                _IpMatch.Text = "Dest IP Match";
                _DestinationType.ItemsSource = LOCATION_TYPE;
                _OriginType.ItemsSource = NETWORK_TYPE;
            }
            _DestinationType.SelectedIndex = 0;
            _OriginType.SelectedIndex = 0;
            
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string desc = null;
            string addressOverride = null;
            string ether_type = null;
            string action = "DROP";
            string priority = null;
            string protocol = "ANY";
            string src_port ="*";
            string dest_port ="*";
            string network_type = "ANY";
            string network_id = null;
            string location_type = "ANY";
            string location_id = null;

            if (!string.IsNullOrEmpty(this._description.Text))
            {
                desc = this._description.Text;
            }

            if (!string.IsNullOrEmpty(this._addressOverride.Text))
            {
                addressOverride = this._addressOverride.Text;
            }

            if (!string.IsNullOrEmpty(this._priority.Text))
            {
                priority = this._priority.Text.Replace(" ", "");
            }
            

            if(this._action.SelectedItem.Equals("Allow"))
            {
                action = "FORWARD";
            }
            
            protocol = PROTO_NAME_TO_NUM[(string)this._protocol.SelectedItem];
            if (protocol.Equals("6") || protocol.Equals("17"))
            {
                src_port = this._sourcePort.Text.Replace(" ", "");
                if(String.IsNullOrEmpty(src_port))
                {
                    src_port = "*";
                }

                dest_port = this._dstPort.Text.Replace(" ", "");
                if(String.IsNullOrEmpty(dest_port))
                {
                    dest_port = "*";
                }
            }

            network_type = NETWORK_TYPE_MAP[(string)this._DestinationType.SelectedItem];

            if(!network_type.Equals("ANY") && !network_type.Equals("ENDPOINT_DOMAIN") 
                && !network_type.Equals("ENDPOINT_ZONE") && !network_type.Equals("ENDPOINT_SUBNET"))
            {
                
                if(this._selectedDestination.HasItems)
                {
                    this._selectedDestination.SelectedIndex = 0;
                    NuageBase nuageObject = (NuageBase)this._selectedDestination.SelectedItem;
                    network_id = nuageObject.ID;
                }

            }

            location_type = NETWORK_TYPE_MAP[(string)this._OriginType.SelectedItem];

            if(!location_type.Equals("ANY") && !location_type.Equals("ENDPOINT_DOMAIN") 
                && !location_type.Equals("ENDPOINT_ZONE") && !location_type.Equals("ENDPOINT_SUBNET"))
            {
                
                if(this._selectedOrigin.HasItems)
                {
                    this._selectedOrigin.SelectedIndex = 0;
                    NuageBase nuageObject = (NuageBase)this._selectedOrigin.SelectedItem;
                    location_id = nuageObject.ID;
                }

            }

            ether_type = (string)this._etherType.SelectedItem;
            try
            {
                NuageACLRule acl;

                if (direction.Equals("ingress"))
                {
                    acl = rest_client.CreateACLRule(acl_id, direction, desc, addressOverride, action, priority, ether_type,
                                              protocol, src_port, dest_port, "", location_type, location_id,
                                              network_type, network_id);
                }
                else
                {
                    //swap the network and location when direction is egress
                    acl = rest_client.CreateACLRule(acl_id, direction, desc, addressOverride, action, priority, ether_type,
                                             protocol, src_port, dest_port, "", network_type, network_id,
                                             location_type, location_id);
                }
                if (acl != null)
                {
                    parent.Items.Add(acl);
                    parent.SelectedIndex = parent.Items.Count - 1;
                    this.Close();
                }
               
            }
            catch (NuageException)
            {
                string error = String.Format("Create ACL rule in {0} failed", acl_id);
                MessageBox.Show(error, "Error");
            }           
                
        }

        private void _AddOrigin(object sender, RoutedEventArgs e)
        {
            if (_OriginType.SelectedIndex == -1)
                return;
            string origin_type = (string)_OriginType.SelectedItem;

            _selectedOrigin.Items.Clear();
            try
            {
                if (origin_type.Equals("Zone"))
                {
                    List<NuageZone> zones = rest_client.GetZonesInDomain(this.domain_id);
                    SelectWin selectwin = new SelectWin(zones, _selectedOrigin, origin_type);
                    selectwin.Owner = this;
                    selectwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectwin.ShowDialog();
                }

                if (origin_type.Equals("Subnet"))
                {
                    List<NuageSubnet> subnets = rest_client.GetSubnetsInDomain(this.domain_id);
                    SelectWin selectwin = new SelectWin(subnets, _selectedOrigin, origin_type);
                    selectwin.Owner = this;
                    selectwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectwin.ShowDialog();
                }

                if (origin_type.Equals("Policy Group"))
                {
                    List<NuagePolicyGroup> pg = rest_client.GetPolicyGroupsInDomain(this.domain_id);
                    SelectWin selectwin = new SelectWin(pg, _selectedOrigin, origin_type);
                    selectwin.Owner = this;
                    selectwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectwin.ShowDialog();
                }

                if (origin_type.Equals("Network Macro"))
                {
                    List<NuageEnterpriseNetworks> macros = rest_client.GetNetworkMacrosInEnterprise(this.ent_id);
                    SelectWin selectwin = new SelectWin(macros, _selectedOrigin, origin_type);
                    selectwin.Owner = this;
                    selectwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectwin.ShowDialog();
                }

                if (origin_type.Equals("Network Macro Group"))
                {
                    List<NuageNetworkMacroGroups> macro_groups = rest_client.GetNetworkMacroGroupsInEnterprise(this.ent_id);
                    SelectWin selectwin = new SelectWin(macro_groups, _selectedOrigin, origin_type);
                    selectwin.Owner = this;
                    selectwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectwin.ShowDialog();
                }

               
            }
            catch (NuageException)
            {
                string error = String.Format("Get {0} in domain {1} failed", origin_type, domain_id);
                MessageBox.Show(error, "Error");
            }

            
        }

        private void _AddDestination(object sender, RoutedEventArgs e)
        {
            if (_DestinationType.SelectedIndex == -1)
                return;
            string dst_type = (string)_DestinationType.SelectedItem;

            _selectedDestination.Items.Clear();
            try
            {
                if (dst_type.Equals("Zone"))
                {
                    List<NuageZone> zones = rest_client.GetZonesInDomain(this.domain_id);
                    SelectWin selectwin = new SelectWin(zones, _selectedDestination, dst_type);
                    selectwin.Owner = this;
                    selectwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectwin.ShowDialog();
                }

                if (dst_type.Equals("Subnet"))
                {
                    List<NuageSubnet> subnets = rest_client.GetSubnetsInDomain(this.domain_id);
                    SelectWin selectwin = new SelectWin(subnets, _selectedDestination, dst_type);
                    selectwin.Owner = this;
                    selectwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectwin.ShowDialog();
                }

                if (dst_type.Equals("Policy Group"))
                {
                    List<NuagePolicyGroup> pg = rest_client.GetPolicyGroupsInDomain(this.domain_id);
                    SelectWin selectwin = new SelectWin(pg, _selectedDestination, dst_type);
                    selectwin.Owner = this;
                    selectwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectwin.ShowDialog();
                }

                if (dst_type.Equals("Network Macro"))
                {
                    List<NuageEnterpriseNetworks> macros = rest_client.GetNetworkMacrosInEnterprise(this.ent_id);
                    SelectWin selectwin = new SelectWin(macros, _selectedDestination, dst_type);
                    selectwin.Owner = this;
                    selectwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectwin.ShowDialog();
                }

                if (dst_type.Equals("Network Macro Group"))
                {
                    List<NuageNetworkMacroGroups> macro_groups = rest_client.GetNetworkMacroGroupsInEnterprise(this.ent_id);
                    SelectWin selectwin = new SelectWin(macro_groups, _selectedDestination, dst_type);
                    selectwin.Owner = this;
                    selectwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    selectwin.ShowDialog();
                }
            }
            catch (NuageException)
            {
                string error = String.Format("Get {0} in domain {1} failed", dst_type, domain_id);
                MessageBox.Show(error, "Error");
            }

            
        }

        private void _protocol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_protocol.SelectedIndex == -1)
                return;
            string protocol = (string)_protocol.SelectedItem;
            if (protocol.Equals("ANY") || protocol.Equals("ICMP - 1"))
            {
                _sourcePort.Visibility = System.Windows.Visibility.Hidden;
                _dstPort.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                _sourcePort.Visibility = System.Windows.Visibility.Visible;
                _dstPort.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void _OriginType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_OriginType.SelectedIndex == -1)
                return;
            string origin_type = (string)_OriginType.SelectedItem;

            if (origin_type.Equals("Endpoint's Domain") || origin_type.Equals("Endpoint's Zone")
                || origin_type.Equals("Endpoint's Subnet") || origin_type.Equals("Any"))
            {
                _OriginTypeButton.IsEnabled = false;
            }
            else
            {
                _OriginTypeButton.IsEnabled = true;
            }

            _selectedOrigin.Items.Clear();
        }

        private void _DestinationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_DestinationType.SelectedIndex == -1)
                return;
            string dst_type = (string)_DestinationType.SelectedItem;

            if (dst_type.Equals("Endpoint's Domain") || dst_type.Equals("Endpoint's Zone")
                || dst_type.Equals("Endpoint's Subnet") || dst_type.Equals("Any"))
            {
                _DstTypeButton.IsEnabled = false;
            }
            else
            {
                _DstTypeButton.IsEnabled = true;
            }

            _selectedDestination.Items.Clear();
        }

        private void _action_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_action.SelectedIndex == -1)
                return;
            string action = (string)_action.SelectedItem;

            if (action.Equals("Allow"))
                _StatefulPannel.Visibility = System.Windows.Visibility.Visible;
            else
                _StatefulPannel.Visibility = System.Windows.Visibility.Hidden;

        }

        private void _MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ((Xceed.Wpf.Toolkit.MaskedTextBox)sender).Select(0, 0);
        }

        private void _ACLRuleWin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Create_Click(sender, e);
        }

        private void EnableLoggingTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _enableFLowLoggin.IsChecked = !(_enableFLowLoggin.IsChecked);
        }

        private void StatefulTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _Stateful.IsChecked = !(_Stateful.IsChecked);
        }

        private void _addressOverride_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this._addressOverride.Text))
            {
                _Create.IsEnabled = true;
                return;
            }

            if (System.Windows.Controls.Validation.GetHasError(_addressOverride) == true)
                _Create.IsEnabled = false;
            else
                _Create.IsEnabled = true;
        }
        

    }
}
