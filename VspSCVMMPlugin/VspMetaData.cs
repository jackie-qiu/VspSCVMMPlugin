using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VirtualManager.UI.AddIns.NuageVSP
{
    public class VspMetaData
    {
        public string domainID { get; set; }
        public string zoneID { get; set; }
        public string policyGroupID { get; set; }
        public string redirectionTargetID { get; set; }
        public string subnetID { get; set; }
        public string StaticIp { get; set; }

        public VspMetaData()
        {

        }
    }
}
