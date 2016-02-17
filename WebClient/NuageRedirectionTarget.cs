using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public class NuageRedirectionTarget
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string virtualNetworkID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string endPointType { get; set; }
        public string redundancyEnabled { get; set; }
        public string triggerType { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string ESI { get; set; }
        public string templateID { get; set; }
    }
}
