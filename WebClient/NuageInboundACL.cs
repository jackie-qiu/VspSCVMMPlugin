using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageInboundACL : NuageServerBaseClass
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string policyState { get; set; }
        public string priority { get; set; }
        public string priorityType { get; set; }
        public string assocAclTemplateId { get; set; }
        public bool defaultAllowIP { get; set; }
        public bool defaultAllowNonIP { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool active { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string associatedLiveEntityID { get; set; }
        public bool allowL2AddressSpoof { get; set; }


        public override string ToString()
        {
            string tostring = "";
            tostring += name + "\r\n";
            if (description == null)
                tostring += "No description given" + "\r\n";
            else
                tostring += this.description + "\r\n";

            if (defaultAllowIP)
                tostring += "Allow IP Traffic by Default\r\n";
            else
                tostring += "Drop IP Traffic by Default\r\n";

            if (defaultAllowNonIP)
                tostring += "Allow non IP Traffic by Default\r\n";
            else
                tostring += "Drop non IP Traffic by Default\r\n";

            return tostring;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.active = true;
            this.defaultAllowNonIP = false;
            this.defaultAllowIP = false;
            this.allowL2AddressSpoof = false;

            if (create_params.ContainsKey("name"))
            {
                this.name = create_params["name"];
                
            }
            if (create_params.ContainsKey("description"))
                this.description = create_params["description"];

            if (create_params.ContainsKey("priority"))
            {
                this.priority = create_params["priority"];
            }

            if (create_params.ContainsKey("addressSpoof"))
            {
                this.allowL2AddressSpoof = true;
            }

            if (create_params.ContainsKey("allowIp"))
            {
                this.defaultAllowIP = true;
            }

            if (create_params.ContainsKey("allowNonIp"))
            {
                this.defaultAllowNonIP = true;
            }

            return JsonConvert.SerializeObject(this);
        }

        public string post_resource(string parent_id)
        {
            return "/domains/" + parent_id + "/ingressacltemplates";
        }

        public string delete_resource(string id)
        {
            return "/ingressacltemplates/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            return "/ingressacltemplates/" + id + "?responseChoice=1";
        }

        public string get_all_resources()
        {
            return "/ingressacltemplates";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/domains/" + parent_id + "/ingressacltemplates";
        }
    }
}
