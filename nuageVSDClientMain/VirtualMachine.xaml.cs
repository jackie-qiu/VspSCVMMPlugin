using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using Microsoft.SystemCenter.VirtualMachineManager;
using Microsoft.SystemCenter.VirtualMachineManager.Cmdlets;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.PowerShell;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.ContextTypes;
using Microsoft.SystemCenter.VirtualMachineManager.Remoting;
using NetTools;

namespace Nuage.VSDClient.Main
{
    /// <summary>
    /// VirtualMachine.xaml Logical
    /// </summary>
    public partial class VirtualMachine : Window
    {
        INuageClient rest_client;
        NuageSubnet subnet;
        ListBox parent;
        private PowerShellContext psConext;
        private ObservableCollection<VM> vms = new ObservableCollection<VM>();
        private ObservableCollection<VirtualNetworkAdapter> vNics = new ObservableCollection<VirtualNetworkAdapter>();
        public string IPAddress { set; get; }
        HypervManagement hyperv_manager;

        public VirtualMachine(PowerShellContext psConext)
        {
            this.psConext = psConext;
            hyperv_manager = new HypervManagement();
            InitializeComponent();
        }
        public VirtualMachine(INuageClient client, PowerShellContext psConext, NuageSubnet subnet, ListBox parent)
        {
            this.rest_client = client;
            this.psConext = psConext;
            this.parent = parent;
            this.subnet = subnet;
            hyperv_manager = new HypervManagement();
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(OnLoaded);

        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            this._vms.ItemsSource = vms;
            this._vnics.ItemsSource = vNics;
            this.vms.CollectionChanged += vms_CollectionChanged;
            _IPAddress.DataContext = this;
            this._IPAddress.Text = subnet.address;
            GetVirtualMachines();
        }

        void vms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        void FilterCreatedVms(ObservableCollection<VM> vms)
        {
            try
            {
                if (vms == null)
                    return;
                List<NuageVms> vm = this.rest_client.GetVirtualMachines(null);
                for (int i = vms.Count - 1; i >= 0; i -- )
                {
                    foreach (NuageVms item in vm)
                    {
                        if (item.UUID.Equals(vms[i].ID.ToString()))
                        {
                            vms.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            catch (NuageException)
            {
                string error = String.Format("Get virtual machine on vsd failed");
                MessageBox.Show("error", "Failed");
                return;
            }
        }
        private void GetVirtualMachines()
        {
            if (psConext == null)
            {
                return;
            }

            psConext.ExecuteScript<VM>(
               "Get-SCVirtualMachine",
                (results, error) =>
                {
                    if (error != null)
                    {
                        MessageBox.Show(error.Problem);
                    }
                    else
                    {
                        if (results != null)
                        {
                            this.vms.Clear();
                            foreach (VM vm in results)
                            {
                                this.vms.Add(vm);
                            }
                            FilterCreatedVms(this.vms);
                        }
                    }

                });

        }

        private void GetVirtualMachineVnics(Guid vmID)
        {
            string vNicScript = String.Format("Get-SCVirtualMachine -ID {0} | Get-SCVirtualNetworkAdapter", vmID);

            psConext.ExecuteScript<VirtualNetworkAdapter>(
                vNicScript.ToString(),
                (results, error) =>
                {
                    if (error != null)
                    {
                        MessageBox.Show(error.Problem);
                    }
                    else
                    {
                        this.vNics.Clear();
                        foreach (VirtualNetworkAdapter nic in results)
                        {
                            this.vNics.Add(nic);
                        }
                    }

                });

            return;
        }

        public void RemoveVirtualMachine(Guid vmID, string status)
        {
            string vmScript = null;

            if (status.Equals("RUNNING"))
            {
                vmScript = String.Format("Get-SCVirtualMachine -ID {0} | Stop-SCVirtualMachine | Remove-SCVirtualMachine", vmID);
            }
            else
            {
                vmScript = String.Format("Get-SCVirtualMachine -ID {0} | Remove-SCVirtualMachine", vmID);
            }

            psConext.ExecuteScript<VM>(
                vmScript.ToString(),
                (results, error) =>
                {
                    return;
                });

            return;
        }

        private void RebootVirtualMachine(Guid vmID)
        {
            string vmScript = String.Format("Get-SCVirtualMachine -ID {0} | Reset-SCVirtualMachine", vmID);

            psConext.ExecuteScript<VM>(
                vmScript.ToString(),
                (results, error) =>
                {
                    return;
                });

            return;
        }

        private void StartVirtualMachine(Guid vmID)
        {
            string vmScript = String.Format("Get-SCVirtualMachine -ID {0} | Start-SCVirtualMachine", vmID);

            psConext.ExecuteScript<VM>(
                vmScript.ToString(),
                (results, error) =>
                {
                    return;
                });

            return;
        }
        


        private void Select_Click(object sender, RoutedEventArgs e)
        {
            IPNetwork ipnetwork;
            IPNetwork.TryParse(subnet.address, subnet.netmask, out ipnetwork);
            var ipRange = IPAddressRange.Parse(ipnetwork.ToString());
            if (!ipRange.Contains(System.Net.IPAddress.Parse(_IPAddress.Text)))
            {
                string error = string.Format("IP Address {0} is out of range.", _IPAddress.Text);
                MessageBox.Show(error);
                return;
            }

            VirtualNetworkAdapter selected_vnic = (VirtualNetworkAdapter)this._vnics.SelectedItem;
            VM selected_vm = (VM)this._vms.SelectedItem;

            if (String.IsNullOrEmpty(selected_vnic.MACAddress) || selected_vnic.MACAddress.Equals("00:00:00:00:00:00"))
            {
                MessageBox.Show(String.Format("The MAC address is invalid"), "Error");
                return;
            }

            NuageVport vport = null;
            try
            {
                vport = rest_client.CreatevPort(selected_vnic.Name, null, null, subnet.ID);
                if (vport != null)
                {
                    NuageVms vm = rest_client.CreateVirtualMachine(selected_vm.Name, selected_vm.ID.ToString(),
                        selected_vnic.ID.ToString(), vport.ID, _IPAddress.Text, selected_vnic.MACAddress);
                    if (vm != null)
                    {
                        this.parent.Items.Add(vm);
                        parent.SelectedIndex = parent.Items.Count - 1;
                    }

                    hyperv_manager.SetHyperVOVSPort(selected_vm.Name, selected_vm.VMHost.ComputerName, "administrator", "Alcateldc2");
                    //hyperv_manager.SetHyperVOVSPort(selected_vm.Name, selected_vm.VMHost.ComputerName, "administrator", "Aluipr@6");
                    //hyperv_manager.SetHyperVOVSPort(selected_vm.Name, selected_vm.VMHost.ComputerName, "administrator", "tigris1@");

                    if (_isReboot.IsChecked.Value)
                    {
                        if (selected_vm.Status == Microsoft.VirtualManager.Utils.VMComputerSystemState.Running)
                        {
                            RebootVirtualMachine(selected_vm.ID);
                        }
                        else
                        {
                            StartVirtualMachine(selected_vm.ID);
                        }
                        
                    }

                    this.Close();
                }

            }
            catch (NuageException)
            {
                if (vport != null)
                    rest_client.DeletevPort(vport.ID);

                string error = String.Format("Create virtual machine {0} on vsd failed", ((VM)this._vms.SelectedItem).Name);
                MessageBox.Show(error, "Failed");
                return;
            }            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _Select.IsEnabled)
                Select_Click(sender, e);
        }

        private void _vms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_vms.SelectedIndex == -1)
                return;

            GetVirtualMachineVnics(((VM)_vms.SelectedItem).ID);
        }

        private void _vnics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (System.Windows.Controls.Validation.GetHasError(_IPAddress) == true
                || _vms.SelectedIndex == -1 || _vnics.SelectedIndex == -1)
                _Select.IsEnabled = false;
            else
                _Select.IsEnabled = true;
        }

        private void _ipAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Windows.Controls.Validation.GetHasError(_IPAddress) == true
                || _vms.SelectedIndex == -1 || _vnics.SelectedIndex == -1)
                _Select.IsEnabled = false;
            else
                _Select.IsEnabled = true;
        }

        private void _vms_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void restVMTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isReboot.IsChecked = !(_isReboot.IsChecked);
        }
    }
}
