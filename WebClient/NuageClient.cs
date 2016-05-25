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
    public class NuageClient
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuageClient));
        private string default_l3domain_template_name { get; set; }
        private RestProxy restproxy { get; set; }
        private Enterprise enterprise { get; set; }
        private Domain l3domain { get; set; }

        public NuageClient(string username, string password, string organization, Uri baseUrl, string version)
        {
            restproxy = new RestProxy(username, password, organization, baseUrl, version);

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

            this.default_l3domain_template_name = "SCVMM_L3_Template";

            l3domain = new Domain(restproxy);
            enterprise = new Enterprise(restproxy);
        }

        public Boolean LoginVSD()
        {
            return restproxy.LoginVSD();
        }

        public NuageEnterprise CreateEnterprise(string name)
        {
            return enterprise.CreateEnterprise(name);
        }

        public List<NuageEnterprise> GetEnterprises()
        {
            return enterprise.GetEnterprises(null);
        }

        public bool DeleteEnterprise(string id)
        {
            return enterprise.DeleteEnterprise(id);
        }

        public NuageNetworkMacroGroups CreateNetworkMacroGroup(string ent_id, string name)
        {
            return enterprise.CreateNetworkMacroGroup(ent_id, name);
        }

        public List<NuageNetworkMacroGroups> GetNetworkMacroGroupsInEnterprise(string ent_id)
        {
            return enterprise.GetNetworkMacroGroupsInEnterprise(ent_id, null);
        }

        public bool DeleteNetworkMacroGroups(string id)
        {
            return enterprise.DeleteEnterprise(id);
        }

        public NuageEnterpriseNetworks CreateNetworkMacro(string ent_id, string name, string address, string netmask)
        {
            return enterprise.CreateNetworkMacro(ent_id, name, address, netmask);
        }

        public List<NuageEnterpriseNetworks> GetNetworkMacrosInEnterprise(string ent_id)
        {
            return enterprise.GetNetworkMacrosInEnterprise(ent_id, null);
        }

        public List<NuageEnterpriseNetworks> GetNetworkMacrosInGroup(string macro_group_id)
        {
            return enterprise.GetNetworkMacrosInGroup(macro_group_id, null);
        }

        public bool DeleteNetworkMacro(string id)
        {
            return enterprise.DeleteEnterprise(id);
        }

        public bool AddNetworkMacrosToGroup(string macro_group_id, string network_macro_id)
        {
            return enterprise.AddNetworkMacrosToGroup(macro_group_id, network_macro_id);
        }

        public bool DeleteNetworkMacrosFromGroup(string macro_group_id, string network_macro_id)
        {
            return enterprise.DeleteNetworkMacrosFromGroup(macro_group_id, network_macro_id);
        }

        public NuageDomainTemplate CreateDefaultL3DomainTemplate(string ent_id)
        {
            return l3domain.CreateL3DomainTemplate(ent_id, this.default_l3domain_template_name);
        }
        public NuageDomainTemplate GetDefaultL3DomainTemplate(string ent_id)
        {
            string filter = string.Format("NAME IS '{0}'", this.default_l3domain_template_name);

            List<NuageDomainTemplate> result = l3domain.GetL3DomainTemplates(ent_id, filter);

            return result.First<NuageDomainTemplate>();
        }

        public NuageDomain CreateL3Domain(string ent_id, string name)
        {
            NuageDomainTemplate tmpt = GetDefaultL3DomainTemplate(ent_id);
            if (tmpt == null)
            {
                tmpt = CreateDefaultL3DomainTemplate(ent_id);
            }

            return l3domain.CreateL3Domain(ent_id, name, tmpt.ID);

        }

        public List<NuageDomain> GetL3DomainsInEnterprise(string ent_id)
        {
            return l3domain.GetL3DomainsInEnterprise(ent_id, null);
        }

        public bool DeleteL3Domain(string id)
        {
            return l3domain.DeleteL3Domain(id);
        }

        public NuageZone CreateZone(string domain_id, string name)
        {
            return l3domain.CreateZone(domain_id, name);
        }

        public List<NuageZone> GetZones()
        {
            return l3domain.GetZones(null);
        }

        public List<NuageZone> GetZonesInDomain(string domain_id)
        {
            return l3domain.GetZonesInDomain(domain_id, null);
        }

        public bool DeleteZone(string id)
        {
            return l3domain.DeleteZone(id);
        }

        public NuageSubnet CreateSubnet(string zone_id, string name, string ip, string netmask, string gateway)
        {
            return l3domain.CreateSubnet(zone_id, name, ip, netmask, gateway);
        }

        public List<NuageSubnet> GetSubnets()
        {
            return l3domain.GetSubnets(null);
        }

        public List<NuageSubnet> GetSubnetsInZone(string zone_id)
        {
            return l3domain.GetSubnetsInZone(zone_id, null);
        }

        public bool DeleteSubnet(string id)
        {
            return l3domain.DeleteSubnet(id);
        }

        public NuagePolicyGroup CreatePolicyGroup(string domain_id, string name)
        {
            return l3domain.CreatePolicyGroup(domain_id, name);
        }

        public List<NuagePolicyGroup> GetPolicyGroupsInDomain(string domain_id)
        {
            return l3domain.GetPolicyGroupsInDomain(domain_id, null);
        }

        public bool DeletePolicyGroup(string id)
        {
            return l3domain.DeletePolicyGroup(id);
        }

        public NuageFloatingIP CreateFloatingIP(string domain_id, string shared_netid)
        {
            return l3domain.CreateFloatingIP(domain_id, shared_netid);
        }

        public List<NuageFloatingIP> GetFloatingIPs()
        {
            return l3domain.GetFloatingIPs(null);
        }

        public List<NuageFloatingIP> GetFloatingIPsInDomain(string domain_id)
        {
            return l3domain.GetFloatingIPsInDomain(domain_id, null);
        }

        public bool DeleteFloatingIP(string id)
        {
            return l3domain.DeleteFloatingIP(id);
        }

        public NuageInboundACL CreateL3DomainIngressACLTmplt(string domain_id, string name, 
                                                    string priority, bool addr_spoof, bool allow_ip, bool allow_nonip)
        {
            return l3domain.CreateL3DomainIngressACLTmplt(domain_id, name, priority, addr_spoof, allow_ip, allow_nonip);
        }

        public List<NuageInboundACL> GetL3DomainIngressACLTmpltsInDomain(string domain_id)
        {
            return l3domain.GetL3DomainIngressACLTmpltsInDomain(domain_id, null);
        }

        public bool DeleteL3DomainIngressACLTmplt(string id)
        {
            return l3domain.DeleteL3DomainIngressACLTmplt(id);
        }

        public NuageOutboundACL CreateL3DomainEgressACLTmplt(string domain_id, string name,
                                                     string priority, bool implicit_rule, bool allow_ip, bool allow_nonip)
        {
            return l3domain.CreateL3DomainEgressACLTmplt(domain_id, name, priority, implicit_rule, allow_ip, allow_nonip);
        }

        public List<NuageOutboundACL> GetL3DomainEgressACLTmpltsInDomain(string domain_id)
        {
            return l3domain.GetL3DomainEgressACLTmpltsInDomain(domain_id, null);
        }

        public bool DeleteL3DomainEgressACLTmplt(string id)
        {
            return l3domain.DeleteL3DomainEgressACLTmplt(id);
        }

        public NuageACLRule CreateACLRule(string acl_id, string direction, string description,
                                         string action, string priority, string ether_type, string protocol,
                                         string src_port, string dest_port, string dscp, string location_type,
                                         string location_id, string network_type, string network_id)
        {
            return l3domain.CreateACLRule(acl_id, direction, description, action, priority, "0x0800", protocol,
                                          src_port, dest_port, "*", location_type, location_id, network_type, network_id);
        }

        public List<NuageACLRule> GetRulesInACL(string acl_id, string direction)
        {
            return l3domain.GetACLRulesInACL(acl_id, direction, null);
        }

        public bool DeleteACLRule(string id, string direction)
        {
            return l3domain.DeleteACLRule(id, direction);
        }

    }
}
