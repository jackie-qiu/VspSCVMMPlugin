/*
Copyright © 2012 Microsoft. All rights reserved.

This code released under the terms of the Microsoft Public License (MS-PL)

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.

1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the
same meaning here as under U.S. copyright law.
A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Microsoft.SystemCenter.VirtualMachineManager;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.PowerShell;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.ContextTypes;

namespace Microsoft.VirtualManager.UI.AddIns.Examples
{
    /// <summary>
    /// Interaction logic for CheckpointWindow.xaml
    /// </summary>
    public partial class CheckpointWindow : Window
    {
        bool isContextual;
        private PowerShellContext powerShellContext;

        public CheckpointWindow(PowerShellContext powerShellContext, IEnumerable<VMContext> selectedVMs)
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
