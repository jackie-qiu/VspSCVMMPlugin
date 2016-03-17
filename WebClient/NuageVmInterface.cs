using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public class NuageVmInterface
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string name { get; set; }
        public string multiNICVPortName { get; set; }
        public string policyDecisionID { get; set; }
        public string domainName { get; set; }
        public string zoneName { get; set; }
        public string attachedNetworkType { get; set; }
        public string netmask { get; set; }
        public string gateway { get; set; }
        public string networkName { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string IPAddress { get; set; }
        public string VMUUID { get; set; }
        public string MAC { get; set; }
        public string associatedFloatingIPAddress { get; set; }
        public string domainID { get; set; }
        public string attachedNetworkID { get; set; }
        public string zoneID { get; set; }
        public string VPortID { get; set; }
        public string tierID { get; set; }
        public string VPortName { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
