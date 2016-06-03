using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageSubnet : NuageBase, NuageServerBaseClass
    {
        public string children { get; set; }
        public override string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string address { get; set; }
        public string netmask { get; set; }
        public string name { get; set; }
        public string gateway { get; set; }
        public string description { get; set; }
        public string maintenanceMode { get; set; }
        public string routeDistinguisher { get; set; }
        public string routeTarget { get; set; }
        public string vnId { get; set; }
        public string defaultAction { get; set; }
        public string splitSubnet { get; set; }
        public string encryption { get; set; }
        public string owner { get; set; }
        public override string ID { get; set; }
        public override string parentID { get; set; }
        public override string externalID { get; set; }
        public string IPType { get; set; }
        public string serviceID { get; set; }
        public string associatedApplicationObjectType { get; set; }
        public string associatedApplicationObjectID { get; set; }
        public string associatedApplicationID { get; set; }
        public string gatewayMACAddress { get; set; }
        public string PATEnabled { get; set; }
        public string policyGroupID { get; set; }
        public string isPublic { get; set; }
        public string templateID { get; set; }
        public string associatedSharedNetworkResourceID { get; set; }
        public string proxyARP { get; set; }
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
            tostring += "Network      " + this.address + "\r\n";
            tostring += "Gateway      " + this.gateway + "\r\n";

            return tostring;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.name = create_params["name"];
            this.address = create_params["ip"];
            this.netmask = create_params["netmask"];
            this.gateway = create_params["gateway"];

            if (create_params.ContainsKey("description"))
                this.description = create_params["description"];

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string post_resource(string parent_id)
        {
            return "/zones/" + parent_id + "/subnets";
        }

        public string delete_resource(string id)
        {
            return "/subnets/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            return "/subnets/" + id + "?responseChoice=1";
        }

        public string get_all_resources()
        {
            return "/subnets";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/zones/" + parent_id + "/subnets";
        }

        public string get_all_resources_in_domain(string domain_id)
        {
            return "/domains/" + domain_id + "/subnets";
        }

        public string get_all_vports(string id)
        {
            return "/subnets/" + id + "/vports";
        }

        public string vm_get_resource(string id)
        {
            return "/subnets/" + id + "/vms";
        }
    }

}
