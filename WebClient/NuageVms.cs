using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public class NuageVms
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string name { get; set; }
        public string enterpriseName { get; set; }
        public string userName { get; set; }
        public string deleteMode { get; set; }
        public string deleteExpiry { get; set; }
        public NuageResyncInfo resyncInfo { get; set; }
        public List<NuageVmInterface> interfaces { get; set; }
        public string appName { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string UUID { get; set; }
        public string status { get; set; }
        public string reasonType { get; set; }
        public string hypervisorIP { get; set; }
        public string siteIdentifier { get; set; }
        public string enterpriseID { get; set; }
        public string userID { get; set; }
        public string domainIDs { get; set; }
        public string l2DomainIDs { get; set; }
        public string zoneIDs { get; set; }
        public string subnetIDs { get; set; }
        public string VRSID { get; set; }
    }

    public class NuageVmsPS
    {
        public List<NuageVms> Value { get; set; }
        public string Count { get; set; }
    }
}
