using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public class NuageSharedNetworkResource : NuageBase, NuageServerBaseClass
    {
        public string children { get; set; }
        public override string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public string netmask { get; set; }
        public string gateway { get; set; }
        public string type { get; set; }
        public string uplinkVPortName { get; set; }
        public string uplinkInterfaceIP { get; set; }
        public string uplinkInterfaceMAC { get; set; }
        public string underlay { get; set; }
        public string backHaulRouteDistinguisher { get; set; }
        public string backHaulRouteTarget { get; set; }
        public string backHaulVNID { get; set; }
        public string vnID { get; set; }
        public string permittedActionType { get; set; }
        public string accessRestrictionEnabled { get; set; }
        public string owner { get; set; }
        public override string ID { get; set; }
        public override string parentID { get; set; }
        public override string externalID { get; set; }
        public string domainRouteDistinguisher { get; set; }
        public string domainRouteTarget { get; set; }
        public string DHCPManaged { get; set; }
        public string sharedResourceParentID { get; set; }
        public string uplinkGWVlanAttachmentID { get; set; }
        public string ECMPCount { get; set; }

        public override string ToString()
        {
            string tostring = "";
            tostring += this.name + "\r\n";
            if (this.description == null)
                tostring += "No description given" + "\r\n";
            else
                tostring += this.description + "\r\n";
            tostring += "Nework    " + this.address + "/" + this.netmask + "\r\n";

            if(this.gateway != null)
                tostring += "Gateway   " + this.gateway + "\r\n";
            else
                tostring += "Gateway   " + "Not Applicable" + "\r\n";

            return tostring;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            throw new NotImplementedException();
        }

        public string post_resource(string parent_id)
        {
            throw new NotImplementedException();
        }

        public string delete_resource(string id)
        {
            throw new NotImplementedException();
        }

        public string put_resource(string id)
        {
            throw new NotImplementedException();
        }

        public string get_all_resources()
        {
            throw new NotImplementedException();
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            throw new NotImplementedException();
        }

        public string get_all_resources_in_enterprise(string ent_id)
        {
            return "/enterprises/" + ent_id + "/sharednetworkresources";
        }
    }
}
