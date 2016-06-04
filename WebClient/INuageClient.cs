using System;
using System.Collections.Generic;

namespace Nuage.VSDClient
{
    public interface INuageClient
    {
        bool AddNetworkMacrosToGroup(string macro_group_id, string network_macro_id);
        NuageACLRule CreateACLRule(string acl_id, string direction, string description, string action, string priority, string ether_type, string protocol, string src_port, string dest_port, string dscp, string location_type, string location_id, string network_type, string network_id);
        NuageDomainTemplate CreateDefaultL3DomainTemplate(string ent_id);
        NuageEnterprise CreateEnterprise(string name, string description);
        NuageFloatingIP CreateFloatingIP(string domain_id, string shared_netid);
        NuageDomain CreateL3Domain(string ent_id, string name, string description);
        NuageOutboundACL CreateL3DomainEgressACLTmplt(string domain_id, string name, string description, string priority, bool implicit_rule, bool allow_ip, bool allow_nonip);
        NuageInboundACL CreateL3DomainIngressACLTmplt(string domain_id, string name, string description, string priority, bool addr_spoof, bool allow_ip, bool allow_nonip);
        NuageEnterpriseNetworks CreateNetworkMacro(string ent_id, string name, string address, string netmask);
        NuageNetworkMacroGroups CreateNetworkMacroGroup(string ent_id, string name, string description);
        NuagePolicyGroup CreatePolicyGroup(string domain_id, string name, string description);
        NuageSubnet CreateSubnet(string zone_id, string name, string description, string ip, string netmask, string gateway);
        NuageZone CreateZone(string domain_id, string name, string description);
        bool DeleteACLRule(string id, string direction);
        bool DeleteEnterprise(string id);
        bool DeleteFloatingIP(string id);
        bool DeleteL3Domain(string id);
        bool DeleteL3DomainEgressACLTmplt(string id);
        bool DeleteL3DomainIngressACLTmplt(string id);
        bool DeleteNetworkMacro(string id);
        bool DeleteNetworkMacroGroups(string id);
        bool DeleteNetworkMacrosFromGroup(string macro_group_id, string network_macro_id);
        bool DeletePolicyGroup(string id);
        bool DeleteSubnet(string id);
        bool DeleteZone(string id);
        NuageDomainTemplate GetDefaultL3DomainTemplate(string ent_id);
        List<NuageEnterprise> GetEnterprises();
        List<NuageFloatingIP> GetFloatingIPs();
        List<NuageFloatingIP> GetFloatingIPsInDomain(string domain_id);
        List<NuageOutboundACL> GetL3DomainEgressACLTmpltsInDomain(string domain_id);
        List<NuageInboundACL> GetL3DomainIngressACLTmpltsInDomain(string domain_id);
        List<NuageDomain> GetL3DomainsInEnterprise(string ent_id);
        List<NuageNetworkMacroGroups> GetNetworkMacroGroupsInEnterprise(string ent_id);
        List<NuageEnterpriseNetworks> GetNetworkMacrosInEnterprise(string ent_id);
        List<NuageEnterpriseNetworks> GetNetworkMacrosInGroup(string macro_group_id);
        List<NuagePolicyGroup> GetPolicyGroupsInDomain(string domain_id);
        List<NuageACLRule> GetRulesInACL(string acl_id, string direction);
        List<NuageSubnet> GetSubnets();
        List<NuageSubnet> GetSubnetsInDomain(string domain_id);
        List<NuageSubnet> GetSubnetsInZone(string zone_id);
        List<NuageVport> GetVportInPolicyGroup(string policy_group_id);
        List<NuageZone> GetZones();
        List<NuageZone> GetZonesInDomain(string domain_id);
        bool LoginVSD();
        bool AddvPortsToPolicyGroup(string policy_group_id, List<string> vport_id);
        bool DeletevPortsFromPolicyGroup(string policy_group_id, string vport_id);
        List<NuagePolicyGroup> GetvPortAssociatePolicyGroups(string vport_id, string filter);
        List<NuageVport> GetVportInDomain(string domain_id, string filter);
        List<NuageSharedNetworkResource> GetSharedNetworkResourceInEnterprise(string ent_id, string filter);
    }
}
