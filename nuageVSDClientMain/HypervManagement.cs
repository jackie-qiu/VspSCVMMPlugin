using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Management.Automation;

using log4net;
using log4net.Config;

namespace Nuage.VSDClient.Main
{
    public class HypervManagement
    {
        private NuagePowerShell nuagePS;
        private string userName = "";
        private string password = "";
        private static readonly ILog logger = LogManager.GetLogger(typeof(HypervManagement));

        public HypervManagement()
        {
            nuagePS = new NuagePowerShell();
        }

        public void Init()
        {
            var config = new Dictionary<string, string>();
            string addinPath;

            addinPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                FileInfo configFile = new FileInfo(System.IO.Path.Combine(addinPath + "\\log.conf"));
                XmlConfigurator.Configure(configFile);
            }
            catch (FileNotFoundException ex)
            {
                logger.ErrorFormat("Log4net config file not found {0}", ex.Message);
            }

            try
            {
                foreach (var row in File.ReadLines(System.IO.Path.Combine(addinPath + "\\hyperv.conf")))
                {
                    config.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
                }
            }
            catch (FileNotFoundException ex)
            {
                logger.ErrorFormat("Config file not found {0}", ex.Message);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Read config file failed {0}", ex.Message);
            }

            if (config.Count > 0)
            {
                config.TryGetValue("Username", out userName);
                config.TryGetValue("Password", out password);
            }
        }
        public void SetHyperVOVSPort(string vmName, string hypervHost)
        {

            IEnumerable<PSObject> output;
            string errors;

            string scripts = @"
                Import-Module C:\openvswitch\usr\bin\OVS.psm1
                Set-VMNetworkAdapterOVSPortDirect -vmName VMNAME -OVSPortName VMNAME
";
            string RestScriptFormatted = scripts.Replace("VMNAME", vmName);


            bool result = nuagePS.RunPowerShellScriptRemote(RestScriptFormatted, hypervHost, userName,
                                   password, out output, out errors);
            if (!result)
            {
                return;
            }

            return;
        }
    }
}
