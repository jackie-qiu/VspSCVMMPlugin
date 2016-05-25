﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageNetworkMacroGroups : NuageServerBaseClass
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }

        public override string ToString()
        {
            return name;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.name = create_params["name"];
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
            return "/enterprises/" + parent_id + "/networkmacrogroups";
        }

        public string delete_resource(string id)
        {
            return "/networkmacrogroups/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            return "/networkmacrogroups/" + id + "?responseChoice=1";
        }

        public string get_all_resources()
        {
            throw new NotImplementedException();
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/enterprises/" + parent_id + "/networkmacrogroups";
        }

        public string get_network_macro_list(string id)
        {
            return "/networkmacrogroups/" + id + "/enterprisenetworks";
        }

        public string put_network_macro_resource(string id)
        {
            return "/networkmacrogroups/" + id + "/enterprisenetworks";
        }
    }
}
