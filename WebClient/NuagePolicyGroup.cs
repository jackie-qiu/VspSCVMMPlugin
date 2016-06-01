using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuagePolicyGroup : NuageServerBaseClass
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string external { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string EVPNCommunityTag { get; set; }
        public string templateID { get; set; }
        public string policyGroupID { get; set; }

        public override string ToString()
        {
            string tostring = "";
            tostring += this.name + "\r\n";
            if(this.description == null)
                tostring += "No description given" + "\r\n";
            else 
                tostring += this.description + "\r\n";
            tostring += "Type      " + this.type + "\r\n";
            tostring += "Scope     " + this.entityScope + "\r\n";
            tostring += "EVPN Tag  " + this.EVPNCommunityTag + "\r\n";
            tostring += "PGID      " + this.policyGroupID + "\r\n";

            return tostring;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.name = create_params["name"];
            this.type = "SOFTWARE";
            if(create_params.ContainsKey("description"))
                this.description = create_params["description"];

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string put_data(List<string> put_params)
        {
            string data = JsonConvert.SerializeObject(put_params);

            return data;
        }

        public string post_resource(string parent_id)
        {
            return "/domains/" + parent_id + "/policygroups";
        }

        public string delete_resource(string id)
        {
            return "/policygroups/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            return "/policygroups/" + id + "?responseChoice=1";
        }

        public string put_vport_resource(string id)
        {
            return "/policygroups/" + id + "/vports" + "?responseChoice=1";
        }

        public string get_all_resources()
        {
            return "/policygroups";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/domains/" + parent_id + "/policygroups";
        }

        public string get_vport_vptag_resource(string id)
        {
            return "/vports/" + id + "/policygroups";
        }
    }

}
