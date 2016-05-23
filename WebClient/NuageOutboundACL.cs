using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageOutboundACL : NuageServerBaseClass
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public bool defaultInstallACLImplicitRules { get; set; }
        public string policyState { get; set; }
        public string priority { get; set; }
        public string priorityType { get; set; }
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

        public override string ToString()
        {
            return name;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.name = create_params["name"];
            this.description = create_params["description"];
            this.active = true;
            this.defaultAllowNonIP = false;
            this.defaultAllowIP = false;
            this.defaultInstallACLImplicitRules = false;

            if (create_params.ContainsKey("priority"))
            {
                this.priority = create_params["priority"];
            }

            if (create_params.ContainsKey("installImplicitRules"))
            {
                this.defaultInstallACLImplicitRules = true;
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
            return "/domains/" + parent_id + "/egressacltemplates";
        }

        public string delete_resource(string id)
        {
            return "/egressacltemplates/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            return "/egressacltemplates/" + id + "?responseChoice=1";
        }

        public string get_all_resources()
        {
            return "/egressacltemplates";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/domains/" + parent_id + "/egressacltemplates";
        }
    }
}
