using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Nuage.VSDClient;

namespace Nuage.VSDClient.Main
{
    /// <summary>
    /// MainWindow.xaml Logical
    /// </summary>
    public partial class MainWindow : Window
    {
        INuageClient rest_client;
        List<NuageEnterprise> enterprises;
        LayoutDocumentPane _zone_panel;
        LayoutDocument _layoutZones;
        LayoutDocumentPane _subnet_panel;
        LayoutDocument _LayoutSubnet;
        LayoutDocumentPane _policy_panel;
        LayoutDocument _layoutPolicyGroup;
        LayoutDocument _layoutIngressPolicy;
        LayoutDocument _layoutEgressPolicy;
        ListBox _Zones;
        ListBox _Subnets;
        ListBox _PolicyGroup;
        ListBox _IngressPolicy;
        ListBox _EgressPolicy;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {

            try
            {
                rest_client = new NuageClient("csproot", "csproot", "csp", new Uri("https://192.168.239.12:8443/"), "3.2");
                if (!rest_client.LoginVSD())
                    MessageBox.Show("Connect to VSD failed, please check the configuration.", "Failed");

            }
            catch(NuageException ex)
            {
                string error = string.Format("Connect to VSD failed with error {0}, please check the configuration.", ex.Message);
                MessageBox.Show(error, "Failed");
            }

            InitWindowsElement();
            return;



        }

        private void InitZonePanel()
        {
            _zone_panel = new LayoutDocumentPane { DockWidth = new GridLength(50) };
            _layoutZones = new LayoutDocument { ContentId = "zone", Title = "Zone", CanClose = false, CanFloat = false, IsSelected = true };
            _layoutZones.IsSelectedChanged += new EventHandler(_layoutZoneDocument_IsSelectedChanged);
            _Zones = new ListBox { BorderThickness = new Thickness(0), ItemContainerStyle = App.Current.Resources["ListBoxStyle"] as Style, AllowDrop = true };
            _Zones.SelectionChanged += new SelectionChangedEventHandler(_Zones_SelectionChanged);
            _Zones.MouseDown += new MouseButtonEventHandler(_Zones_MouseDown);
            ContextMenu context_menu = new ContextMenu { };
            MenuItem add_item = new MenuItem { Header = "Add" };
            add_item.Click += _ZoneAdd_Click;
            context_menu.Items.Add(add_item);
            MenuItem del_item = new MenuItem { Header = "Delete" };
            del_item.Click += _ZoneDel_Click;
            context_menu.Items.Add(del_item);
            MenuItem update_item = new MenuItem { Header = "Update", IsEnabled = false };
            update_item.Click += _ZoneUpdate_Click;
            context_menu.Items.Add(update_item);
            _Zones.ContextMenu = context_menu;
            _layoutZones.Content = _Zones;
            _zone_panel.Children.Add(_layoutZones);
        }

        private void InitSubnetPanel()
        {
            _subnet_panel = new LayoutDocumentPane { DockWidth = new GridLength(50) };
            _LayoutSubnet = new LayoutDocument { ContentId = "subnets", Title = "Subnet", CanClose = false, CanFloat = false, IsSelected = false };
            _Subnets = new ListBox { BorderThickness = new Thickness(0), ItemContainerStyle = App.Current.Resources["ListBoxStyle"] as Style, AllowDrop = true };
            _Subnets.SelectionChanged += new SelectionChangedEventHandler(_Subnet_SelectionChanged);
            _Subnets.MouseDown += new MouseButtonEventHandler(_Subnet_MouseDown);
            ContextMenu context_menu = new ContextMenu { };
            MenuItem add_item = new MenuItem { Header = "Add" };
            add_item.Click += _SubnetAdd_Click;
            context_menu.Items.Add(add_item);
            MenuItem del_item = new MenuItem { Header = "Delete" };
            del_item.Click += _SubnetDel_Click;
            context_menu.Items.Add(del_item);
            MenuItem update_item = new MenuItem { Header = "Update", IsEnabled = false };
            update_item.Click += _SubnetUpdate_Click;
            context_menu.Items.Add(update_item);
            _Subnets.ContextMenu = context_menu;
            _LayoutSubnet.Content = _Subnets;
            _subnet_panel.Children.Add(_LayoutSubnet);
        }

        private void InitPolicyPanel()
        {
            _policy_panel = new LayoutDocumentPane { DockWidth = new GridLength(50) };

            /*policy group layout document*/
            _layoutPolicyGroup = new LayoutDocument { ContentId = "policy group", Title = "Policy Group", CanClose = false, CanFloat = false, IsSelected = true };
            _layoutPolicyGroup.IsSelectedChanged += new EventHandler(_layoutPolicyGroup_IsSelectedChanged);
            _PolicyGroup = new ListBox { BorderThickness = new Thickness(0), ItemContainerStyle = App.Current.Resources["ListBoxStyle"] as Style, AllowDrop = true };
            _PolicyGroup.SelectionChanged += new SelectionChangedEventHandler(_PolicyGroup_SelectionChanged);
            _PolicyGroup.MouseDown += new MouseButtonEventHandler(_PolicyGroup_MouseDown);
            ContextMenu context_menu = new ContextMenu { };
            MenuItem add_item = new MenuItem { Header = "Add" };
            add_item.Click += _PolicyGroupAdd_Click;
            context_menu.Items.Add(add_item);
            MenuItem del_item = new MenuItem { Header = "Delete" };
            del_item.Click += _PolicyGroupDel_Click;
            context_menu.Items.Add(del_item);
            MenuItem update_item = new MenuItem { Header = "Update", IsEnabled = false };
            update_item.Click += _PolicyGroupUpdate_Click;
            context_menu.Items.Add(update_item);
            _PolicyGroup.ContextMenu = context_menu;
            _layoutPolicyGroup.Content = _PolicyGroup;
            _policy_panel.Children.Add(_layoutPolicyGroup);

            /*ingress policy layout document*/
            _layoutIngressPolicy = new LayoutDocument { ContentId = "ingress policy", Title = "Ingress Policy", CanClose = false, CanFloat = false, IsSelected = false };
            _layoutIngressPolicy.IsSelectedChanged += new EventHandler(_layoutIngressPolicy_IsSelectedChanged);
            _IngressPolicy = new ListBox { BorderThickness = new Thickness(0), ItemContainerStyle = App.Current.Resources["ListBoxStyle"] as Style, AllowDrop = true };
            _IngressPolicy.SelectionChanged += new SelectionChangedEventHandler(_IngressPolicy_SelectionChanged);
            _IngressPolicy.MouseDown += new MouseButtonEventHandler(_IngressPolicy_MouseDown);
            context_menu = new ContextMenu { };
            add_item = new MenuItem { Header = "Add" };
            add_item.Click += _IngressPolicyAdd_Click;
            context_menu.Items.Add(add_item);
            del_item = new MenuItem { Header = "Delete" };
            del_item.Click += _IngressPolicyDel_Click;
            context_menu.Items.Add(del_item);
            update_item = new MenuItem { Header = "Update", IsEnabled = false };
            update_item.Click += _IngressPolicyUpdate_Click;
            context_menu.Items.Add(update_item);
            _IngressPolicy.ContextMenu = context_menu;
            _layoutIngressPolicy.Content = _IngressPolicy;
            _policy_panel.Children.Add(_layoutIngressPolicy);

            /*egress policy layout document*/
            _layoutEgressPolicy = new LayoutDocument { ContentId = "egress policy", Title = "Egress Policy", CanClose = false, CanFloat = false, IsSelected = false };
            _layoutEgressPolicy.IsSelectedChanged += new EventHandler(_layoutEgressPolicy_IsSelectedChanged);
            _EgressPolicy = new ListBox { BorderThickness = new Thickness(0), ItemContainerStyle = App.Current.Resources["ListBoxStyle"] as Style, AllowDrop = true };
            _EgressPolicy.SelectionChanged += new SelectionChangedEventHandler(_EgressPolicy_SelectionChanged);
            _EgressPolicy.MouseDown += new MouseButtonEventHandler(_EgressPolicy_MouseDown);
            context_menu = new ContextMenu { };
            add_item = new MenuItem { Header = "Add" };
            add_item.Click += _EgressPolicyAdd_Click;
            context_menu.Items.Add(add_item);
            del_item = new MenuItem { Header = "Delete" };
            del_item.Click += _EgressPolicyDel_Click;
            context_menu.Items.Add(del_item);
            update_item = new MenuItem { Header = "Update", IsEnabled = false };
            update_item.Click += _EgressPolicyUpdate_Click;
            context_menu.Items.Add(update_item);
            _EgressPolicy.ContextMenu = context_menu;
            _layoutEgressPolicy.Content = _EgressPolicy;
            _policy_panel.Children.Add(_layoutEgressPolicy);
        }

        private void InitWindowsElement()
        {
            InitZonePanel();
            InitSubnetPanel();
            InitPolicyPanel();
            DrawZoneandSubnetPanel();

        }

        private void OnPreparePropertyItem(object sender, PropertyItemEventArgs e)
        {
            
        }

        private void _EnterpriseAdd_Click(object sender, RoutedEventArgs e)
        {
            Organization org = new Organization(this.rest_client, this._Enterprises);
            org.Owner = Application.Current.MainWindow;
            org.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            org.Show();
        }
        private void _EnterpriseDel_Click(object sender, RoutedEventArgs e)
        {
            if (_Enterprises.SelectedIndex == -1)
            {
                return;
            }

            NuageEnterprise ent = (NuageEnterprise)this._Enterprises.SelectedItem;
            try
            {
                rest_client.DeleteEnterprise(ent.ID);
            }
            catch (NuageException)
            {
                MessageBox.Show("The Enterprise contains one or more domains and cannot be deleted.", "Error");
                return;
            }
            
            _Enterprises.Items.RemoveAt(_Enterprises.SelectedIndex);
        }
        private void _EnterpriseUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _layoutEnterprise_IsSelectedChanged(object sender, EventArgs e)
        {
            try
            {
                enterprises = rest_client.GetEnterprises();
                this._Enterprises.Items.Clear();
                if (enterprises != null && enterprises.Count() > 0)
                {
                    foreach (NuageEnterprise item in enterprises)
                    {
                        this._Enterprises.Items.Add(item);
                    }
                }
            }
            catch (NuageException ex)
            {
                string error = string.Format("Get Enterprises from VSD failed with error {0}.", ex.Message);
                MessageBox.Show(error, "Failed");
            }
            return;
        }

        private void _Enterprises_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._Enterprises == null || this._Enterprises.SelectedItem == null)
                return;
            NuageEnterprise ent = (NuageEnterprise)this._Enterprises.SelectedItem;
            this._propertyGrid.DataContext = ent;

            try
            {
                List<NuageDomain> domains = rest_client.GetL3DomainsInEnterprise(ent.ID);
                this._Domains.Items.Clear();
                if (domains != null && domains.Count() !=0)
                {
                    foreach(NuageDomain item in domains)
                    {
                        this._Domains.Items.Add(item);
                    }
                    
                }

                List<NuageEnterpriseNetworks> network_macros = rest_client.GetNetworkMacrosInEnterprise(ent.ID);
                this._NetworkMacros.Items.Clear();
                if (network_macros != null && network_macros.Count() != 0)
                {
                    foreach (NuageEnterpriseNetworks item in network_macros)
                    {
                        this._NetworkMacros.Items.Add(item);
                    }

                }

                List<NuageNetworkMacroGroups> network_macro_groups = rest_client.GetNetworkMacroGroupsInEnterprise(ent.ID);
                this._NetworkMacroGroups.Items.Clear();
                if (network_macro_groups != null && network_macro_groups.Count() != 0)
                {
                    foreach (NuageNetworkMacroGroups item in network_macro_groups)
                    {
                        this._NetworkMacroGroups.Items.Add(item);
                    }

                }
                
            }
            catch (NuageException ex)
            {
                string error = string.Format("Get data from VSD failed with error {0}.", ex.Message);
                MessageBox.Show(error, "Failed");
            }
        }
        
        private void _DomainAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_Enterprises.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the organization.", "Warning");
                return;
            }

            NuageEnterprise ent = (NuageEnterprise)this._Enterprises.SelectedItem;
            Domain domain = new Domain(this.rest_client, ent.ID, this._Domains);
            domain.Owner = Application.Current.MainWindow;
            domain.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            domain.Show();
        }
        private void _DomainDel_Click(object sender, RoutedEventArgs e)
        {
            if (_Domains.SelectedIndex == -1)
            {
                return;
            }

            NuageDomain domain = (NuageDomain)this._Domains.SelectedItem;
            try
            {
                rest_client.DeleteL3Domain(domain.ID);
            }
            catch (NuageException)
            {
                MessageBox.Show("Domain is in use.Please detach the resource associated with it and retry.", "Error");
                return;
            }

            _Domains.Items.RemoveAt(_Domains.SelectedIndex);
        }
        private void _DomainUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _Domains_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._Domains == null || this._Domains.SelectedItem == null)
                return;

            NuageDomain domain = (NuageDomain)this._Domains.SelectedItem;
            this._propertyGrid.DataContext = domain;

            try
            {

                List<NuageZone> zones = rest_client.GetZonesInDomain(domain.ID);
                this._Zones.Items.Clear();
                if (zones != null && zones.Count() > 0)
                {
                    foreach (NuageZone item in zones)
                    {
                        this._Zones.Items.Add(item);
                    }
                }

                List<NuagePolicyGroup> pg = rest_client.GetPolicyGroupsInDomain(domain.ID);
                this._PolicyGroup.Items.Clear();
                if (pg != null && pg.Count() > 0)
                {
                    foreach (NuagePolicyGroup item in pg)
                    {
                        this._PolicyGroup.Items.Add(item);
                    }
                }

                List<NuageInboundACL> inACL = rest_client.GetL3DomainIngressACLTmpltsInDomain(domain.ID);
                this._IngressPolicy.Items.Clear();
                if (inACL != null && inACL.Count() > 0)
                {
                    foreach (NuageInboundACL item in inACL)
                    {
                        this._IngressPolicy.Items.Add(item);
                    }
                }

                List<NuageOutboundACL> outACL = rest_client.GetL3DomainEgressACLTmpltsInDomain(domain.ID);
                this._EgressPolicy.Items.Clear();
                if (outACL != null && outACL.Count() > 0)
                {
                    foreach (NuageOutboundACL item in outACL)
                    {
                        this._EgressPolicy.Items.Add(item);
                    }
                }

            }
            catch (NuageException ex)
            {
                string error = string.Format("{0}.", ex.Message);
                MessageBox.Show(error, "Failed");
            }

        }

        private void _NetworkMacroAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_Enterprises.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the organization.", "Warning");
                return;
            }

            NuageEnterprise ent = (NuageEnterprise)this._Enterprises.SelectedItem;
            NetworkMacro macro = new NetworkMacro(this.rest_client, ent.ID, this._NetworkMacros);
            macro.Owner = Application.Current.MainWindow;
            macro.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            macro.Show();
        }
        private void _NetworkMacroDel_Click(object sender, RoutedEventArgs e)
        {
            if (_NetworkMacros.SelectedIndex == -1)
            {
                return;
            }

            NuageEnterpriseNetworks macro = (NuageEnterpriseNetworks)this._NetworkMacros.SelectedItem;
            try
            {
                rest_client.DeleteNetworkMacro(macro.ID);
            }
            catch (NuageException)
            {
                MessageBox.Show("Network macro is in use.Please detach the resource associated with it and retry.", "Error");
                return;
            }

            _NetworkMacros.Items.RemoveAt(_NetworkMacros.SelectedIndex);
        }
        private void _NetworkMacroUpdate_Click(object sender, RoutedEventArgs e)
        {

        }
        private void _NetworkMacros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._NetworkMacros == null || this._NetworkMacros.SelectedItem == null)
                return;

            NuageEnterpriseNetworks macro = (NuageEnterpriseNetworks)this._NetworkMacros.SelectedItem;
            this._propertyGrid.DataContext = macro;

        }
        private void _NetworkMacroGroupsAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_Enterprises.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the organization.", "Warning");
                return;
            }

            NuageEnterprise ent = (NuageEnterprise)this._Enterprises.SelectedItem;
            NetworkMacroGroup macro_group = new NetworkMacroGroup(this.rest_client, ent.ID, this._NetworkMacroGroups);
            macro_group.Owner = Application.Current.MainWindow;
            macro_group.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            macro_group.Show();
        }
        private void _NetworkMacroGroupsDel_Click(object sender, RoutedEventArgs e)
        {
            if (_NetworkMacroGroups.SelectedIndex == -1)
            {
                return;
            }

            NuageNetworkMacroGroups macro_group = (NuageNetworkMacroGroups)this._NetworkMacroGroups.SelectedItem;
            try
            {
                rest_client.DeleteNetworkMacroGroups(macro_group.ID);
            }
            catch (NuageException)
            {
                MessageBox.Show("Network macro group is in use.Please detach the resource associated with it and retry.", "Error");
                return;
            }

            _NetworkMacroGroups.Items.RemoveAt(_NetworkMacroGroups.SelectedIndex);
        }
        private void _NetworkMacroGroupsUpdate_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AddMacrotoMacroGroup()
        {
            List<NuageBase> macros_candidate = null;
            ListBox selected = new ListBox();

            if (this._Enterprises.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the organization.", "Warning");
                return;
            }
            if (this._NetworkMacroGroups.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the network macro group.", "Warning");
                return;
            }
            NuageNetworkMacroGroups macro_group = (NuageNetworkMacroGroups)this._NetworkMacroGroups.SelectedItem;
            NuageEnterprise ent = (NuageEnterprise)this._Enterprises.SelectedItem;
            try
            {
                List<NuageEnterpriseNetworks> macros_in_enterprise = this.rest_client.GetNetworkMacrosInEnterprise(ent.ID);
                List<NuageEnterpriseNetworks> macros_in_group = this.rest_client.GetNetworkMacrosInGroup(macro_group.ID);
                
                if (macros_in_enterprise != null)
                {
                    NuageCompare comp = new NuageCompare();
                    macros_candidate = macros_in_enterprise.Except(macros_in_group, comp).ToList();
                }
            }
            catch (NuageException)
            {
                string error = String.Format("Get network macros from  organization {0} failed", ent.name);
                MessageBox.Show("error", "Failed");
                return;
            }
            SelectWin macro_win = new SelectWin(macros_candidate, selected, "Network Macro");
            macro_win.Owner = Application.Current.MainWindow;
            macro_win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            macro_win.ShowDialog();

            if (selected.HasItems)
            {
                selected.SelectedIndex = 0;
                NuageEnterpriseNetworks selected_macro = (NuageEnterpriseNetworks)selected.SelectedItem;
                
                try
                {
                    this.rest_client.AddNetworkMacrosToGroup(macro_group.ID, selected_macro.ID);
                    _CommonElement.Items.Add(selected_macro);
                }
                catch (NuageException)
                {
                    string error = String.Format("Add network macros {0} from group {1} failed", selected_macro.name, macro_group.name);
                    MessageBox.Show("error", "Failed");
                    return;
                }
            }


        }

        private void RemoveMacrofromMacroGroup()
        {
            if (this._NetworkMacroGroups.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the network macro group.");
                return;
            }

            if (this._CommonElement.SelectedIndex == -1)
            {
                return;
            }
            NuageEnterpriseNetworks macro = (NuageEnterpriseNetworks)this._CommonElement.SelectedItem;
            NuageNetworkMacroGroups macro_group = (NuageNetworkMacroGroups)this._NetworkMacroGroups.SelectedItem;
            try
            {
                rest_client.DeleteNetworkMacrosFromGroup(macro_group.ID, macro.ID);
                _CommonElement.Items.RemoveAt(_CommonElement.SelectedIndex);
            }
            catch (NuageException)
            {
                MessageBox.Show("Remove network macro from macro group failed.", "Error");
                return;
            }

            return;
        }
        private void _NetworkMacroGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._NetworkMacroGroups == null || this._NetworkMacroGroups.SelectedItem == null)
                return;

            NuageNetworkMacroGroups macro_group = (NuageNetworkMacroGroups)this._NetworkMacroGroups.SelectedItem;
            this._propertyGrid.DataContext = macro_group;
            try
            {

                List<NuageEnterpriseNetworks> network_macros = rest_client.GetNetworkMacrosInGroup(macro_group.ID);
                this._CommonElement.Items.Clear();
                if (network_macros != null && network_macros.Count() > 0)
                {
                    foreach (NuageEnterpriseNetworks item in network_macros)
                    {
                        this._CommonElement.Items.Add(item);
                    }

                }

            }
            catch (NuageException ex)
            {
                string error = string.Format("{0}.", ex.Message);
                MessageBox.Show(error, "Failed");
            }

        }
        private void _ZoneAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this._Domains.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the domain.", "Warning");
                return;
            }

            NuageDomain domain = (NuageDomain)this._Domains.SelectedItem;
            Zone zone = new Zone(this.rest_client, domain.ID, this._Zones);
            zone.Owner = Application.Current.MainWindow;
            zone.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            zone.Show();
        }
        private void _ZoneDel_Click(object sender, RoutedEventArgs e)
        {
            if (this._Zones.SelectedIndex == -1)
            {
                return;
            }

            NuageZone zone = (NuageZone)this._Zones.SelectedItem;
            try
            {
                rest_client.DeleteZone(zone.ID);
            }
            catch (NuageException)
            {
                MessageBox.Show("Zone is in use.Please detach the resource associated with it and retry.", "Error");
                return;
            }

            _Zones.Items.RemoveAt(_Zones.SelectedIndex);
        }
        private void _ZoneUpdate_Click(object sender, RoutedEventArgs e)
        {

        }
        private void _Zones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._Zones == null || this._Zones.SelectedItem == null)
                return;
            NuageZone zone = (NuageZone)this._Zones.SelectedItem;
            this._propertyGrid.DataContext = zone;

            try
            {
                List<NuageSubnet> subnets = rest_client.GetSubnetsInZone(zone.ID);
                this._Subnets.Items.Clear();
                if (subnets != null && subnets.Count() > 0)
                {
                    foreach (NuageSubnet item in subnets)
                    {
                        this._Subnets.Items.Add(item);
                    }

                }

            }
            catch (NuageException ex)
            {
                string error = string.Format("Get subnets from VSD failed with error {0}.", ex.Message);
                MessageBox.Show(error, "Failed");
            }
        }
        private void _SubnetAdd_Click(object sender, RoutedEventArgs e)
        {
            SubnetAdd();
        }
        private void _SubnetDel_Click(object sender, RoutedEventArgs e)
        {
            if (this._Subnets.SelectedIndex == -1)
            {
                return;
            }
            SubnetDel();

        }
        private void _SubnetUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _Subnet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._Subnets == null || this._Subnets.SelectedItem == null)
                return;

            this._propertyGrid.DataContext = this._Subnets.SelectedItem;

            _layoutZoneDocument_IsSelectedChanged(sender, e);

            //Get virtual machines in the subnet
            try
            {
                List<NuageVms> vms = rest_client.GetVirtualMachinesInSubnet(((NuageSubnet)this._Subnets.SelectedItem).ID, null);
                this._CommonElement.Items.Clear();
                if (vms != null && vms.Count() > 0)
                {
                    foreach (NuageVms item in vms)
                    {
                        this._CommonElement.Items.Add(item);
                    }

                }
            }
            catch (NuageException)
            {
                string error = string.Format("Get virtualmachine from VSD in subnets {0} failed.", ((NuageSubnet)this._Subnets.SelectedItem).name);
                MessageBox.Show(error, "Failed");
            }
        }

        private void SubnetAdd()
        {
            if (this._Zones.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the zone.", "Warning");
                return;
            }

            NuageZone zone = (NuageZone)this._Zones.SelectedItem;

            Subnet subnet = new Subnet(rest_client, zone.ID, this._Subnets);
            subnet.Owner = Application.Current.MainWindow;
            subnet.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            subnet.Show();
        }
        private void SubnetDel()
        {
            NuageSubnet subnet = (NuageSubnet)this._Subnets.SelectedItem;

            try
            {
                rest_client.DeleteSubnet(subnet.ID);
                _Subnets.Items.RemoveAt(_Subnets.SelectedIndex);
            }
            catch (NuageException)
            {
                MessageBox.Show("Subnet is in use.Please detach the resource associated with it and retry.", "Error");
                return;
            }

            return;
        }
        private void SubnetUpdate()
        {

        }

        private void AddvPorttoPolicyGroup()
        {
            if (this._Domains.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the domain.", "Warning");
                return;
            }
            if (this._PolicyGroup.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the policy group.", "Warning");
                return;
            }

            NuageDomain domain = (NuageDomain)this._Domains.SelectedItem;
            NuagePolicyGroup pg = (NuagePolicyGroup)this._PolicyGroup.SelectedItem;

            PolicyGroupVports pg_vport = new PolicyGroupVports(rest_client, domain.ID, pg.ID, this._CommonElement);
            pg_vport.Owner = Application.Current.MainWindow;
            pg_vport.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            pg_vport.Show();
        }

        private void RemovevPortfromPolicyGroup()
        {
            if (this._PolicyGroup.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the policy group disassociate from.");
                return;
            }
            NuageVport vport = (NuageVport)this._CommonElement.SelectedItem;
            NuagePolicyGroup pg = (NuagePolicyGroup)this._PolicyGroup.SelectedItem;
            try
            {
                rest_client.DeletevPortsFromPolicyGroup(pg.ID, vport.ID);
                _CommonElement.Items.RemoveAt(_CommonElement.SelectedIndex);
            }
            catch (NuageException)
            {
                MessageBox.Show("Remove vport from policy group failed.", "Error");
                return;
            }

            return;
        }

        private void AclRuleAdd(string direction)
        {
            string acl_id;
            if (_Domains.SelectedIndex == -1)
                return;

            NuageDomain domain = (NuageDomain)_Domains.SelectedItem;
            if (direction.Equals("ingress"))
            {
                if (this._IngressPolicy.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select the ingress ACL.", "Warning");
                    return;
                }
                acl_id = ((NuageInboundACL)this._IngressPolicy.SelectedItem).ID;
            }
            else
            {
                if (this._EgressPolicy.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select the egress ACL.", "Warning");
                    return;
                }

                acl_id = ((NuageOutboundACL)this._EgressPolicy.SelectedItem).ID;
            }

            ACLRule acl_rule = new ACLRule(direction, rest_client, domain.parentID, domain.ID, acl_id, this._CommonElement);
            acl_rule.Owner = Application.Current.MainWindow;
            acl_rule.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            acl_rule.Show();
        }
        private void AclRuleDel(string direction)
        {
            NuageACLRule acl_rule = (NuageACLRule)this._CommonElement.SelectedItem;

            try
            {
                rest_client.DeleteACLRule(acl_rule.ID, direction);
                _CommonElement.Items.RemoveAt(_CommonElement.SelectedIndex);
            }
            catch (NuageException)
            {
                string error = string.Format("Delete {0} ACL rule {1} failed.", direction, acl_rule.ID);
                MessageBox.Show(error, "Error");
                return;
            }

            return;
        }
        private void AclRuleUpdate(string direction)
        {

        }

        private void _CommonElementAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this._layoutCommonElement.Title.Equals("Virtual Machine"))
            {
                MessageBox.Show("add virtual machine");
            }

            if (this._layoutCommonElement.Title.Equals("vPorts"))
            {
                AddvPorttoPolicyGroup();
            }

            if (this._layoutCommonElement.Title.Equals("Ingress Security Policy Entries"))
            {
                AclRuleAdd("ingress");
            }

            if (this._layoutCommonElement.Title.Equals("Egress Security Policy Entries"))
            {
                AclRuleAdd("egress");
            }

            if (this._layoutCommonElement.Title.Equals("Network Macros"))
            {
                AddMacrotoMacroGroup();
            }
            
            
        }
        private void _CommonElementDel_Click(object sender, RoutedEventArgs e)
        {
            if (this._CommonElement.SelectedIndex == -1)
            {
                return;
            }

            if (this._layoutCommonElement.Title.Equals("Virtual Machine"))
            {
                MessageBox.Show("delete virtual machine");
            }
            if (this._layoutCommonElement.Title.Equals("vPorts"))
            {
                RemovevPortfromPolicyGroup();
            }

            if (this._layoutCommonElement.Title.Equals("Ingress Security Policy Entries"))
            {
                AclRuleDel("ingress");
            }

            if (this._layoutCommonElement.Title.Equals("Egress Security Policy Entries"))
            {
                AclRuleDel("egress");
            }

            if (this._layoutCommonElement.Title.Equals("Network Macros"))
            {
                RemoveMacrofromMacroGroup();
            }
            

        }
        private void _CommonElementUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _CommonElement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._CommonElement == null || this._CommonElement.SelectedItem == null)
                return;
            //NuageSubnet subnet = (NuageSubnet)this._CommonElement.SelectedItem;
            this._propertyGrid.DataContext = this._CommonElement.SelectedItem;
        }
        private void _IngressPolicyAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this._Domains.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the domain.", "Warning");
                return;
            }

            NuageDomain domain = (NuageDomain)this._Domains.SelectedItem;
            IngressACL acl = new IngressACL(this.rest_client, domain.ID, this._IngressPolicy);
            acl.Owner = Application.Current.MainWindow;
            acl.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            acl.Show();
        }
        private void _IngressPolicyDel_Click(object sender, RoutedEventArgs e)
        {
            if (this._IngressPolicy.SelectedIndex == -1)
            {
                return;
            }

            NuageInboundACL ingress = (NuageInboundACL)this._IngressPolicy.SelectedItem;
            try
            {
                rest_client.DeleteL3DomainIngressACLTmplt(ingress.ID);
            }
            catch (NuageException)
            {
                MessageBox.Show("Delete ingress acl failed.", "Error");
                return;
            }

            _IngressPolicy.Items.RemoveAt(_IngressPolicy.SelectedIndex);
        }
        private void _IngressPolicyUpdate_Click(object sender, RoutedEventArgs e)
        {

        }
        private void _IngressPolicy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._IngressPolicy == null || this._IngressPolicy.SelectedItem == null)
                return;
            NuageInboundACL inACL = (NuageInboundACL)this._IngressPolicy.SelectedItem;
            this._propertyGrid.DataContext = inACL;

            try
            {
                List<NuageACLRule> ingress_acl_rules = rest_client.GetRulesInACL(inACL.ID, "ingress");
                this._CommonElement.Items.Clear();
                if (ingress_acl_rules != null && ingress_acl_rules.Count() > 0)
                {
                    foreach (NuageACLRule item in ingress_acl_rules)
                    {
                        this._CommonElement.Items.Add(item);
                    }

                }

            }
            catch (NuageException ex)
            {
                string error = string.Format("Get ingress ACL rules from VSD failed with error {0}.", ex.Message);
                MessageBox.Show(error, "Failed");
            }
        }
        private void _EgressPolicyAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this._Domains.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the domain.", "Warning");
                return;
            }

            NuageDomain domain = (NuageDomain)this._Domains.SelectedItem;
            EgressACL acl = new EgressACL(this.rest_client, domain.ID, this._EgressPolicy);
            acl.Owner = Application.Current.MainWindow;
            acl.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            acl.Show();
        }
        private void _EgressPolicyDel_Click(object sender, RoutedEventArgs e)
        {
            if (this._EgressPolicy.SelectedIndex == -1)
            {
                return;
            }

            NuageOutboundACL egress = (NuageOutboundACL)this._EgressPolicy.SelectedItem;
            try
            {
                rest_client.DeleteL3DomainEgressACLTmplt(egress.ID);
            }
            catch (NuageException)
            {
                MessageBox.Show("Delete egress acl failed.", "Error");
                return;
            }

            _EgressPolicy.Items.RemoveAt(_EgressPolicy.SelectedIndex);
        }
        private void _EgressPolicyUpdate_Click(object sender, RoutedEventArgs e)
        {

        }
        private void _EgressPolicy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._EgressPolicy == null || this._EgressPolicy.SelectedItem == null)
                return;
            NuageOutboundACL outACL = (NuageOutboundACL)this._EgressPolicy.SelectedItem;
            this._propertyGrid.DataContext = outACL;

            try
            {
                List<NuageACLRule> egress_acl_rules = rest_client.GetRulesInACL(outACL.ID, "egress");
                this._CommonElement.Items.Clear();
                if (egress_acl_rules != null && egress_acl_rules.Count() > 0)
                {
                    foreach (NuageACLRule item in egress_acl_rules)
                    {
                        this._CommonElement.Items.Add(item);
                    }

                }

            }
            catch (NuageException ex)
            {
                string error = string.Format("Get ingress ACL rules from VSD failed with error {0}.", ex.Message);
                MessageBox.Show(error, "Failed");
            }
        }
        private void _PolicyGroupAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this._Domains.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the domain.", "Warning");
                return;
            }

            NuageDomain domain = (NuageDomain)this._Domains.SelectedItem;
            PolicyGroup pg = new PolicyGroup(this.rest_client, domain.ID, this._PolicyGroup);
            pg.Owner = Application.Current.MainWindow;
            pg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            pg.Show();
        }
        private void _PolicyGroupDel_Click(object sender, RoutedEventArgs e)
        {
            if (this._PolicyGroup.SelectedIndex == -1)
            {
                return;
            }

            NuagePolicyGroup pg = (NuagePolicyGroup)this._PolicyGroup.SelectedItem;
            try
            {
                rest_client.DeletePolicyGroup(pg.ID);
            }
            catch (NuageException)
            {
                MessageBox.Show("Delete policy group failed.", "Error");
                return;
            }

            _PolicyGroup.Items.RemoveAt(_PolicyGroup.SelectedIndex);
        }
        private void _PolicyGroupUpdate_Click(object sender, RoutedEventArgs e)
        {

        }
        private void _PolicyGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._PolicyGroup == null || this._PolicyGroup.SelectedItem == null)
                return;
            NuagePolicyGroup pg = (NuagePolicyGroup)this._PolicyGroup.SelectedItem;
            this._propertyGrid.DataContext = pg;

            try
            {
                List<NuageVport> vports = rest_client.GetVportInPolicyGroup(pg.ID);
                this._CommonElement.Items.Clear();
                if (vports != null && vports.Count() > 0)
                {
                    foreach (NuageVport item in vports)
                    {
                        this._CommonElement.Items.Add(item);
                    }

                }

            }
            catch (NuageException ex)
            {
                string error = string.Format("Get policy group vports from VSD failed with error {0}.", ex.Message);
                MessageBox.Show(error, "Failed");
            }
        }

        private void _layoutIngressPolicy_IsSelectedChanged(object sender, EventArgs e)
        {
            if (this._layoutCommonElement != null)
                this._layoutCommonElement.Title = "Ingress Security Policy Entries";
            if (this._CommonElement != null && this._CommonElement.Items != null)
                this._CommonElement.Items.Clear();
        }

        private void _layoutEgressPolicy_IsSelectedChanged(object sender, EventArgs e)
        {
            if (this._layoutCommonElement != null)
                this._layoutCommonElement.Title = "Egress Security Policy Entries";
            if (this._CommonElement != null && this._CommonElement.Items != null)
                this._CommonElement.Items.Clear();
        }

        private void _layoutPolicyGroup_IsSelectedChanged(object sender, EventArgs e)
        {
            if (this._layoutCommonElement != null)
                this._layoutCommonElement.Title = "vPorts";
            if (this._CommonElement != null && this._CommonElement.Items != null)
                this._CommonElement.Items.Clear();

        }
        private void _layoutZoneDocument_IsSelectedChanged(object sender, EventArgs e)
        {
            if (this._layoutCommonElement != null)
                this._layoutCommonElement.Title = "Virtual Machine";
            if (this._CommonElement != null && this._CommonElement.Items != null)
                this._CommonElement.Items.Clear();

        }
        private void _layoutNetworkMacroGroupDocument_IsSelectedChanged(object sender, EventArgs e)
        {
            if (this._layoutCommonElement != null)
                this._layoutCommonElement.Title = "Network Macros";
            if (this._CommonElement != null && this._CommonElement.Items != null)
                this._CommonElement.Items.Clear();

        }
        
        private void _Domains_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._Domains.UnselectAll();
        }

        private void _Zones_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._Zones.UnselectAll();
        }

        private void _PolicyGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._PolicyGroup.UnselectAll();
        }

        private void _IngressPolicy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._IngressPolicy.UnselectAll();
        }

        private void _EgressPolicy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._EgressPolicy.UnselectAll();
        }

        private void _CommonElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._CommonElement.UnselectAll();
        }

        private void _NetworkMacro_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._NetworkMacros.UnselectAll();
        }

        private void _NetworkMacroGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._NetworkMacroGroups.UnselectAll();
        }

        private void _Subnet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._Subnets.UnselectAll();
        }

        private void DrawZoneandSubnetPanel()
        {
            this._designPaneGroup.Children.Clear();
            this._designPaneGroup.Orientation = Orientation.Vertical;

            this._designPaneGroup.Children.Add(_zone_panel);
            this._designPaneGroup.Children.Add(_subnet_panel);
        }

        private void DrawPoliciesPanel()
        {
            this._designPaneGroup.Children.Clear();
            this._designPaneGroup.Orientation = Orientation.Horizontal;

            this._designPaneGroup.Children.Add(_policy_panel);

        }
        private void Design_Click(object sender, RoutedEventArgs e)
        {
            DrawZoneandSubnetPanel();
            if (this._layoutZones.IsSelected)
                _layoutZoneDocument_IsSelectedChanged(sender, e);

            this._Policies.Foreground = new SolidColorBrush(Colors.Gray);
            this._Design.Foreground = new SolidColorBrush(Colors.Blue);
        }

        private void Policies_Click(object sender, RoutedEventArgs e)
        {
            DrawPoliciesPanel();
            if (this._layoutPolicyGroup.IsSelected)
                _layoutPolicyGroup_IsSelectedChanged(sender, e);
            if (this._layoutIngressPolicy.IsSelected)
                _layoutIngressPolicy_IsSelectedChanged(sender, e);
            if (this._layoutEgressPolicy.IsSelected)
                _layoutEgressPolicy_IsSelectedChanged(sender, e);

            this._Design.Foreground = new SolidColorBrush(Colors.Gray);
            this._Policies.Foreground = new SolidColorBrush(Colors.Blue);


        }

        private void _floatingIpLayoutAnchroable_IsSelectedChanged(object sender, EventArgs e)
        {
            if (this._Domains.SelectedIndex == -1)
                return;

            try
            {
                List<NuageFloatingIP> floatings = rest_client.GetFloatingIPsInDomain(((NuageDomain)this._Domains.SelectedItem).ID);
                this._floatingIPs.Items.Clear();
                if (floatings != null && floatings.Count() > 0)
                {
                    foreach (NuageFloatingIP item in floatings)
                    {
                        this._floatingIPs.Items.Add(item);
                    }
                }
            }
            catch(NuageException)
            {
                string error = String.Format("Get floating ip from domain {0} failed", ((NuageDomain)this._Domains.SelectedItem).name);
                MessageBox.Show(error, "Error");
            }
        }

        private void _FloatingIpAdd_Click(object sender, RoutedEventArgs e)
        {
            
            if (this._Enterprises.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the organization.", "Warning");
                return;
            }

            if (this._Domains.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the domain.", "Warning");
                return;
            }

            ListBox selected = new ListBox();
            List<NuageSharedNetworkResource> shared_networks = null;
            NuageEnterprise ent = (NuageEnterprise)this._Enterprises.SelectedItem;
            try
            {
                shared_networks = this.rest_client.GetSharedNetworkResourceInEnterprise(ent.ID, null);

            }
            catch (NuageException)
            {
                string error = String.Format("Get shared network resources from  organization {0} failed", ent.name);
                MessageBox.Show("error", "Failed");
                return;
            }
            SelectWin share_win = new SelectWin(shared_networks, selected, "Shared Network");
            share_win.Owner = Application.Current.MainWindow;
            share_win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            share_win.ShowDialog();

            if (selected.HasItems)
            {
                selected.SelectedIndex = 0;
                NuageSharedNetworkResource selected_shared_network = (NuageSharedNetworkResource)selected.SelectedItem;

                try
                {
                    NuageFloatingIP floating_ip = this.rest_client.CreateFloatingIP(((NuageDomain)this._Domains.SelectedItem).ID, selected_shared_network.ID);
                    _floatingIPs.Items.Add(floating_ip);
                }
                catch (NuageException)
                {
                    string error = String.Format("Claim floating ip from shared network {0} failed.", selected_shared_network.name);
                    MessageBox.Show("error", "Failed");
                    return;
                }
            }


        }

        private void _FloatingIpDel_Click(object sender, RoutedEventArgs e)
        {
            if (this._floatingIPs.SelectedIndex == -1)
            {
                return;
            }

            NuageFloatingIP floatIp = (NuageFloatingIP)this._floatingIPs.SelectedItem;
            try
            {
                rest_client.DeleteFloatingIP(floatIp.ID);
            }
            catch (NuageException)
            {
                MessageBox.Show("Delete floating ip failed.", "Error");
                return;
            }

            _floatingIPs.Items.RemoveAt(_floatingIPs.SelectedIndex);
        }
    }

}
