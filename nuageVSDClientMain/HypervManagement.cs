using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace Nuage.VSDClient.Main
{
    public class HypervManagement
    {
        private NuagePowerShell nuagePS;

        public HypervManagement()
        {
            nuagePS = new NuagePowerShell();
        }

        public void SetHyperVOVSPort(string vmName, string hypervHost, string hypervUsername, string HypervPasswd)
        {

            IEnumerable<PSObject> output;
            string errors;

            string scripts = @"
                Import-Module C:\openvswitch\usr\bin\OVS.psm1
                Set-VMNetworkAdapterOVSPortDirect -vmName VMNAME -OVSPortName VMNAME
";
            string RestScriptFormatted = scripts.Replace("VMNAME", vmName);


            bool result = nuagePS.RunPowerShellScriptRemote(RestScriptFormatted, hypervHost, hypervUsername,
                                   HypervPasswd, out output, out errors);
            if (!result)
            {
                return;
            }

            return;
        }
    }
}
