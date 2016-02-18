/*
Copyright © 2012 Nuage. All rights reserved.
*/

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

        private List<TextBlock> vmLogicalNetwork = new List<TextBlock>();
        private List<TextBlock> vmMacAddress = new List<TextBlock>();
        private List<ComboBox> vsdDomain = new List<ComboBox>();
        private List<ComboBox> vsdZone = new List<ComboBox>();
        private List<ComboBox> vsdPolicyGroup = new List<ComboBox>();
        private List<ComboBox> vsdRedirectionTarget = new List<ComboBox>();
        private List<ComboBox> vsdNetwork = new List<ComboBox>();
        private List<TextBox> vmStaticIp = new List<TextBox>();

        public NuageVSPWindow(PowerShellContext powerShellContext, IEnumerable<VMContext> selectedVMs)
        {
            this.vmContext = selectedVMs.First();

            InitializeComponent();

            this.powerShellContext = powerShellContext;

            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            //Get vm
            getVirtualMachine(vmContext.ID);
            // Get the vm nics
            GetVirtualMachineVnics(vmContext.ID);

            return;

        }

        //Get the vm
        private void getVirtualMachine(Guid vmID)
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
                        DrawMainWindows(vNics.Count());
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

            if(vNicCount == 0)
                return;

            //Draw vNICs
            StackPanel TopSP = new StackPanel{Orientation = Orientation.Vertical, Margin = new Thickness(10,0,0,0),VerticalAlignment=VerticalAlignment.Top};
            
            for (int i = 0; i < vNicCount;i++ )
            {
                StackPanel GroupBoxSP = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Top };
                GroupBox gp = new GroupBox{  Header="Network Adapter" + i, HorizontalAlignment=HorizontalAlignment.Stretch,VerticalAlignment=VerticalAlignment.Stretch, Margin = new Thickness(10, 10, 0, 0) };
                gp.Content = GroupBoxSP;
                Grid.SetRow(gp, 1);

                //logical network StackPanel
                StackPanel LogicalNetworkSP = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
                LogicalNetworkSP.Children.Add(new TextBlock { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0), VerticalAlignment = VerticalAlignment.Top, Width = 110, Background = Brushes.LightGray, Text = "Logical Network" });
                TextBlock tb = new TextBlock { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0), VerticalAlignment = VerticalAlignment.Top, Width = 280, Background = Brushes.LightGray };
                LogicalNetworkSP.Children.Add(tb);
                vmLogicalNetwork.Add(tb);
                GroupBoxSP.Children.Add(LogicalNetworkSP);

                //Mac Address StackPanel
                StackPanel MacAddressSP = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
                MacAddressSP.Children.Add(new TextBlock { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0), VerticalAlignment = VerticalAlignment.Top, Width = 110, Background = Brushes.LightGray, Text = "Mac Address" });
                tb = new TextBlock { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0), VerticalAlignment = VerticalAlignment.Top, Width = 280, Background = Brushes.LightGray };
                MacAddressSP.Children.Add(tb);
                vmMacAddress.Add(tb);
                GroupBoxSP.Children.Add(MacAddressSP);

                //VSP element
                DrawVSPElement(GroupBoxSP, "Domain", vsdDomain);
                DrawVSPElement(GroupBoxSP, "Zone", vsdZone);
                DrawVSPElement(GroupBoxSP, "Policy Group",vsdPolicyGroup);
                DrawVSPElement(GroupBoxSP, "Redirection Target",vsdRedirectionTarget);
                DrawVSPElement(GroupBoxSP, "Network",vsdNetwork);

                //Static IP
                StackPanel StaticIpSP = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
                StaticIpSP.Children.Add(new TextBlock { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0), VerticalAlignment = VerticalAlignment.Top, Width = 110, Text = "Static IP Address" });
                TextBox Tbox = new TextBox { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(30, 2, 0, 2), VerticalAlignment = VerticalAlignment.Top, Width = 200 };
                StaticIpSP.Children.Add(Tbox);
                vmStaticIp.Add(Tbox);
                GroupBoxSP.Children.Add(StaticIpSP);

                TopSP.Children.Add(gp);

            }

            this.vNicGroupBox.Content = TopSP;
            //this.RootGrid.Children.Add(TopSP);

        }

        private void DrawVSPElement(StackPanel GroupBoxSP,String name, List<ComboBox> VsdElement)
        {
            StackPanel vsdDomainSP = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            vsdDomainSP.Children.Add(new TextBlock { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0), VerticalAlignment = VerticalAlignment.Top, Width = 110, Text = name });
            ComboBox cb = new ComboBox { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(30, 2, 0, 2), Width = 200 };
            vsdDomainSP.Children.Add(cb);

            VsdElement.Add(cb);
            GroupBoxSP.Children.Add(vsdDomainSP);
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

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            //Connect to VSD and reterive the metadata
            Uri baseUrl = new Uri("https://192.168.239.12:8443/");
            string username = "csproot";
            string password = "csproot";


            nuageVSDSession nuSession = new nuageVSDSession(username, password, "csp", baseUrl);
            nuSession.start();

            MessageBox.Show(nuSession.enterprise[0].name);
            //update the WPF element
        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            //Write the metadata to virtual machine's customer properties

            //IEnumerable<Guid> checkpointVMIds = null;
            //if (!this.isContextual)
            //{
            //    // Only checkpoint selected VMs
            //    checkpointVMIds =
            //        this.VMList.SelectedItems.Cast<VMContext>().Select(vm => vm.ID);
            //}
            //else
            //{
            //    // Checkpoint all VMs in the list
            //    checkpointVMIds =
            //        this.VMList.Items.Cast<VMContext>().Select(vm => vm.ID);
            //}

            //StringBuilder checkpointScript = new StringBuilder();
            //foreach (Guid vmId in checkpointVMIds)
            //{
            //    checkpointScript.AppendLine(
            //        string.Format(
            //            "Get-SCVirtualMachine -ID {0} | New-SCVMCheckpoint -Name \"{0}\" -RunAsynchronous",
            //            vmId, this.checkpointName.Text));
            //}

            //this.powerShellContext.ExecuteScript(checkpointScript.ToString(),
            //    (results, error) =>
            //    {
            //        if (error != null)
            //        {
            //            MessageBox.Show(error.Problem);
            //        }
            //    });


            this.Close();
        }
    }
}
