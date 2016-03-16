using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Management.Automation;
using System.Collections.ObjectModel;
using log4net;
using log4net.Config;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{

    public class NuageVSDPowerShellSession
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(nuageVSDSession));
        public int FAILED = -1;
        public int SUCCESS = 0;
        private string username { get; set; }
        private string password { get; set; }
        private string organization { get; set; }
        private string token { get; set; }
        private Uri baseUrl { get; set; }
        public NuageMe[] mes { get; set; }
        public NuageEnterprisePS enterprise { get; set; }
        public NuageDomainPS domains { get; set; }
        public NuageZonePS zones { get; set; }
        public NuagePolicyGroupPS policyGroups { get; set; }
        public NuageRedirectionTargetPS redirectionTargets { get; set; }
        public NuageSubnetPS subnets { get; set; }
        
        public NuageVSDPowerShellSession(string username, string password, string organization, Uri baseUrl)
        {
            FileInfo config = new FileInfo(".\\log.conf");
            XmlConfigurator.Configure(config);

            this.username = username;
            this.password = password;
            this.organization = organization;
            this.baseUrl = baseUrl;

        }

        public Boolean LoginVSD()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/me";
            string authKey = nuageBase64.Base64Encode(this.username + ":" + this.password);

            List<NuageMe[]> result = this.CallRestGetAPI<NuageMe[]>(url, authKey, null, false);
            if (result.Count() == 0)
            {
                logger.Error("Login VSD Failed....");
                return false;
            }

            this.mes = result.First();
            this.token = nuageBase64.Base64Encode(username + ":" + mes.First().APIKey);
            logger.DebugFormat("Token: {0}", this.token);
            return true;

        }
        public List<NuageDomain> GetDomains()
        {
            if (this.domains != null)
                return this.domains.Value;
            return null;
        }
        public Boolean GetDomainsRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/domains";

            List<NuageDomainPS> result = this.CallRestGetAPI<NuageDomainPS>(url,this.token, null, true);
            if (result.Count() == 0)
            {
                logger.Error("Get Domains Failed....");
                return false;
            }

            this.domains = result.First();
            return true;

            
        }
        public List<NuageEnterprise> GetEnterrpise()
        {
            if (this.enterprise != null)
                return this.enterprise.Value;
            return null;
        }
        public Boolean GetEnterrpiseRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/enterprises";

            List<NuageEnterprisePS> result = this.CallRestGetAPI<NuageEnterprisePS>(url, this.token, null, true);
            if (result.Count() == 0)
            {
                logger.Error("Get Enterrpise Failed....");
                return false;
            }

            this.enterprise = result.First();
            return true;
        }
        public List<NuagePolicyGroup> GetPolicyGroups()
        {
            if (this.policyGroups != null)
                return this.policyGroups.Value;
            return null;
        }
        public Boolean GetPolicyGroupRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/policygroups";

            List<NuagePolicyGroupPS> result = this.CallRestGetAPI<NuagePolicyGroupPS>(url, this.token, null, true);
            if (result.Count() == 0)
            {
                logger.Error("Get PolicyGroup Failed....");
                return false;
            }

            this.policyGroups = result.First();
            return true;
            
        }
        public List<NuageRedirectionTarget> GetRedirectionTargets()
        {
            if (this.redirectionTargets != null)
                return this.redirectionTargets.Value;
            return null;
        }
        public Boolean GetRedirectionTargetRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/redirectiontargets";

            List<NuageRedirectionTargetPS> result = this.CallRestGetAPI<NuageRedirectionTargetPS>(url, this.token, null, true);
            if (result.Count() == 0)
            {
                logger.Error("Get RedirectionTarget Failed....");
                return false;
                
            }

            this.redirectionTargets = result.First();
            return true;
           
        }
        public List<NuageSubnet> GetSubnets()
        {
            if (this.redirectionTargets != null)
                return this.subnets.Value;
            return null;
        }
        public Boolean GetSubnetRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/subnets";

            List<NuageSubnetPS> result = this.CallRestGetAPI<NuageSubnetPS>(url, this.token, null, true);
            if (result.Count() == 0)
            {
                logger.Error("Get Subnet Failed....");
                return false;
                
            }

            this.subnets = result.First();

            for ( int i = this.subnets.Value.Count -1; i >=0; i -- )
            {
                NuageSubnet item = this.subnets.Value[i];
                //Remove unused BackHaulSubnet
                if (item.name.Equals("BackHaulSubnet"))
                {
                    this.subnets.Value.Remove(item);
                }
            }
            return true;
        }
        public List<NuageZone> GetZones()
        {
            if (this.redirectionTargets != null)
                return this.zones.Value;
            return null;
        }
        public Boolean GetZoneRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/zones";

            List<NuageZonePS> result = this.CallRestGetAPI<NuageZonePS>(url, this.token, null, true);
            if (result.Count() == 0)
            {
                logger.Error("Get Zone Failed....");
                return false;
            }
            this.zones = result.First();
            for (int i = this.zones.Value.Count - 1; i >= 0; i--)
            {
                //Remove unused BackHaulSubnet
                NuageZone item = this.zones.Value[i];
                if (item.name.Equals("BackHaulZone") || item.name.Equals("Shared Zone template"))
                {
                    this.zones.Value.Remove(item);
                }
            }
            return true;

        }

        public NuageVport CreateVport(string subnetId, string name)
        {
            NuageVport request = new NuageVport();

            request.type = "VM";
            request.name = name;
            request.description = name;
            request.addressSpoofing = "INHERITED";

            string request_json = JsonConvert.SerializeObject(request);

            logger.DebugFormat("Create vPort with content: {0}", request_json);

            string url = this.baseUrl.ToString() + "nuage/api/v3_2/subnets/" + subnetId + "/vports";

            List<NuageVportPS> result = this.CallRestPostAPI<NuageVportPS>(url, this.token, request_json, true);
            if (result != null && result.Count > 0)
                return result.First().Value.First();

            return null;
            
        }

        public NuageVms CreateVirtualMachine(List<NuageVmInterface> vmNics, string UUID, string name)
        {
            NuageVms request = new NuageVms();

            request.UUID = UUID;
            request.name = name;
            request.interfaces = vmNics;

            string request_json = JsonConvert.SerializeObject(request);

            logger.DebugFormat("Create virtual machine with content: {0}", request_json);

            string url = this.baseUrl.ToString() + "nuage/api/v3_2/vms";

            List<NuageVmsPS> result = this.CallRestPostAPI<NuageVmsPS>(url, this.token, request_json, false);
            if (result != null && result.Count > 0)
                return result.First().Value.First();

            return null;

        }

        private List<T> CallRestGetAPI<T>(string url, string token, string content, bool tojson)
        {
            return CallRestAPI<T>(url, token, "Get", content, tojson);
        }

        private List<T> CallRestPostAPI<T>(string url, string token, string content, bool tojson)
        {
            return CallRestAPI<T>(url, token, "Post", content, tojson);
        }

        private List<T> CallRestAPI<T>(string url, string token, string action, string content, bool tojson)
        {
           
            List<T> NuageObject = new List<T>();
            string RestScript = @"
                $CertificatePolicyMethod= @'
                    using System.Net;
                    using System.Security.Cryptography.X509Certificates;
                    public class TrustAllCertsPolicy : ICertificatePolicy {
                        public bool CheckValidationResult(
                            ServicePoint srvPoint, X509Certificate certificate,
                            WebRequest request, int certificateProblem) {
                            return true;
                        }
                }
'@


                Add-Type $CertificatePolicyMethod
                [System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy

                $headers = New-Object 'System.Collections.Generic.Dictionary[[String],[String]]'
                $headers.Add('Content-Type','application/json')
                $headers.Add('X-Nuage-Organization','ORGANIZATION')
                $headers.Add('X-Requested-With','XMLHttpRequest')
                $headers.Add('Authorization', 'XREST TOKEN')

                $body=@'
                BODY
'@
                $post = 'ACTION'
                if ($post -ne 'Post')
                {
                    $result=Invoke-RestMethod -Method ACTION -ContentType application/json -Uri URL -Headers $headers TOJSON
                }
                Else
                {
                    $result=Invoke-RestMethod -Method ACTION -ContentType application/json -Uri URL -Headers $headers -Body $body TOJSON
                }

                
                return $result";

            string RestScriptFormatted = RestScript.Replace("URL", url);
            RestScriptFormatted = RestScriptFormatted.Replace("ORGANIZATION", this.organization);
            RestScriptFormatted = RestScriptFormatted.Replace("TOKEN", token);
            RestScriptFormatted = RestScriptFormatted.Replace("ACTION", action);
            if (tojson)
            {
                RestScriptFormatted = RestScriptFormatted.Replace("TOJSON", "| ConvertTo-Json");
            }
            else
            {
                RestScriptFormatted = RestScriptFormatted.Replace("TOJSON", "");
            }

            //Replace json body if the operation is POST
            if (action.Equals("Post"))
            {
                RestScriptFormatted = RestScriptFormatted.Replace("BODY", content);
            }
            else
            {
                RestScriptFormatted = RestScriptFormatted.Replace("BODY", "");
            }
            logger.DebugFormat("The rest powershell scripts to excuete {0}", RestScriptFormatted);
            PowerShell ps = PowerShell.Create();
            ps.AddScript(RestScriptFormatted);
            try
            {
                Collection<PSObject> results = ps.Invoke<PSObject>();
                foreach (var psObject in results)
                {
                    //Console.WriteLine("{0}", psObject.Properties["ID"].Value.ToString());
                    //Console.WriteLine("{0}", psObject.ToString());
                    if (psObject == null)
                        continue;
                    T result = JsonConvert.DeserializeObject<T>(psObject.ToString());
                    NuageObject.Add(result);
                }
            }
            catch(Exception ex)
            {
                logger.ErrorFormat("Invoke Powershell rest url {0} failed! {1}",url, ex.Message);
            }

            return NuageObject;
        }

    }
}
