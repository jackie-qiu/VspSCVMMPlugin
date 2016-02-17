using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public class NuageEnterprise
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
    }
}
