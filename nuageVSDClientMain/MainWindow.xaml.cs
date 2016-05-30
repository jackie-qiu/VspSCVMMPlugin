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

namespace nuageVSDClientMain
{
    /// <summary>
    /// MainWindow.xaml Logical
    /// </summary>
    public partial class MainWindow : Window
    {
        INuageClient rest_client;
        List<NuageEnterprise> enterprises;
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
            return;

        }

        private void OnPreparePropertyItem(object sender, PropertyItemEventArgs e)
        {
            
        }

        private void onAddClick(object sender, RoutedEventArgs e)
        {



        }

        private void onDelClick(object sender, RoutedEventArgs e)
        {

        }

        private void _layoutEnterprise_IsSelectedChanged(object sender, EventArgs e)
        {
            try
            {
                enterprises = rest_client.GetEnterprises();
                if (enterprises != null)
                {
                    this._Enterprises.Items.Clear();
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
                if (domains != null)
                {
                    this._Domains.Items.Clear();
                    foreach(NuageDomain item in domains)
                    {
                        this._Domains.Items.Add(item);
                    }
                    
                }
                
            }
            catch (NuageException ex)
            {
                string error = string.Format("Get domains from VSD failed with error {0}.", ex.Message);
                MessageBox.Show(error, "Failed");
            }
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
                if (zones != null)
                {
                    this._Zones.Items.Clear();
                    foreach (NuageZone item in zones)
                    {
                        this._Zones.Items.Add(item);
                    }

                }

                List<NuagePolicyGroup> pg = rest_client.GetPolicyGroupsInDomain(domain.ID);
                if (pg != null)
                {
                    this._PolicyGroup.Items.Clear();
                    foreach (NuagePolicyGroup item in pg)
                    {
                        this._PolicyGroup.Items.Add(item);
                    }

                }

                List<NuageInboundACL> inACL = rest_client.GetL3DomainIngressACLTmpltsInDomain(domain.ID);
                if (inACL != null)
                {
                    this._IngressPolicy.Items.Clear();
                    foreach (NuageInboundACL item in inACL)
                    {
                        this._IngressPolicy.Items.Add(item);
                    }

                }

                List<NuageOutboundACL> outACL = rest_client.GetL3DomainEgressACLTmpltsInDomain(domain.ID);
                if (outACL != null)
                {
                    this._EgressPolicy.Items.Clear();
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

        private void _Zones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._Zones == null || this._Zones.SelectedItem == null)
                return;
            NuageZone zone = (NuageZone)this._Zones.SelectedItem;
            this._propertyGrid.DataContext = zone;

            try
            {
                List<NuageSubnet> subnets = rest_client.GetSubnetsInZone(zone.ID);
                if (subnets != null)
                {
                    this._CommonElement.Items.Clear();
                    foreach (NuageSubnet item in subnets)
                    {
                        this._CommonElement.Items.Add(item);
                    }

                }

            }
            catch (NuageException ex)
            {
                string error = string.Format("Get subnets from VSD failed with error {0}.", ex.Message);
                MessageBox.Show(error, "Failed");
            }
        }

        private void _CommonElement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._CommonElement == null || this._CommonElement.SelectedItem == null)
                return;
            //NuageSubnet subnet = (NuageSubnet)this._CommonElement.SelectedItem;
            this._propertyGrid.DataContext = this._CommonElement.SelectedItem;
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
                if (ingress_acl_rules != null)
                {
                    this._CommonElement.Items.Clear();
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

        private void _EgressPolicy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._EgressPolicy == null || this._EgressPolicy.SelectedItem == null)
                return;
            NuageOutboundACL outACL = (NuageOutboundACL)this._EgressPolicy.SelectedItem;
            this._propertyGrid.DataContext = outACL;

            try
            {
                List<NuageACLRule> egress_acl_rules = rest_client.GetRulesInACL(outACL.ID, "egress");
                if (egress_acl_rules != null)
                {
                    this._CommonElement.Items.Clear();
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

        private void _PolicyGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._PolicyGroup == null || this._PolicyGroup.SelectedItem == null)
                return;
            NuagePolicyGroup pg = (NuagePolicyGroup)this._PolicyGroup.SelectedItem;
            this._propertyGrid.DataContext = pg;

            try
            {
                List<NuageVport> vports = rest_client.GetVportInPolicyGroup(pg.ID);
                if (vports != null)
                {
                    this._CommonElement.Items.Clear();
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
        
        private void _layoutPolicy_IsSelectedChanged(object sender, EventArgs e)
        {
            if (this._layoutCommonElement != null)
                this._layoutCommonElement.Title = "Security Policy Entries";
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
                this._layoutCommonElement.Title = "Subnet";
            if (this._CommonElement != null && this._CommonElement.Items != null)
                this._CommonElement.Items.Clear();

        }

    }

}
