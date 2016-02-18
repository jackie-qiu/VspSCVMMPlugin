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
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.PowerShell;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.ContextTypes;
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
        private List<VMContext> vmList = new List<VMContext>();
        private VMContext vmContext;
        private List<VirtualNetworkAdapter> vNics = new List<VirtualNetworkAdapter>();
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuageVSPWindow));

        public NuageVSPWindow(PowerShellContext powerShellContext, IEnumerable<VMContext> selectedVMs)
        {
            InitializeComponent();

            this.powerShellContext = powerShellContext;

            foreach (VMContext vm in selectedVMs)
            {
                this.vmList.Add(vm); 
            }

            this.vmContext = vmList[0];
 
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            // Get the vm nics
            int vNicCount = GetVirtualMachineVnics();

            logger.InfoFormat("The number of vNic is {0} of virtual machine {1}", vNicCount, this.vmContext.Name);

            //Draw the main windows according to the number of vNics
            this.DrawMainWindows(vNicCount);

            return;

        }

        private void DrawMainWindows(int vNicCount)
        {
            this.vmName.Text = vmContext.Name;
            this.vmUUID.Text = vmContext.ID.ToString();
            this.vmState.Text = vmContext.Status.ToString();
            return;
        }

        private int GetVirtualMachineVnics()
        {
            StringBuilder vNicScript = new StringBuilder();

            vNicScript.AppendLine(
                string.Format(
                    "Get-SCVirtualMachine -ID {0} | Get-SCVirtualNetworkAdapter -VM",
                    this.vmContext.ID));

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
