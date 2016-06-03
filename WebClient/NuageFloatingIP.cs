using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageFloatingIP : NuageBase, NuageServerBaseClass
    {
        public string children { get; set; }
        public override string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string address { get; set; }
        public string assigned { get; set; }
        public string assignedToObjectType { get; set; }
        public string accessControl { get; set; }
        public string owner { get; set; }
        public override string ID { get; set; }
        public override string parentID { get; set; }
        public override string externalID { get; set; }
        public string associatedSharedNetworkResourceID { get; set; }

        public override string ToString()
        {
            return address;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.associatedSharedNetworkResourceID = create_params["shared_netid"];

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string post_resource(string parent_id)
        {
            return "/domains/" + parent_id + "/floatingips";
        }

        public string delete_resource(string id)
        {
            return "/floatingips/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            throw new NotImplementedException();
        }

        public string get_all_resources()
        {
            return "/floatingips";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/domains/" + parent_id + "/floatingips";
        }
    }
}
