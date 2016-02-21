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
        private int FAILED = -1;
        private int SUCCESS = 0;
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

        public int LoginVSD()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/me";
            string authKey = nuageBase64.Base64Encode(this.username + ":" + this.password);

            List<NuageMe[]> result = this.CallRestAPI<NuageMe[]>(url, authKey, false);
            if (result.Count() == 0)
            {
                logger.Error("Login VSD Failed....");
                return FAILED;
            }

            this.mes = result.First();
            this.token = nuageBase64.Base64Encode(username + ":" + mes.First().APIKey);
            logger.DebugFormat("Token: {0}", this.token);
            return SUCCESS;

        }

        public int GetDomains()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/domains";

            List<NuageDomainPS> result = this.CallRestAPI<NuageDomainPS>(url,this.token, true);
            if (result.Count() == 0)
            {
                logger.Error("Get Domains Failed....");
                return FAILED;
            }

            this.domains = result.First();
            return SUCCESS;

            
        }

        public int GetEnterrpise()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/enterprises";

            List<NuageEnterprisePS> result = this.CallRestAPI<NuageEnterprisePS>(url, this.token, true);
            if (result.Count() == 0)
            {
                logger.Error("Get Enterrpise Failed....");
                return FAILED;
            }

            this.enterprise = result.First();
            return SUCCESS;
        }

        public int GetPolicyGroup()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/policygroups";

            List<NuagePolicyGroupPS> result = this.CallRestAPI<NuagePolicyGroupPS>(url, this.token, true);
            if (result.Count() == 0)
            {
                logger.Error("Get PolicyGroup Failed....");
                return FAILED;
            }

            this.policyGroups = result.First();
            return SUCCESS;
            
        }

        public int GetRedirectionTarget()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/redirectiontargets";

            List<NuageRedirectionTargetPS> result = this.CallRestAPI<NuageRedirectionTargetPS>(url, this.token, true);
            if (result.Count() == 0)
            {
                logger.Error("Get RedirectionTarget Failed....");
                return FAILED;
                
            }

            this.redirectionTargets = result.First();
            return SUCCESS;
           
        }

        public int GetSubnet()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/subnets";

            List<NuageSubnetPS> result = this.CallRestAPI<NuageSubnetPS>(url, this.token, true);
            if (result.Count() == 0)
            {
                logger.Error("Get Subnet Failed....");
                return FAILED;
                
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
            return SUCCESS;
        }

        public int GetZone()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/zones";

            List<NuageZonePS> result = this.CallRestAPI<NuageZonePS>(url, this.token, true);
            if (result.Count() == 0)
            {
                logger.Error("Get Zone Failed....");
                return FAILED;
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
            return SUCCESS;

        }

        private List<T> CallRestAPI<T>(string url, string token, bool tojson)
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

                $result=Invoke-RestMethod -Method Get -Uri URL -Headers $headers TOJSON
                return $result";

            string RestScriptFormatted = RestScript.Replace("URL", url);
            RestScriptFormatted = RestScriptFormatted.Replace("ORGANIZATION", this.organization);
            RestScriptFormatted = RestScriptFormatted.Replace("TOKEN", token);
            if (tojson)
            {
                RestScriptFormatted = RestScriptFormatted.Replace("TOJSON", "| ConvertTo-Json");
            }
            else
            {
                RestScriptFormatted = RestScriptFormatted.Replace("TOJSON", "");
            }

            PowerShell ps = PowerShell.Create();
            ps.AddScript(RestScriptFormatted);
            try
            {
                Collection<PSObject> results = ps.Invoke<PSObject>();
                foreach (var psObject in results)
                {
                    //Console.WriteLine("{0}", psObject.Properties["ID"].Value.ToString());
                    //Console.WriteLine("{0}", psObject.ToString());
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
