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
        private List<VirtualNetworkAdapter> vNics = new List<VirtualNetworkAdapter>();
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuageVSPWindow));

        public NuageVSPWindow(PowerShellContext powerShellContext, IEnumerable<ContextObject> selectedVMs)
        {
            var contextObject = selectedVMs.First();

            InitializeComponent();

            this.powerShellContext = powerShellContext;

            this.vm = getVirtualMachine(contextObject.ID);
 
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            // Get the vm nics
            int vNicCount = GetVirtualMachineVnics(this.vm.ID);

            logger.InfoFormat("The number of vNic is {0} of virtual machine {1}", vNicCount, this.vm.Name);

            //Draw the main windows according to the number of vNics
            this.DrawMainWindows(vNicCount);

            return;

        }

        //Get the vm
        private VM getVirtualMachine(Guid vmID)
        {
            List<VM> vmList = new List<VM>();
            
            string GetVmScript = @"
                Get-SCVirtualMachine -ID VMID
            ";

            string GetVmScriptFormatted =
                    GetVmScript.Replace("VMID", vmID.ToString());

            this.powerShellContext.ExecuteScript<VM>(
                GetVmScript.ToString(),
                (results, error) =>
                {
                    foreach (VM vm in results)
                    {
                        vmList.Add(vm);
                    }

                });

            return vmList[0];
        }

        private void DrawMainWindows(int vNicCount)
        {
            this.vmName.Text = vm.Name;
            this.vmUUID.Text = vm.ID.ToString();
            this.vmState.Text = vm.StatusString;
            this.vmHost.Text = vm.HostName.ToString();
            return;
        }

        private int GetVirtualMachineVnics(Guid vmID)
        {
            StringBuilder vNicScript = new StringBuilder();

            vNicScript.AppendLine(
                string.Format(
                    "Get-SCVirtualMachine -ID {0} | Get-SCVirtualNetworkAdapter -VM",
                    vmID.ToString()));

            this.powerShellContext.ExecuteScript<VirtualNetworkAdapter>(
                vNicScript.ToString(),
                (results, error) =>
                {
                    foreach (VirtualNetworkAdapter nic in results)
                    {
                        this.vNics.Add(nic);
                    }

                });

            return vNics.Count();
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
