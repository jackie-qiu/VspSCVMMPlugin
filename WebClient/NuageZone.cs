using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public class NuageZone
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string address { get; set; }
        public string netmask { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string maintenanceMode { get; set; }
        public string publicZone { get; set; }
        public string encryption { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string IPType { get; set; }
        public string numberOfHostsInSubnets { get; set; }
        public string associatedApplicationObjectType { get; set; }
        public string associatedApplicationObjectID { get; set; }
        public string associatedApplicationID { get; set; }
        public string templateID { get; set; }
        public string policyGroupID { get; set; }
        public string multicast { get; set; }
        public string associatedMulticastChannelMapID { get; set; }
    }
}
