using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public class NuageDomain
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string routeDistinguisher { get; set; }
        public string routeTarget { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string maintenanceMode { get; set; }
        public string dhcpServerAddresses { get; set; }
        public string applicationDeploymentPolicy { get; set; }
        public string policyChangeStatus { get; set; }
        public string backHaulRouteDistinguisher { get; set; }
        public string backHaulRouteTarget { get; set; }
        public string backHaulVNID { get; set; }
        public string importRouteTarget { get; set; }
        public string exportRouteTarget { get; set; }
        public string encryption { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string serviceID { get; set; }
        public string customerID { get; set; }
        public string DHCPBehavior { get; set; }
        public string DHCPServerAddress { get; set; }
        public string secondaryDHCPServerAddress { get; set; }
        public string labelID { get; set; }
        public string multicast { get; set; }
        public string PATEnabled { get; set; }
        public string associatedMulticastChannelMapID { get; set; }
        public string stretched { get; set; }
        public string tunnelType { get; set; }
        public string ECMPCount { get; set; }
        public string templateID { get; set; }
        public string uplinkPreference { get; set; }
        public string globalRoutingEnabled { get; set; }
        public string leakingEnabled { get; set; }
    }
}
