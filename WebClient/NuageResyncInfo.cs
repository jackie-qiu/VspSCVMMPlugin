using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public class NuageResyncInfo : NuageBase
    {
        public string children { get; set; }
        public override string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string lastTimeResyncInitiated { get; set; }
        public string status { get; set; }
        public string lastRequestTimestamp { get; set; }
        public string owner { get; set; }
        public override string ID { get; set; }
        public override string parentID { get; set; }
        public override string externalID { get; set; }
    }
}
