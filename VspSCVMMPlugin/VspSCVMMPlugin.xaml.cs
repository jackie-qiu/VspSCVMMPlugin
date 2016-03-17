/*
Copyright © 2012 Nuage. All rights reserved.
*/

using System;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

using Microsoft.SystemCenter.VirtualMachineManager;
using Microsoft.SystemCenter.VirtualMachineManager.Cmdlets;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.PowerShell;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.ContextTypes;
using Microsoft.SystemCenter.VirtualMachineManager.Remoting;
using log4net;
using log4net.Config;
using Nuage.VSDClient;
using Newtonsoft.Json;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using NetTools;

namespace Microsoft.VirtualManager.UI.AddIns.NuageVSP
{
    /// <summary>
    /// Interaction logic for VspSCVMMPlugin.xaml
    /// </summary>
    public partial class NuageVSPWindow : Window
    {
        private PowerShellContext powerShellContext;
        private VM vm;
        private VMContext vmContext;
        private List<VirtualNetworkAdapter> vNics = new List<VirtualNetworkAdapter>();
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuageVSPWindow));

        private List<TextBlock> vmLogicalNetworks = new List<TextBlock>();
        private List<TextBlock> vmMacAddresses = new List<TextBlock>();
        private List<ComboBox> vsdDomains = new List<ComboBox>();
        private List<ComboBox> vsdZones = new List<ComboBox>();
        private List<ComboBox> vsdPolicyGroups = new List<ComboBox>();
        private List<ComboBox> vsdRedirectionTargets = new List<ComboBox>();
        private List<ComboBox> vsdNetworks = new List<ComboBox>();
        private List<TextBox> vmStaticIps = new List<TextBox>();

        private string addinPath = ".\\bin\\AddInPipeline\\AddIns\\NUAGE_scvmm-dev\\VspSCVMMPlugin\\"; 
        private string baseUrl = "";
        private string username = "";
        private string password = "";
        private string organization = "";

        NuageVSDPowerShellSession nuSession;

        public NuageVSPWindow(PowerShellContext powerShellContext, IEnumerable<VMContext> selectedVMs)
        {
            this.vmContext = selectedVMs.First();
            InitializeComponent();
            this.powerShellContext = powerShellContext;
            var vsdConfig = new Dictionary<string, string>();

            try
            {
                FileInfo config = new FileInfo(addinPath + "log.conf");
                XmlConfigurator.Configure(config);
            }
            catch (FileNotFoundException ex)
            {
                logger.InfoFormat("Log4net config file not found {0}", ex.Message);
            }

            readVSDConfigFile();

            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            this.RefreshVSDMetadata();
            
            //Get vm
            GetVirtualMachine(vmContext.ID);
            // Get the vm nics
            GetVirtualMachineVnics(vmContext.ID);

            return;

        }

        private Boolean readVSDConfigFile()
        {
            var vsdConfig = new Dictionary<string, string>();

            try
            {
                foreach (var row in File.ReadLines(addinPath + "vsd.conf"))
                {
                    vsdConfig.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
                }
            }
            catch (FileNotFoundException ex)
            {
                logger.InfoFormat("Config file not found {0}", ex.Message);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Read VSD configure file failed {0}", ex.Message);
                return false;
            }

            if (vsdConfig.Count > 0)
            {
                vsdConfig.TryGetValue("Url", out this.baseUrl);
                vsdConfig.TryGetValue("Username", out this.username);
                vsdConfig.TryGetValue("Password", out this.password);
                vsdConfig.TryGetValue("Organization", out this.organization);

            }

            return true;
        }
        //Get the vm
        private void GetVirtualMachine(Guid vmID)
        {
            List<VM> vmList = new List<VM>();
            StringBuilder GetVmScript = new StringBuilder();

            GetVmScript.AppendLine(string.Format("Get-SCVirtualMachine -ID {0}",vmID));
           
            this.powerShellContext.ExecuteScript<VM>(
                GetVmScript.ToString(),
                (results, error) =>
                {
                    if (error != null)
                    {
                        MessageBox.Show(error.Problem);
                    }
                    else
                    {
                        foreach (VM vm in results)
                        {
                            vmList.Add(vm);
                        }
                        this.vm = vmList.First();
                        //DrawMainWindows(vNics.Count());
                    }

                });

            return;
        }

        private void DrawMainWindows(int vNicCount)
        {
            //Draw general information
            this.vmName.Text = vm.Name;
            this.vmUUID.Text = vm.ID.ToString();
            this.vmState.Text = vm.VirtualMachineState.ToString();
            this.vmHost.Text = vm.HostName.ToString();
            this.vsdEneterprise.ItemsSource = nuSession.GetEnterrpise();

            if(vNicCount == 0)
                return;

            vsdDomains.Clear();
            vsdZones.Clear();
            vsdPolicyGroups.Clear();
            vsdRedirectionTargets.Clear();
            vsdNetworks.Clear();
            vmStaticIps.Clear();

            //Draw vNICs
            StackPanel TopSP = new StackPanel{Orientation = Orientation.Vertical, Margin = new Thickness(10,0,0,0),VerticalAlignment=VerticalAlignment.Top};
            try
            {
                for (int i = 0; i < vNicCount; i++)
                {
                    StackPanel GroupBoxSP = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(10, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    GroupBox gp = new GroupBox
                    {
                        Header = "Network Adapter" + i,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = new Thickness(10, 10, 0, 0)
                    };
                    gp.Content = GroupBoxSP;
                    Grid.SetRow(gp, 1);

                    //logical network StackPanel
                    StackPanel LogicalNetworkSP = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(10, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    LogicalNetworkSP.Children.Add(new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0),
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 110,
                        Background = Brushes.LightGray,
                        Text = "Logical Network"
                    });

                    TextBlock tb = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0),
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 280,
                        Background = Brushes.LightGray,
                        Text = (this.vNics[i].LogicalNetwork == null ? "" : this.vNics[i].LogicalNetwork.ToString())
                    };

                    LogicalNetworkSP.Children.Add(tb);
                    vmLogicalNetworks.Add(tb);
                    GroupBoxSP.Children.Add(LogicalNetworkSP);

                    //Mac Address StackPanel
                    StackPanel MacAddressSP = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(10, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    MacAddressSP.Children.Add(new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0),
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 110,
                        Background = Brushes.LightGray,
                        Text = "Mac Address"
                    });

                    tb = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0),
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 280,
                        Background = Brushes.LightGray,
                        Text = (this.vNics[i].MACAddress == null ? "" : this.vNics[i].MACAddress.ToString())
                    };

                    MacAddressSP.Children.Add(tb);
                    vmMacAddresses.Add(tb);
                    GroupBoxSP.Children.Add(MacAddressSP);

                    //VSP element
                    DrawVSPElement<NuageDomain>(GroupBoxSP, "Domain", vsdDomains, nuSession.GetDomains());
                    DrawVSPElement<NuageZone>(GroupBoxSP, "Zone", vsdZones, nuSession.GetZones());
                    DrawVSPElement<NuagePolicyGroup>(GroupBoxSP, "Policy Group", vsdPolicyGroups, nuSession.GetPolicyGroups());
                    DrawVSPElement<NuageRedirectionTarget>(GroupBoxSP, "Redirection Target", vsdRedirectionTargets, nuSession.GetRedirectionTargets());
                    DrawVSPElement<NuageSubnet>(GroupBoxSP, "Network", vsdNetworks, nuSession.GetSubnets());

                    //Static IP
                    StackPanel StaticIpSP = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(10, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    StaticIpSP.Children.Add(new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0),
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 110,
                        Text = "Static IP Address"
                    });
                    TextBox Tbox = new TextBox
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(30, 2, 0, 2),
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 200
                    };
                    StaticIpSP.Children.Add(Tbox);
                    vmStaticIps.Add(Tbox);
                    GroupBoxSP.Children.Add(StaticIpSP);

                    TopSP.Children.Add(gp);

                }
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
            this.vNicGroupBox.Content = TopSP;
            //this.RootGrid.Children.Add(TopSP);

        }

        private void DrawVSPElement<T>(StackPanel GroupBoxSP, String Text, List<ComboBox> VsdElement, List<T> SourceData)
        {
            try
            {

                StackPanel sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                sp.Children.Add(new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 110,
                    Text = Text
                });
                ComboBox cb = new ComboBox { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(30, 2, 0, 2), Width = 200 };
                sp.Children.Add(cb);
                cb.ItemsSource = SourceData;
                cb.SelectionChanged += new SelectionChangedEventHandler(vsdElement_SelectionChanged);
                VsdElement.Add(cb);
                GroupBoxSP.Children.Add(sp);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        private void GetVirtualMachineVnics(Guid vmID)
        {
            StringBuilder vNicScript = new StringBuilder();

            vNicScript.AppendLine(
                string.Format(
                    "Get-SCVirtualMachine -ID {0} | Get-SCVirtualNetworkAdapter",vmID));

            this.powerShellContext.ExecuteScript<VirtualNetworkAdapter>(
                vNicScript.ToString(),
                (results, error) =>
                {
                    if (error != null)
                    {
                        MessageBox.Show(error.Problem);
                    }
                    else
                    {

                        foreach (VirtualNetworkAdapter nic in results)
                        {
                            this.vNics.Add(nic);
                        }
                        logger.InfoFormat("The number of vNic is {0} of virtual machine {1}", vNics.Count(), vmContext.Name);
                        DrawMainWindows(this.vNics.Count());
                    }

                });

            return;
        }

        private Boolean RefreshVSDMetadata()
        {
            readVSDConfigFile();

            //Connect to VSD and reterive all of the metadata
            Uri uriResult;
            Uri.TryCreate(baseUrl, UriKind.Absolute, out uriResult);
            if (!(uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                logger.ErrorFormat("Invalid Url address {0}.", baseUrl);
                return false;
            }
            nuSession = new NuageVSDPowerShellSession(username, password, organization, uriResult);
            
            if (!nuSession.LoginVSD())
            {
                string result = string.Format("Connect to vsd {0} failed.", this.baseUrl);
                MessageBox.Show(result);
                return false;
            }

            nuSession.GetEnterrpiseRestApi();
            nuSession.GetDomainsRestApi();
            nuSession.GetSubnetRestApi();
            nuSession.GetZoneRestApi();
            nuSession.GetPolicyGroupRestApi();
            nuSession.GetRedirectionTargetRestApi();

            return true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {

            if (this.RefreshVSDMetadata())
            {
                MessageBox.Show("Reresh VSD metadata success!");
            }

            //update the WPF element
            DrawMainWindows(this.vNics.Count());
        }
        private void configMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VspSCVMMPluginConfigVsdWindows configVsdWindow = new VspSCVMMPluginConfigVsdWindows(baseUrl, username, organization);
            configVsdWindow.Show();

        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            Guid uuid = this.vm.ID;
            List<VspMetaData> VspMetaData = GetVspMetadata();
            List<NuageVmInterface> vmInterfaces = new List<NuageVmInterface>();

            if (VspMetaData == null || VspMetaData.Count() == 0)
            {
                logger.ErrorFormat("Can not get the vsp metadata");
                return;
            }

            int i = 0;
            foreach (VspMetaData items in VspMetaData)
            {
                string vPortName = this.vm.Name + uuid.ToString() + "_" + i;
                i ++;

                NuageVport vPort = nuSession.CreateVport(items.subnetID, vPortName);
                if (vPort != null)
                {
                    logger.DebugFormat("Create vPort {0} on VSD success", vPortName);
                    vmInterfaces.Add(new NuageVmInterface
                    {
                        IPAddress = items.StaticIp,
                        MAC = items.MAC,
                        VPortID = vPort.ID,
                        externalID = items.ID
                    });

                }
                else
                {
                    logger.ErrorFormat("Create vPort {0} on VSD failed", vPortName);
                    MessageBox.Show(string.Format("Create vPort {0} on VSD failed", vPortName), "Error");
                    return;
                }
                           
            }

            if (vmInterfaces.Count > 0)
            {
                NuageVms vm = nuSession.CreateVirtualMachine(vmInterfaces, uuid.ToString(), this.vm.Name + uuid.ToString());
                if (vm != null)
                {
                    logger.DebugFormat("Create virtual machine {0} on VSD success", this.vm.Name + uuid.ToString());
                }
            }
            else
            {
                logger.ErrorFormat("Create virtual machine {0} on VSD success", this.vm.Name + uuid.ToString());
                MessageBox.Show(string.Format("Create virtual machine {0} on VSD success", this.vm.Name + uuid.ToString()),"Error");
                return;
            }

            this.Close();

        }

        //Write the metadata to virtual machine's customer properties
        private void SetVirtualMachineCustomerProperties()
        {
            List<VspMetaData> VspMetaData = GetVspMetadata();
            string metaData = null;
            if (VspMetaData == null || VspMetaData.Count() == 0)
            {
                logger.ErrorFormat("Can not get the vsp metadata");
                return;
            }

            try
            {
                metaData = JsonConvert.SerializeObject(VspMetaData);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Convert vsp metadata to Json failed {0}", ex.Message);
                return;
            }

            string NuageMetadataScript = @"
                if (!(Get-SCCustomProperty -Name 'VspMetadata'))
                    {New-SCCustomProperty -Name 'VspMetadata' -Description 'Nuage Network Metadata' -AddMember @('VM')}
                ";

            this.powerShellContext.ExecuteScript(NuageMetadataScript,
                (results, error) =>
                {
                    if (error != null)
                    {
                        MessageBox.Show(error.Problem);
                    }
                });

            string UpdateCustomerPropScript = @"
                    function Update-CustomerProperty ($Vm, [string]$Name, [string]$Value) {
                        process {
                            if ($_) { $Vm = $_ } 
                        }
                        end {
                            $CustomProp = Get-SCCustomProperty -Name $Name
                            $CustomPropValue = $Vm | Get-SCCustomPropertyValue -CustomProperty $CustomProp
                            if ([String]::IsNullOrEmpty($CustomPropValue.Value))
                            {
                                $Vm | Set-SCCustomPropertyValue -CustomProperty $CustomProp -Value $Value -RunAsynchronously
                            }
                            Else
                            {
                                Remove-SCCustomPropertyValue -CustomPropertyValue $CustomPropValue
                                $Vm | Set-SCCustomPropertyValue -CustomProperty $CustomProp -Value $Value -RunAsynchronously
                            }
                        }

                    }

                    $vm = Get-SCVirtualMachine -ID VMID

                    $vm | Update-CustomerProperty -Name VspMetadata -Value 'METADATA' | Out-Null

                ";

            string UpdateScriptFormatted = UpdateCustomerPropScript.Replace("VMID", vmContext.ID.ToString());
            UpdateScriptFormatted = UpdateScriptFormatted.Replace("METADATA", metaData);

            this.powerShellContext.ExecuteScript(UpdateScriptFormatted,
                (results, error) =>
                {
                    if (error != null)
                    {
                        MessageBox.Show(error.Problem);
                    }
                });


            this.Close();
        }

        private List<VspMetaData> GetVspMetadata()
        {
            List<VspMetaData> VspMetaData = new List<VspMetaData>();

            for (int i = 0; i < this.vNics.Count(); i++)
            {
                string domain = "";
                string zone = "";
                string policyGroup = "";
                string redirectionTarget = "";
                NuageSubnet subnet = null;
                string subnetID = "";
                string StaticIp = "";

                if (this.vsdDomains[i].SelectedItem != null)
                {
                    domain = ((NuageDomain)this.vsdDomains[i].SelectedItem).ID;
                }
                if (this.vsdZones[i].SelectedItem != null)
                {
                    zone = ((NuageZone)this.vsdZones[i].SelectedItem).ID;
                }
                if (this.vsdPolicyGroups[i].SelectedItem != null)
                {
                    policyGroup = ((NuagePolicyGroup)this.vsdPolicyGroups[i].SelectedItem).ID;
                }
                if (this.vsdRedirectionTargets[i].SelectedItem != null)
                {
                    redirectionTarget = ((NuageRedirectionTarget)this.vsdRedirectionTargets[i].SelectedItem).ID;
                }
                if (this.vsdNetworks[i].SelectedItem != null)
                {
                    subnet = ((NuageSubnet)this.vsdNetworks[i].SelectedItem);
                    subnetID = subnet.ID;
                }
                if (!System.String.IsNullOrEmpty(this.vmStaticIps[i].Text))
                {
                    StaticIp = this.vmStaticIps[i].Text;
                }

                var ipRange = IPAddressRange.Parse(subnet.address + "/" + subnet.netmask);
                if(!ipRange.Contains(IPAddress.Parse(StaticIp)))
                {
                    logger.ErrorFormat("Static ip subnet {0} must in the range of {1}", subnet.ToString(), subnet.address + "/" + subnet.netmask);
                    MessageBox.Show(string.Format("Static ip of subnet {0} must be in the range of {1}", subnet.ToString(), subnet.address + "/" + subnet.netmask), "Error");
                    return null;
                }

                VspMetaData.Add(new VspMetaData
                {
                    domainID = domain,
                    zoneID = zone,
                    policyGroupID = policyGroup,
                    redirectionTargetID = redirectionTarget,
                    StaticIp = StaticIp,
                    subnetID = subnetID,
                    MAC = this.vNics[i].MACAddress == null ? "" : this.vNics[i].MACAddress.ToString(),
                    ID = this.vNics[i].ID.ToString()
                });

            }


            return VspMetaData;
        }

        private void vsdEneterprise_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NuageEnterprise selectEneterprise = (NuageEnterprise)this.vsdEneterprise.SelectedItem;
            if (selectEneterprise == null) return;
            //Filter domains in this enterprise
            List<NuageDomain> afterFilter = new List<NuageDomain>();

            foreach (NuageDomain items in this.nuSession.GetDomains())
            {
                if (items.parentID.Equals(selectEneterprise.ID))
                {
                    afterFilter.Add(items);
                }
            }

            foreach(ComboBox combox in this.vsdDomains)
                combox.ItemsSource = afterFilter;

            return;

        }

        private void vsdElement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int NicIndex = -1;

            if(isDomainComboBoxSelected(sender))
            {
                NicIndex = this.vsdDomains.IndexOf((ComboBox)sender);
                NuageDomain selectedDomain = (NuageDomain)this.vsdDomains[NicIndex].SelectedItem;
                if (selectedDomain == null) return;

                //Filter Zone, policy group, redirection target in this domain
                List<NuageZone> zoneFilter = new List<NuageZone>();
                List<NuagePolicyGroup> policyGroupFilter = new List<NuagePolicyGroup>();
                List<NuageRedirectionTarget> redirctionTargetFilter = new List<NuageRedirectionTarget>();

                foreach (NuageZone items in this.nuSession.GetZones())
                {
                    if (items.parentID.Equals(selectedDomain.ID))
                    {
                        zoneFilter.Add(items);
                    }
                }

                foreach (NuagePolicyGroup items in this.nuSession.GetPolicyGroups())
                {
                    if (items.parentID.Equals(selectedDomain.ID))
                    {
                        policyGroupFilter.Add(items);
                    }
                }

                foreach (NuageRedirectionTarget items in this.nuSession.GetRedirectionTargets())
                {
                    if (items.parentID.Equals(selectedDomain.ID))
                    {
                        redirctionTargetFilter.Add(items);
                    }
                }

                this.vsdZones[NicIndex].ItemsSource = zoneFilter;
                this.vsdPolicyGroups[NicIndex].ItemsSource = policyGroupFilter;
                this.vsdRedirectionTargets[NicIndex].ItemsSource = redirctionTargetFilter;
                
                return;
            }

            if(isZoneComboBoxSelected(sender))
            {
                NicIndex = this.vsdZones.IndexOf((ComboBox)sender);
                NuageZone selectZone = (NuageZone)this.vsdZones[NicIndex].SelectedItem;
                List<NuageSubnet> subnetFilter = new List<NuageSubnet>();

                if (selectZone == null) return;

                foreach (NuageSubnet items in this.nuSession.GetSubnets())
                {
                    if (items.parentID.Equals(selectZone.ID))
                    {
                        subnetFilter.Add(items);
                    }
                }

                this.vsdNetworks[NicIndex].ItemsSource = subnetFilter;
                return;
            }

            if (isSubnetComboBoxSelected(sender))
            {
                NicIndex = this.vsdNetworks.IndexOf((ComboBox)sender);
                NuageSubnet selectSubnet = (NuageSubnet)this.vsdNetworks[NicIndex].SelectedItem;

                if (selectSubnet == null) return;

                //write subnet cidr to the ip address Text

                foreach (NuageSubnet items in this.nuSession.GetSubnets())
                {
                    if (items.ID.Equals(selectSubnet.ID))
                    {
                        this.vmStaticIps[NicIndex].Clear();
                        this.vmStaticIps[NicIndex].AppendText(items.address);
                    }
                }
                
            }
            

            return;

        }

        private void UpdateVSDComboBoxItems(int NicIndex, string parentID)
        {
 
            //Filter Zone, policy group, redirection target in this domain
            List<NuageZone> zoneFilter = new List<NuageZone>();
            List<NuagePolicyGroup> policyGroupFilter = new List<NuagePolicyGroup>();
            List<NuageRedirectionTarget> redirctionTargetFilter = new List<NuageRedirectionTarget>();
            
            foreach (NuageZone items in this.nuSession.GetZones())
            {
                if (items.parentID.Equals(parentID))
                {
                    zoneFilter.Add(items);
                }
            }

            foreach (NuagePolicyGroup items in this.nuSession.GetPolicyGroups())
            {
                if (items.parentID.Equals(parentID))
                {
                    policyGroupFilter.Add(items);
                }
            }

            foreach (NuageRedirectionTarget items in this.nuSession.GetRedirectionTargets())
            {
                if (items.parentID.Equals(parentID))
                {
                    redirctionTargetFilter.Add(items);
                }
            }

            this.vsdZones[NicIndex].ItemsSource = zoneFilter;
            this.vsdPolicyGroups[NicIndex].ItemsSource = policyGroupFilter;
            this.vsdRedirectionTargets[NicIndex].ItemsSource = redirctionTargetFilter;


        }

        private Boolean isDomainComboBoxSelected(object sender)
        {
            foreach (ComboBox cb in this.vsdDomains)
            {
                if (cb.Equals(sender))
                {
                    return true;
                }
            }

            return false;
        }

        private Boolean isZoneComboBoxSelected(object sender)
        {
            foreach (ComboBox cb in this.vsdZones)
            {
                if (cb.Equals(sender))
                {
                    return true;
                }
            }

            return false;
        }

        private Boolean isSubnetComboBoxSelected(object sender)
        {
            foreach (ComboBox cb in this.vsdNetworks)
            {
                if (cb.Equals(sender))
                {
                    return true;
                }
            }

            return false;
        }

        private void deleteVMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NuageVms nuageVM = this.nuSession.GetVirtualMachineByUUID(this.vm.ID.ToString());
            if (nuageVM == null)
            {
                MessageBox.Show(string.Format("There is no data on VSD about this vm"));
                return;
            }
            List<string> vPortIDs = new List<string>();
            foreach (NuageVmInterface items in nuageVM.interfaces)
            {
                vPortIDs.Add(items.VPortID);
            }
            //Delete the virtualmachine
            if (!this.nuSession.DeleteVirtualMachine(nuageVM))
            {
                MessageBox.Show("Remove virtual machine on VSD failed.");
                return;
            }

            logger.DebugFormat("Remove virtual machine {0} success.", nuageVM.ToString());
            foreach (string item in vPortIDs)
            {
                if (!this.nuSession.DeleteVPort(item))
                {
                    MessageBox.Show(string.Format("Remove vPort {0} on VSD failed.", item));
                    return;
                }

                logger.DebugFormat("Remove vports {0} success.", item);
            }

            MessageBox.Show(string.Format("Remove virtual machine {0} success.", nuageVM.ToString()));
            return;


        }
    }
}
