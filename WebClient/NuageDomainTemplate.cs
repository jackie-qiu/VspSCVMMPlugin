﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageDomainTemplate : NuageServerBaseClass
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string policyChangeStatus { get; set; }
        public string encryption { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string multicast { get; set; }
        public string associatedMulticastChannelMapID { get; set; }


        public override string ToString()
        {
            return name;
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.name = create_params["name"];
            //this.encryption = "DISABLED";
            //this.multicast = "DISABLED";

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string post_resource(string parent_id)
        {
            return "/enterprises/" + parent_id + "/domaintemplates";
        }

        public string delete_resource(string id)
        {
            return "/domaintemplates/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            return "/domaintemplates/" + id + "?responseChoice=1";
        }

        public string get_all_resources()
        {
            return "/domaintemplates";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            return "/enterprises/" + parent_id + "/domaintemplates";
        }

    }
}