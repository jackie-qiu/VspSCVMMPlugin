using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageVport : NuageServerBaseClass
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string systemType { get; set; }
        public string addressSpoofing { get; set; }
        public string active { get; set; }
        public string operationalState { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string domainID { get; set; }
        public string zoneID { get; set; }
        public string multiNICVPortID { get; set; }
        public string VLANID { get; set; }
        public string associatedFloatingIPID { get; set; }
        public string hasAttachedInterfaces { get; set; }
        public string multicast { get; set; }
        public string associatedMulticastChannelMapID { get; set; }
        public string associatedSendMulticastChannelMapID { get; set; }

        public override string ToString()
        {
            string tostring = "";
            tostring += this.name + "\r\n";
            if(this.description == null)
                tostring += "No description given" + "\r\n";
            else 
                tostring += this.description + "\r\n";
            tostring += "Type      " + this.type + "\r\n";
            tostring += "Spoofing  " + this.addressSpoofing + "\r\n";

            return tostring;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.type = "VM";
            this.name = create_params["name"];
            this.description = create_params["description"];
            this.addressSpoofing = "INHERITED";

            if (create_params.ContainsKey("floatingip"))
            {
                this.associatedFloatingIPID = create_params["floatingip"];
            }

            string data = JsonConvert.SerializeObject(this);

            return data;

        }

        public string post_resource(string parent_id)
        {
            return "/subnets/" + parent_id + "/vports";
        }

        public string delete_resource(string id)
        {
            return "/vports/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            return "/vports/" + id + "?responseChoice=1";
        }

        public string get_all_resources()
        {
            throw new NotImplementedException();
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/subnets/" + parent_id + "/vports";
        }

        public string get_all_resources_in_domain(string domain_Id)
        {
            return "/domains/" + domain_Id + "/vports";
        }

        public string get_vports_for_vptag(string vptag_id)
        {
            return "/policygroups/" + vptag_id + "/vports";
        }

        public string get_vport_for_fip(string fip_id)
        {
            return "/floatingips/" + fip_id + "/vports";
        }

        public string fip_update_data(string fip_id)
        {
            this.associatedFloatingIPID = fip_id;

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

    }


}
