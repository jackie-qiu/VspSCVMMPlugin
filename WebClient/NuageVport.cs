using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public class NuageVport
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string systemType { get; set; }
        public string addressSpoofing { get; set; }
        public string active { get; set; }
        public string operationalState { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string domainID { get; set; }
        public string zoneID { get; set; }
        public string multiNICVPortID { get; set; }
        public string VLANID { get; set; }
        public string associatedFloatingIPID { get; set; }
        public string hasAttachedInterfaces { get; set; }
        public string multicast { get; set; }
        public string associatedMulticastChannelMapID { get; set; }
        public string associatedSendMulticastChannelMapID { get; set; }

        public override string ToString()
        {
            return name;
        }


    }

    public class NuageVportPS
    {
        public List<NuageVport> Value { get; set; }
        public string Count { get; set; }
    }
}
