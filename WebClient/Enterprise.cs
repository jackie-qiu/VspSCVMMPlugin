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
    public class Enterprise
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuageClient));
        private RestProxy restproxy { get; set; }

        public Enterprise(RestProxy proxy)
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

        public NuageEnterprise CreateEnterprise(string name, string description)
        {
            NuageEnterprise net_partition = new NuageEnterprise();

            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);
            if(description != null)
                create_params.Add("description", description);

            List<NuageEnterprise> result = restproxy.CallRestPostAPI<NuageEnterprise>(
                                                 net_partition.post_resource(""),
                                                 net_partition.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create enterprise {0} Failed....", name);
                logger.Error(msg);
                throw new NuageException(msg);
            }
            net_partition = result.First();

            return net_partition;

        }

        public List<NuageEnterprise> GetEnterprises(string filter)
        {
            NuageEnterprise ent = new NuageEnterprise();

            List<NuageEnterprise> result = restproxy.CallRestGetAPI<NuageEnterprise>(ent.get_all_resources(), filter);
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Get Enterprises Failed....");
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result;
        }

        public bool DeleteEnterprise(string id)
        {
            NuageEnterprise ent = new NuageEnterprise();

            return restproxy.CallRestDeleteAPI<NuageEnterprise>(ent.delete_resource(id));

        }

        public NuageNetworkMacroGroups CreateNetworkMacroGroup(string ent_id, string name)
        {
            NuageNetworkMacroGroups macro_group = new NuageNetworkMacroGroups();

            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);
            create_params.Add("description", name);

            List<NuageNetworkMacroGroups> result = restproxy.CallRestPostAPI<NuageNetworkMacroGroups>(
                                                 macro_group.post_resource(ent_id),
                                                 macro_group.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create network macro group {0} Failed....", name);
                logger.Error(msg);
                throw new NuageException(msg);
            }
            macro_group = result.First();

            return macro_group;

        }

        public List<NuageNetworkMacroGroups> GetNetworkMacroGroupsInEnterprise(string ent_id, string filter)
        {
            NuageNetworkMacroGroups macro_group = new NuageNetworkMacroGroups();

            List<NuageNetworkMacroGroups> result = restproxy.CallRestGetAPI<NuageNetworkMacroGroups>(macro_group.get_all_resources_in_parent(ent_id), filter);
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Get network macro group from enterprise {0} Failed....", ent_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result;
        }

        public bool DeleteNetworkMacroGroups(string id)
        {
            NuageNetworkMacroGroups network_macro_group = new NuageNetworkMacroGroups();

            return restproxy.CallRestDeleteAPI<NuageNetworkMacroGroups>(network_macro_group.delete_resource(id));

        }

        public List<NuageEnterpriseNetworks> GetNetworkMacrosInGroup(string macro_group_id, string filter)
        {
            NuageNetworkMacroGroups macro_group = new NuageNetworkMacroGroups();

            List<NuageEnterpriseNetworks> result = restproxy.CallRestGetAPI<NuageEnterpriseNetworks>(macro_group.get_network_macro_list(macro_group_id), filter);
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Get network macro list from group {0} Failed....", macro_group_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result;
        }

        public NuageEnterpriseNetworks CreateNetworkMacro(string ent_id, string name, string address, string netmask)
        {
            NuageEnterpriseNetworks network_macro = new NuageEnterpriseNetworks();

            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);
            create_params.Add("address", address);
            create_params.Add("netmask", netmask);

            List<NuageEnterpriseNetworks> result = restproxy.CallRestPostAPI<NuageEnterpriseNetworks>(
                                                 network_macro.post_resource(ent_id),
                                                 network_macro.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create network macro {0} Failed....", name);
                logger.Error(msg);
                throw new NuageException(msg);
            }
            network_macro = result.First();

            return network_macro;

        }

        public List<NuageEnterpriseNetworks> GetNetworkMacrosInEnterprise(string ent_id, string filter)
        {
            NuageEnterpriseNetworks network_macro = new NuageEnterpriseNetworks();

            List<NuageEnterpriseNetworks> result = restproxy.CallRestGetAPI<NuageEnterpriseNetworks>(network_macro.get_all_resources_in_parent(ent_id), filter);
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Get network macro group from enterprise {0} Failed....", ent_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result;
        }

        public bool DeleteNetworkMacro(string id)
        {
            NuageEnterpriseNetworks network_macro = new NuageEnterpriseNetworks();

            return restproxy.CallRestDeleteAPI<NuageEnterpriseNetworks>(network_macro.delete_resource(id));

        }

        public bool AddNetworkMacrosToGroup(string macro_group_id, string network_macro_id)
        {
            List<string> macro_group_list = new List<string>();

            List<NuageEnterpriseNetworks> result = GetNetworkMacrosInGroup(macro_group_id, null);
            foreach (NuageEnterpriseNetworks item in result)
            {
                if (item.ID.Equals(network_macro_id))
                    return true;
                macro_group_list.Add(item.ID);
            }

            macro_group_list.Add(network_macro_id);

            NuageNetworkMacroGroups macro_group = new NuageNetworkMacroGroups();

            return restproxy.CallRestPutAPI<NuageEnterpriseNetworks>(macro_group.get_network_macro_list(macro_group_id),
                                                                           macro_group.put_data(macro_group_list));
        }

        public bool DeleteNetworkMacrosFromGroup(string macro_group_id, string network_macro_id)
        {
            List<string> macro_group_list = new List<string>();

            List<NuageEnterpriseNetworks> result = GetNetworkMacrosInGroup(macro_group_id, null);
            foreach (NuageEnterpriseNetworks item in result)
            {
                if (item.ID.Equals(network_macro_id))
                    continue;
                macro_group_list.Add(item.ID);
            }

            NuageNetworkMacroGroups macro_group = new NuageNetworkMacroGroups();

            return restproxy.CallRestPutAPI<NuageEnterpriseNetworks>(macro_group.get_network_macro_list(macro_group_id),
                                                                           macro_group.put_data(macro_group_list));
        }

    }
}
