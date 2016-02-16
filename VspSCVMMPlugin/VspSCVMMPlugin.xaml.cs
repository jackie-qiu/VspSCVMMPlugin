/*
Copyright © 2012 Nuage. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Microsoft.SystemCenter.VirtualMachineManager;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.PowerShell;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.ContextTypes;

namespace Microsoft.VirtualManager.UI.AddIns.NuageVSP
{
    /// <summary>
    /// Interaction logic for VspSCVMMPlugin.xaml
    /// </summary>
    public partial class NuageVSPWindow : Window
    {
        bool isContextual;
        private PowerShellContext powerShellContext;

        public NuageVSPWindow(PowerShellContext powerShellContext, IEnumerable<VMContext> selectedVMs)
        {
            this.powerShellContext = powerShellContext;
            this.isContextual = selectedVMs != null;

            InitializeComponent();

            if (selectedVMs != null)
            {
                foreach (VMContext vm in selectedVMs)
                {
                    this.VMList.Items.Add(vm);
                }
            }
            else
            {
                this.IsEnabled = false;

                // If the instance isn't contextual, change the title
                this.titleText.Text = "Select the virtual machines to be checkpointed";
            }

            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            // If this add-in is non-contextual, retrieve the list of VMs
            if(!this.isContextual)
            {
                this.powerShellContext.ExecuteScript<VM>(
                    "Get-SCVirtualMachine",
                    (results, error) =>
                    {
                        foreach (VM vm in results)
                        {
                            this.VMList.Items.Add(new VMContext(vm));
                        }

                        this.IsEnabled = true;
                    });
            }

            this.checkpointName.Text = String.Format("Checkpoint -- {0}", Convert.ToString(System.DateTime.Now));
            this.checkpointName.Focus();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Guid> checkpointVMIds = null;
            if (!this.isContextual)
            {
                // Only checkpoint selected VMs
                checkpointVMIds = 
                    this.VMList.SelectedItems.Cast<VMContext>().Select(vm => vm.ID);
            }
            else
            {
                // Checkpoint all VMs in the list
                checkpointVMIds = 
                    this.VMList.Items.Cast<VMContext>().Select(vm => vm.ID);
            }

            StringBuilder checkpointScript = new StringBuilder();
            foreach (Guid vmId in checkpointVMIds)
            {
                checkpointScript.AppendLine(
                    string.Format(
                        "Get-SCVirtualMachine -ID {0} | New-SCVMCheckpoint -Name \"{0}\" -RunAsynchronous",
                        vmId, this.checkpointName.Text));
            }
            
            this.powerShellContext.ExecuteScript(checkpointScript.ToString(),
                (results, error) =>
                {
                    if (error != null)
                    {
                        MessageBox.Show(error.Problem);
                    }
                });


            this.Close();
        }
    }
}
