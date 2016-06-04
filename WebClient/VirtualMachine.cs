using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Nuage.VSDClient
{
    public class VirtualMachine
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuageClient));
        private RestProxy restproxy { get; set; }

        public VirtualMachine(RestProxy proxy)
        {
            string addinPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                FileInfo configFile = new FileInfo(System.IO.Path.Combine(addinPath + "\\log.conf"));
                XmlConfigurator.Configure(configFile);
            }
            catch (FileNotFoundException ex)
            {
                logger.InfoFormat("Log4net config file not found {0}", ex.Message);
            }
            this.restproxy = proxy;
           
        }

        public NuageVms CreateVirtualMachine(string name, string description)
        {
            return null;
        }

        public List<NuageVms> GetVirtualMachinesInSubnet(string subnet_id, string filter)
        {
            NuageVms vm = new NuageVms();

            List<NuageVms> result = restproxy.CallRestGetAPI<NuageVms>(vm.get_all_resources_in_subnet(subnet_id), filter);

            return result;
        }
    }
}
