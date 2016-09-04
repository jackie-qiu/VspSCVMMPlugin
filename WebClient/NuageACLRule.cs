using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class NuageACLRule : NuageBase, NuageServerBaseClass
    {
        public string children { get; set; }
        public override string parentType { get; set; }
        public string entityScope { get; set; }
        public string lastUpdatedBy { get; set; }
        public string lastUpdatedDate { get; set; }
        public string creationDate { get; set; }
        public string reflexive { get; set; }
        public string policyState { get; set; }
        public string locationType { get; set; }
        public string networkType { get; set; }
        public string etherType { get; set; }
        public string description { get; set; }
        public string sourcePort { get; set; }
        public string destinationPort { get; set; }
        public string protocol { get; set; }
        public string priority { get; set; }
        public string action { get; set; }
        public string addressOverride { get; set; }
        public string statsLoggingEnabled { get; set; }
        public string flowLoggingEnabled { get; set; }
        public string owner { get; set; }
        public override string ID { get; set; }
        public override string parentID { get; set; }
        public override string externalID { get; set; }
        public string associatedLiveEntityID { get; set; }
        public string locationID { get; set; }
        public string networkID { get; set; }
        public string DSCP { get; set; }
        public string associatedApplicationObjectType { get; set; }
        public string associatedApplicationObjectID { get; set; }
        public string associatedApplicationID { get; set; }
        public string statsID { get; set; }

        public override string ToString()
        {
            Dictionary<string, string> PROTO_NUM_TO_NAME = new Dictionary<string, string>();
            PROTO_NUM_TO_NAME.Add("ANY", "ANY");
            PROTO_NUM_TO_NAME.Add("6", "TCP - 6");
            PROTO_NUM_TO_NAME.Add("17", "UDP - 17");
            PROTO_NUM_TO_NAME.Add("1", "ICMP - 1");

            string tostring = "";
            if (this.description == null)
                tostring += "No description given\r\n";
            else
                tostring += this.description + "\r\n";

            tostring += "Source Port: " + this.sourcePort + " to " + "Destination Port: " + this.destinationPort + " (EtherType: " + this.etherType + ".Protocol: " + PROTO_NUM_TO_NAME[this.protocol] + ")" + "\r\n"; 

            return tostring;
        }

        public string in_post_resource(string parent_id)
        {
            return "/ingressacltemplates/" + parent_id + "/ingressaclentrytemplates?responseChoice=1";
        }

        public string in_delete_resource(string id)
        {
            return "/ingressaclentrytemplates/" + id + "?responseChoice=1";
        }

        public string in_put_resource(string id)
        {
            return "/ingressaclentrytemplates/" + id + "?responseChoice=1";
        }

        public string in_get_all_resources()
        {
            return "/ingressaclentrytemplates";
        }

        public string in_get_all_resources_in_parent(string parent_id)
        {
            return "/ingressacltemplates/" + parent_id + "/ingressaclentrytemplates";
        }

        public string in_get_all_resources_in_domain(string domain_id)
        {
            return "/domains/" + domain_id + "/ingressaclentrytemplates";
        }

        public string eg_post_resource(string parent_id)
        {
            return "/egressacltemplates/" + parent_id + "/egressaclentrytemplates?responseChoice=1";
        }

        public string eg_delete_resource(string id)
        {
            return "/egressaclentrytemplates/" + id + "?responseChoice=1";
        }

        public string eg_put_resource(string id)
        {
            return "/egressaclentrytemplates/" + id + "?responseChoice=1";
        }

        public string eg_get_all_resources()
        {
            return "/egressacltemplates";
        }

        public string eg_get_all_resources_in_parent(string parent_id)
        {
            return "/egressacltemplates/" + parent_id + "/egressaclentrytemplates";
        }

        public string eg_get_all_resources_in_domain(string domain_id)
        {
            return "/domains/" + domain_id + "/egressaclentrytemplates";
        }

        public string post_data(Dictionary<string, string> create_params)
        {
            this.DSCP = create_params["DSCP"];
            this.etherType = create_params["etherType"];
            if(create_params.ContainsKey("description"))
            {
                this.description = create_params["description"];
            }

            if (create_params.ContainsKey("priority"))
            {
                this.priority = create_params["priority"];
            }

            this.etherType = create_params["etherType"];
            this.protocol = create_params["protocol"];
            if (this.protocol.Equals("6") || this.protocol.Equals("17"))
            {
                this.sourcePort = create_params["sourcePort"];
                this.destinationPort = create_params["destPort"];
            }

            this.locationType = create_params["locationType"];
            if(!this.locationType.Equals("ANY"))
                this.locationID = create_params["locationID"];

            this.networkType = create_params["networkType"];
            if (!this.networkType.Equals("ANY"))
                this.networkID = create_params["networkID"];

            this.action = create_params["action"];

            if (create_params.ContainsKey("addressOverride"))
            {
                this.addressOverride = create_params["addressOverride"];
            }

            return JsonConvert.SerializeObject(this);
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
    }
}
