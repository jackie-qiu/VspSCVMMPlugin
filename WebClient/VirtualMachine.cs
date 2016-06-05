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

        public List<NuageVms> GetVirtualMachines(string filter)
        {
            NuageVms vm = new NuageVms();

            List<NuageVms> result = restproxy.CallRestGetAPI<NuageVms>(vm.get_all_resources(), filter);

            return result;
        }

        public List<NuageVms> GetVirtualMachinesInSubnet(string subnet_id, string filter)
        {
            NuageVms vm = new NuageVms();

            List<NuageVms> result = restproxy.CallRestGetAPI<NuageVms>(vm.get_all_resources_in_subnet(subnet_id), filter);

            return result;
        }

        public NuageVms GetVirtualMachine(string vm_id, string filter)
        {
            NuageVms vm = new NuageVms();

            List<NuageVms> result = restproxy.CallRestGetAPI<NuageVms>(vm.get_resources(vm_id), filter);
            if (result != null && result.Count > 0)
            {
                return result.First();
            }

            return null;
        }

        public NuageVms CreateVirtualMachine(string name, string uuid, string external_id, string vport_id, string ip, string mac)
        {
            NuageVms vm = new NuageVms();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);
            create_params.Add("uuid", uuid);
            create_params.Add("external_id", external_id);
            create_params.Add("mac", mac);
            create_params.Add("vport_id", vport_id);
  

            if (ip != null)
                create_params.Add("ip", ip);

            List<NuageVms> result = restproxy.CallRestPostAPI<NuageVms>(
                                                 vm.post_resource(null),
                                                 vm.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create virtual machine in subnet Failed....");
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public bool DeleteVirtualMachine(string id)
        {
            NuageVms vm = new NuageVms();

            return restproxy.CallRestDeleteAPI<NuageVms>(vm.delete_resource(id));

        }

        public NuageVport GetvPort(string vport_id, string filter)
        {
            NuageVport vport = new NuageVport();

            List<NuageVport> result = restproxy.CallRestGetAPI<NuageVport>(vport.get_resource(vport_id), filter);
            if (result != null && result.Count > 0)
            {
                return result.First();
            }

            return null;
        }

        public List<NuageVport> GetVportInDomain(string domain_id, string filter)
        {
            NuageVport vport = new NuageVport();

            List<NuageVport> result = restproxy.CallRestGetAPI<NuageVport>(vport.get_all_resources_in_domain(domain_id), filter);

            return result;
        }

        public NuageVport CreatevPort(string name, string description, string external_id, string subnet_id)
        {
            NuageVport vPort = new NuageVport();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);

            if (description != null)
                create_params.Add("description", description);
            else
                create_params.Add("description", "VM VPort");

            if (external_id != null)
                create_params.Add("external_id", external_id);

            List<NuageVport> result = restproxy.CallRestPostAPI<NuageVport>(
                                                 vPort.post_resource(subnet_id),
                                                 vPort.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create vport in subnet {0} Failed....", subnet_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public bool DeletevPort(string id)
        {
            NuageVport vport = new NuageVport();

            return restproxy.CallRestDeleteAPI<NuageVport>(vport.delete_resource(id));

        }

    }
}
