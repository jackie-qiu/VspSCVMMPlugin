using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageVmInterface : NuageServerBaseClass
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

        public string post_data(Dictionary<string, string> create_params)
        {
            this.MAC = create_params["mac"];
            this.VPortID = create_params["vport_id"];
            this.externalID = create_params["external_id"];

            if (create_params.ContainsKey("ip"))
            {
                this.IPAddress = create_params["ip"];
            }

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string post_resource(string parent_id)
        {
            return "/vms/" + parent_id + "/vminterfaces";
        }

        public string delete_resource(string id)
        {
            return "/vminterfaces/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            throw new NotImplementedException();
        }

        public string get_all_resources()
        {
            return "/vminterfaces";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/vms/" + parent_id + "/vminterfaces";
        }

        public string get_inteface_for_vport(string vport_id)
        {
            return "/vports/" + vport_id + "/vminterfaces";
        }
    }
}
