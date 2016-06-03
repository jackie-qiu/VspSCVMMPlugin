using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageRedirectionTarget : NuageBase, NuageServerBaseClass
    {
        public string children { get; set; }
        public override string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string virtualNetworkID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string endPointType { get; set; }
        public string redundancyEnabled { get; set; }
        public string triggerType { get; set; }
        public string owner { get; set; }
        public override string ID { get; set; }
        public override string parentID { get; set; }
        public override string externalID { get; set; }
        public string ESI { get; set; }
        public string templateID { get; set; }

        public override string ToString()
        {
            return name;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.name = create_params["name"];
            this.redundancyEnabled = create_params["redundancy_enabled"];
            this.description = create_params["description"];
            this.endPointType = "L3";
            if (create_params.ContainsKey("insertion_mode"))
            {
                this.endPointType = create_params["insertion_mode"];
            }

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string post_resource(string parent_id)
        {
            return "/domains/" + parent_id + "/redirectiontargets";
        }

        public string delete_resource(string id)
        {
            return "/redirectiontargets/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            throw new NotImplementedException();
        }

        public string get_all_resources()
        {
            return "/redirectiontargets";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/domains/" + parent_id + "/redirectiontargets";
        }

        public string get_vport_redirect_target(string vport_id)
        {
            return "/vports/" + vport_id + "/redirectiontargets";
        }

    }


}
