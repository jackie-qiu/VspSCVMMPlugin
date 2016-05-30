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
    public class Domain
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuageClient));
        private RestProxy restproxy { get; set; }
        private Dictionary<string, string> PROTO_NAME_TO_NUM = new Dictionary<string, string>();
        private string[] NETWORK_TYPE = { "ANY", "ZONE", "SUBNET", "POLICYGROUP", "ENDPOINT_DOMAIN", "ENDPOINT_ZONE", "ENDPOINT_SUBNET", "ENTERPRISE_NETWORK", "NETWORK_MACRO_GROUP"};
        public Domain(RestProxy proxy)
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

            PROTO_NAME_TO_NUM.Add("any", "ANY");
            PROTO_NAME_TO_NUM.Add("tcp", "6");
            PROTO_NAME_TO_NUM.Add("udp", "17");
            PROTO_NAME_TO_NUM.Add("icmp", "1");
        }

        public NuageDomain CreateL3Domain(string ent_id, string name, string description, string template_id)
        {
            NuageDomain domain = new NuageDomain();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);
            create_params.Add("templateID", template_id);

            if(description != null)
                create_params.Add("description", description);

            List<NuageDomain> result = restproxy.CallRestPostAPI<NuageDomain>(
                                                 domain.post_resource(ent_id),
                                                 domain.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create Domains in enterprise {0} Failed....", ent_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public List<NuageDomain> GetL3Domains(string filter)
        {
            NuageDomain domain = new NuageDomain();

            List<NuageDomain> result = restproxy.CallRestGetAPI<NuageDomain>(domain.get_all_resources(), filter);

            return result;
        }

        public List<NuageDomain> GetL3DomainsInEnterprise(string ent_id, string filter)
        {
            NuageDomain domain = new NuageDomain();

            List<NuageDomain> result = restproxy.CallRestGetAPI<NuageDomain>(domain.get_all_resources_in_parent(ent_id), filter);

            return result;
        }

        public bool DeleteL3Domain(string id)
        {
            NuageDomain domain = new NuageDomain();

            return restproxy.CallRestDeleteAPI<NuageDomain>(domain.delete_resource(id));

        }

        public NuageDomainTemplate CreateL3DomainTemplate(string ent_id, string name)
        {
            NuageDomainTemplate domain_template = new NuageDomainTemplate();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);

            List<NuageDomainTemplate> result = restproxy.CallRestPostAPI<NuageDomainTemplate>(
                                                         domain_template.post_resource(ent_id), 
                                                         domain_template.post_data(create_params)
                                                         );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create Domains Template in enterprise {0} Failed....", ent_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public List<NuageDomainTemplate> GetL3DomainTemplates(string ent_id, string filter)
        {
            NuageDomainTemplate domain_template = new NuageDomainTemplate();

            List<NuageDomainTemplate> result = restproxy.CallRestGetAPI<NuageDomainTemplate>(
                                                         domain_template.get_all_resources_in_parent(ent_id), filter);
            return result;
        }

        public bool DeleteL3DomainTemplate(string id)
        {
            NuageDomainTemplate domain_template = new NuageDomainTemplate();

            return restproxy.CallRestDeleteAPI<NuageDomainTemplate>(domain_template.delete_resource(id));

        }

        public NuageZone CreateZone(string domain_id, string name)
        {
            NuageZone zone = new NuageZone();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);

            List<NuageZone> result = restproxy.CallRestPostAPI<NuageZone>(
                                                 zone.post_resource(domain_id),
                                                 zone.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create zones in domain {0} Failed....", domain_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public List<NuageZone> GetZones(string filter)
        {
            NuageZone zone = new NuageZone();

            List<NuageZone> result = restproxy.CallRestGetAPI<NuageZone>(zone.get_all_resources(), filter);

            return result;
        }

        public List<NuageZone> GetZonesInDomain(string domain_id, string filter)
        {
            NuageZone zone = new NuageZone();

            List<NuageZone> result = restproxy.CallRestGetAPI<NuageZone>(zone.get_all_resources_in_parent(domain_id), filter);

            return result;
        }

        public bool DeleteZone(string id)
        {
            NuageZone zone = new NuageZone();

            return restproxy.CallRestDeleteAPI<NuageZone>(zone.delete_resource(id));

        }

        public NuageSubnet CreateSubnet(string zone_id, string name, string ip, string netmask, string gateway)
        {
            NuageSubnet Subnet = new NuageSubnet();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);
            create_params.Add("ip", ip);
            create_params.Add("netmask", netmask);
            create_params.Add("gateway", gateway);

            List<NuageSubnet> result = restproxy.CallRestPostAPI<NuageSubnet>(
                                                 Subnet.post_resource(zone_id),
                                                 Subnet.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create Subnets in zone {0} Failed....", zone_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public List<NuageSubnet> GetSubnets(string filter)
        {
            NuageSubnet Subnet = new NuageSubnet();

            List<NuageSubnet> result = restproxy.CallRestGetAPI<NuageSubnet>(Subnet.get_all_resources(), filter);

            return result;
        }

        public List<NuageSubnet> GetSubnetsInDomain(string domain_id, string filter)
        {
            NuageSubnet Subnet = new NuageSubnet();

            List<NuageSubnet> result = restproxy.CallRestGetAPI<NuageSubnet>(Subnet.get_all_resources_in_domain(domain_id), filter);

            return result;
        }

        public List<NuageSubnet> GetSubnetsInZone(string zone_id, string filter)
        {
            NuageSubnet Subnet = new NuageSubnet();

            List<NuageSubnet> result = restproxy.CallRestGetAPI<NuageSubnet>(Subnet.get_all_resources_in_parent(zone_id), filter);

            return result;
        }

        public bool DeleteSubnet(string id)
        {
            NuageSubnet Subnet = new NuageSubnet();

            return restproxy.CallRestDeleteAPI<NuageSubnet>(Subnet.delete_resource(id));

        }

        public NuagePolicyGroup CreatePolicyGroup(string domain_id, string name)
        {
            NuagePolicyGroup policy_group = new NuagePolicyGroup();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);
            create_params.Add("description", name);

            List<NuagePolicyGroup> result = restproxy.CallRestPostAPI<NuagePolicyGroup>(
                                                 policy_group.post_resource(domain_id),
                                                 policy_group.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create policy group in domain {0} Failed....", domain_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public List<NuagePolicyGroup> GetPolicyGroups(string filter)
        {
            NuagePolicyGroup policy_group = new NuagePolicyGroup();

            List<NuagePolicyGroup> result = restproxy.CallRestGetAPI<NuagePolicyGroup>(policy_group.get_all_resources(), filter);

            return result;
        }

        public List<NuagePolicyGroup> GetPolicyGroupsInDomain(string domain_id, string filter)
        {
            NuagePolicyGroup policy_group = new NuagePolicyGroup();

            List<NuagePolicyGroup> result = restproxy.CallRestGetAPI<NuagePolicyGroup>(policy_group.get_all_resources_in_parent(domain_id), filter);

            return result;
        }

        public bool DeletePolicyGroup(string id)
        {
            NuagePolicyGroup policy_group = new NuagePolicyGroup();

            return restproxy.CallRestDeleteAPI<NuagePolicyGroup>(policy_group.delete_resource(id));

        }

        public List<NuageVport> GetVportInPolicyGroup(string policy_group_id, string filter)
        {
            NuageVport vport = new NuageVport();

            List<NuageVport> result = restproxy.CallRestGetAPI<NuageVport>(vport.get_vports_for_vptag(policy_group_id), filter);

            return result;
        }

        public NuageFloatingIP CreateFloatingIP(string domain_id, string shared_netid)
        {
            NuageFloatingIP floatingip = new NuageFloatingIP();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("shared_netid", shared_netid);

            List<NuageFloatingIP> result = restproxy.CallRestPostAPI<NuageFloatingIP>(
                                                 floatingip.post_resource(domain_id),
                                                 floatingip.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create floating ip in domain {0} Failed....", domain_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public List<NuageFloatingIP> GetFloatingIPs(string filter)
        {
            NuageFloatingIP floatingip = new NuageFloatingIP();

            List<NuageFloatingIP> result = restproxy.CallRestGetAPI<NuageFloatingIP>(floatingip.get_all_resources(), filter);

            return result;
        }

        public List<NuageFloatingIP> GetFloatingIPsInDomain(string domain_id, string filter)
        {
            NuageFloatingIP floatingip = new NuageFloatingIP();

            List<NuageFloatingIP> result = restproxy.CallRestGetAPI<NuageFloatingIP>(floatingip.get_all_resources_in_parent(domain_id), filter);

            return result;
        }

        public bool DeleteFloatingIP(string id)
        {
            NuageFloatingIP floatingip = new NuageFloatingIP();

            return restproxy.CallRestDeleteAPI<NuageFloatingIP>(floatingip.delete_resource(id));

        }

        public NuageInboundACL CreateL3DomainIngressACLTmplt(string domain_id, string name, string priority, bool addr_spoof, bool allow_ip, bool allow_nonip)
        {
            NuageInboundACL L3DomainIngressACLTmplt = new NuageInboundACL();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            if (name != null)
            {
                create_params.Add("name", name);
                create_params.Add("description", name);
            }

            if (priority != null) create_params.Add("priority", priority);
            if (addr_spoof) create_params.Add("addressSpoof", "true");
            if (allow_ip) create_params.Add("allowIp", "true");
            if (allow_nonip) create_params.Add("allowNonIp", "true");

            List<NuageInboundACL> result = restproxy.CallRestPostAPI<NuageInboundACL>(
                                                 L3DomainIngressACLTmplt.post_resource(domain_id),
                                                 L3DomainIngressACLTmplt.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create ingress acl in domain {0} Failed....", domain_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public List<NuageInboundACL> GetL3DomainIngressACLTmplts(string filter)
        {
            NuageInboundACL L3DomainIngressACLTmplt = new NuageInboundACL();

            List<NuageInboundACL> result = restproxy.CallRestGetAPI<NuageInboundACL>(L3DomainIngressACLTmplt.get_all_resources(), filter);
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Get L3 domain ingress acls Failed....");
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result;
        }

        public List<NuageInboundACL> GetL3DomainIngressACLTmpltsInDomain(string domain_id, string filter)
        {
            NuageInboundACL L3DomainIngressACLTmplt = new NuageInboundACL();

            List<NuageInboundACL> result = restproxy.CallRestGetAPI<NuageInboundACL>(L3DomainIngressACLTmplt.get_all_resources_in_parent(domain_id), filter);

            return result;
        }

        public bool DeleteL3DomainIngressACLTmplt(string id)
        {
            NuageInboundACL L3DomainIngressACLTmplt = new NuageInboundACL();

            return restproxy.CallRestDeleteAPI<NuageInboundACL>(L3DomainIngressACLTmplt.delete_resource(id));

        }

        public NuageOutboundACL CreateL3DomainEgressACLTmplt(string domain_id, string name, string priority, bool implicit_rule, bool allow_ip, bool allow_nonip)
        {
            NuageOutboundACL L3DomainEgressACLTmplt = new NuageOutboundACL();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            if (name != null)
            {
                create_params.Add("name", name);
                create_params.Add("description", name);
            }

            if (priority != null) create_params.Add("priority", priority);
            if (implicit_rule) create_params.Add("installImplicitRules", "true");
            if (allow_ip) create_params.Add("allowIp", "true");
            if (allow_nonip) create_params.Add("allowNonIp", "true");

            List<NuageOutboundACL> result = restproxy.CallRestPostAPI<NuageOutboundACL>(
                                                 L3DomainEgressACLTmplt.post_resource(domain_id),
                                                 L3DomainEgressACLTmplt.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create egress acl in domain {0} Failed....", domain_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public List<NuageOutboundACL> GetL3DomainEgressACLTmplts(string filter)
        {
            NuageOutboundACL L3DomainEgressACLTmplt = new NuageOutboundACL();

            List<NuageOutboundACL> result = restproxy.CallRestGetAPI<NuageOutboundACL>(L3DomainEgressACLTmplt.get_all_resources(), filter);

            return result;
        }

        public List<NuageOutboundACL> GetL3DomainEgressACLTmpltsInDomain(string domain_id, string filter)
        {
            NuageOutboundACL L3DomainEgressACLTmplt = new NuageOutboundACL();

            List<NuageOutboundACL> result = restproxy.CallRestGetAPI<NuageOutboundACL>(L3DomainEgressACLTmplt.get_all_resources_in_parent(domain_id), filter);

            return result;
        }

        public bool DeleteL3DomainEgressACLTmplt(string id)
        {
            NuageOutboundACL L3DomainEgressACLTmplt = new NuageOutboundACL();

            return restproxy.CallRestDeleteAPI<NuageOutboundACL>(L3DomainEgressACLTmplt.delete_resource(id));

        }

        public NuageACLRule CreateACLRule(string acl_id, string direction, Dictionary<string, string> create_params)
        {
            NuageACLRule rule = new NuageACLRule();
            string resource = null;
            if (direction.Equals("ingress"))
            {
                resource = rule.in_post_resource(acl_id);
            }
            else
            {
                resource = rule.eg_post_resource(acl_id);
            }
            List<NuageACLRule> result = restproxy.CallRestPostAPI<NuageACLRule>(
                                                 resource,
                                                 rule.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create {0} rule in acl {1} Failed....", direction, acl_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public NuageACLRule CreateACLRule(string acl_id, string direction, string description,
                                         string action, string priority,string ether_type, string protocol,
                                         string src_port, string dest_port, string dscp, string location_type,
                                         string location_id, string network_type, string network_id)
        {
            NuageACLRule rule = new NuageACLRule();
            Dictionary<string, string> create_params = new Dictionary<string, string>();
            if (!PROTO_NAME_TO_NUM.ContainsKey(protocol))
            {
                string msg = string.Format("Unsupport protocol {0}", protocol);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            if (!NETWORK_TYPE.Contains(location_type) || !NETWORK_TYPE.Contains(network_type))
            {
                string msg = string.Format("Unsupport network type {0} or location type {1}", network_type, location_type);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            string resource = null;
            if (direction.Equals("ingress"))
            {
                resource = rule.in_post_resource(acl_id);
            }
            else
            {
                resource = rule.eg_post_resource(acl_id);
            }

            if (!String.IsNullOrEmpty(description)) create_params["description"] = description;
            if (!String.IsNullOrEmpty(priority)) create_params["priority"] = priority;

            create_params["etherType"] = ether_type;
            create_params["DSCP"] = dscp;
            create_params["protocol"] = PROTO_NAME_TO_NUM[protocol];
            if (protocol.Equals("tcp") || protocol.Equals("udp"))
            {
                create_params["sourcePort"] = src_port;
                create_params["destPort"] = dest_port;
            }
            create_params["locationType"] = location_type;
            if(!location_type.Equals("ANY")) create_params["locationID"] = location_id;
            create_params["networkType"] = network_type;
            if (!network_type.Equals("ANY")) create_params["networkID"] = network_id;
            create_params["action"] = action;
            List<NuageACLRule> result = restproxy.CallRestPostAPI<NuageACLRule>(
                                                 resource,
                                                 rule.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create {0} rule in acl {1} Failed....", direction, acl_id);
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result.First();
        }

        public List<NuageACLRule> GetACLRules(string direction, string filter)
        {
            NuageACLRule rule = new NuageACLRule();
            string resource = null;
            if (direction.Equals("ingress"))
            {
                resource = rule.in_get_all_resources();
            }
            else
            {
                resource = rule.eg_get_all_resources();
            }

            List<NuageACLRule> result = restproxy.CallRestGetAPI<NuageACLRule>(resource, filter);

            return result;
        }

        public List<NuageACLRule> GetACLRulesInDomain(string domain_id, string direction, string filter)
        {
            NuageACLRule rule = new NuageACLRule();
            string resource = null;
            if (direction.Equals("ingress"))
            {
                resource = rule.in_get_all_resources_in_domain(domain_id);
            }
            else
            {
                resource = rule.eg_get_all_resources_in_domain(domain_id);
            }
            List<NuageACLRule> result = restproxy.CallRestGetAPI<NuageACLRule>(resource, filter);

            return result;
        }

        public List<NuageACLRule> GetACLRulesInACL(string acl_id, string direction, string filter)
        {
            NuageACLRule rule = new NuageACLRule();
            string resource = null;
            if (direction.Equals("ingress"))
            {
                resource = rule.in_get_all_resources_in_parent(acl_id);
            }
            else
            {
                resource = rule.eg_get_all_resources_in_parent(acl_id);
            }
            List<NuageACLRule> result = restproxy.CallRestGetAPI<NuageACLRule>(resource, filter);

            return result;
        }

        public bool DeleteACLRule(string id, string direction)
        {
            NuageACLRule rule = new NuageACLRule();
            string resource = null;
            if (direction.Equals("ingress"))
            {
                resource = rule.in_delete_resource(id);
            }
            else
            {
                resource = rule.eg_delete_resource(id);
            }
            return restproxy.CallRestDeleteAPI<NuageACLRule>(resource);

        }

    }
}
