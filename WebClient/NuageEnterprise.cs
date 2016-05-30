using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageEnterprise : NuageServerBaseClass
    {
        public string children { get; set; }
        public string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string avatarType { get; set; }
        public string avatarData { get; set; }
        public string floatingIPsQuota { get; set; }
        public string floatingIPsUsed { get; set; }
        public string allowTrustedForwardingClass { get; set; }
        public string allowAdvancedQOSConfiguration { get; set; }
        public string[] allowedForwardingClasses { get; set; }
        public string allowGatewayManagement { get; set; }
        public string encryptionManagementMode { get; set; }
        public string owner { get; set; }
        public string ID { get; set; }
        public string parentID { get; set; }
        public string externalID { get; set; }
        public string customerID { get; set; }
        public string DHCPLeaseInterval { get; set; }
        public string enterpriseProfileID { get; set; }
        public string receiveMultiCastListID { get; set; }
        public string sendMultiCastListID { get; set; }
        public string associatedGroupKeyEncryptionProfileID { get; set; }
        public string associatedEnterpriseSecurityID { get; set; }
        public string associatedKeyServerMonitorID { get; set; }
        public string LDAPEnabled { get; set; }
        public string LDAPAuthorizationEnabled { get; set; }
        
        public override string ToString()
        {
            return name + "\r\n";
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.name = create_params["name"];
            this.allowedForwardingClasses = new string[]{"E", "F", "G", "H"};

            if(create_params.ContainsKey("fp_quota"))
                this.floatingIPsQuota = create_params["fp_quota"];

            string data = JsonConvert.SerializeObject(this);

            return data;
        }

        public string post_resource(string parent_id)
        {
            return "/enterprises";
        }

        public string delete_resource(string id)
        {
            return "/enterprises/" + id + "?responseChoice=1";
        }

        public string put_resource(string id)
        {
            throw new NotImplementedException();
        }

        public string get_all_resources()
        {
            return "/enterprises";
        }

        public string get_all_resources_in_parent(string parent_id)
        {
            throw new NotImplementedException();
        }
    }

}
