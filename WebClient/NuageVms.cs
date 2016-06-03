using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageVms : NuageBase, NuageServerBaseClass
    {
        public string children { get; set; }
        public override string parentType { get; set; }
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
        public override string ID { get; set; }
        public override string parentID { get; set; }
        public override string externalID { get; set; }
        public string UUID { get; set; }
        public string status { get; set; }
        public string reasonType { get; set; }
        public string hypervisorIP { get; set; }
        public string siteIdentifier { get; set; }
        public string enterpriseID { get; set; }
        public string userID { get; set; }
        public List<string> domainIDs { get; set; }
        public List<string> l2DomainIDs { get; set; }
        public List<string> zoneIDs { get; set; }
        public List<string> subnetIDs { get; set; }
        public string VRSID { get; set; }

        public override string ToString()
        {
            return name;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            NuageVmInterface intf = new NuageVmInterface();
            List<NuageVmInterface> vmInterfaces = new List<NuageVmInterface>();

            intf.MAC = create_params["mac"];
            intf.VPortID = create_params["vport_id"];
            intf.externalID = create_params["external_id"];

            if (create_params.ContainsKey("ip"))
            {
                intf.IPAddress = create_params["ip"];
            }
            vmInterfaces.Add(intf);

            this.UUID = create_params["uuid"];
            this.name = create_params["name"];
            this.interfaces = vmInterfaces;

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string post_resource(string parent_id)
        {
            return "/vms";
        }

        public string delete_resource(string id)
        {
            return "/vms/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            throw new NotImplementedException();
        }

        public string get_all_resources()
        {
            return "/vms";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            throw new NotImplementedException();
        }
    }

}
