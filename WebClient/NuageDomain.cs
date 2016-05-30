using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageDomain : NuageServerBaseClass
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string routeDistinguisher { get; set; }
        public string routeTarget { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string maintenanceMode { get; set; }
        public string dhcpServerAddresses { get; set; }
        public string applicationDeploymentPolicy { get; set; }
        public string policyChangeStatus { get; set; }
        public string backHaulRouteDistinguisher { get; set; }
        public string backHaulRouteTarget { get; set; }
        public string backHaulVNID { get; set; }
        public string importRouteTarget { get; set; }
        public string exportRouteTarget { get; set; }
        public string encryption { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string serviceID { get; set; }
        public string customerID { get; set; }
        public string DHCPBehavior { get; set; }
        public string DHCPServerAddress { get; set; }
        public string secondaryDHCPServerAddress { get; set; }
        public string labelID { get; set; }
        public string multicast { get; set; }
        public string PATEnabled { get; set; }
        public string associatedMulticastChannelMapID { get; set; }
        public string stretched { get; set; }
        public string tunnelType { get; set; }
        public string ECMPCount { get; set; }
        public string templateID { get; set; }
        public string uplinkPreference { get; set; }
        public string globalRoutingEnabled { get; set; }
        public string leakingEnabled { get; set; }

        public override string ToString()
        {
            string tostring = "";
            tostring += this.name + "\r\n";
            if (this.description == null)
                tostring += "No description given" + "\r\n";
            else
                tostring += this.description + "\r\n";

            return tostring;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.name = create_params["name"];
            this.templateID = create_params["templateID"];

            if(create_params.ContainsKey("description"))
                this.description = create_params["description"];

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string post_resource(string parent_id) 
        {
            return "/enterprises/" + parent_id + "/domains";
        }

        public string delete_resource(string id)
        {
            return "/domains/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            return "/domains/" + id + "?responseChoice=1";
        }

        public string get_all_resources()
        {
            return "/domains";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/enterprises/" + parent_id + "/domains";
        }

        public string get_all_zones()
        {
            return "/domains/" + this.ID + "/zones";
        }

        public string get_domain_subnets()
        {
            return "/domains/" + this.ID + "/subnets";
        }
    }

}
