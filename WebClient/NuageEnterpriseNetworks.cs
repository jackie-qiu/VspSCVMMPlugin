using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageEnterpriseNetworks : NuageServerBaseClass
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
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string IPType { get; set; }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.name = create_params["name"];
            this.address = create_params["address"];
            this.netmask = create_params["netmask"];

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string post_resource(string parent_id)
        {
            return "/enterprises/" + parent_id + "/enterprisenetworks";
        }

        public string delete_resource(string id)
        {
            return "/enterprisenetworks/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            return "/enterprisenetworks/" + id + "?responseChoice=1";
        }

        public string get_all_resources()
        {
            throw new NotImplementedException();
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/enterprises/" + parent_id + "/enterprisenetworks";
        }

    }
}
