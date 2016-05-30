using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageZone : NuageServerBaseClass
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

        public override string ToString()
        {
            string tostring = "";
            tostring += this.name + "\r\n";
            if(this.description == null)
                tostring += "No description given" + "\r\n";
            else 
                tostring += this.description + "\r\n";

            return tostring;

        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.name = create_params["name"];

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string post_resource(string parent_id)
        {
            return "/domains/" + parent_id + "/zones";
        }

        public string delete_resource(string id)
        {
            return "/zones/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            return "/zones/" + id + "?responseChoice=1";
        }

        public string get_all_resources()
        {
            return "/zones";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/domains/" + parent_id + "/zones";
        }

    }

}
