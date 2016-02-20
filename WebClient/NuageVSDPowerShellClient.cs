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

        public void LoginVSD()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/me";
            string authKey = nuageBase64.Base64Encode(this.username + ":" + this.password);

            List<NuageMe[]> result = this.CallRestAPI<NuageMe[]>(url, authKey, false);
            this.mes = result.First();

            this.token = nuageBase64.Base64Encode(username + ":" + mes.First().APIKey);
            logger.DebugFormat("Token: {0}", this.token);
        }

        public void GetDomains()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/domains";

            List<NuageDomainPS> result = this.CallRestAPI<NuageDomainPS>(url,this.token, true);
            this.domains = result.First();
        }

        public void GetEnterrpise()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/enterprises";

            List<NuageEnterprisePS> result = this.CallRestAPI<NuageEnterprisePS>(url, this.token, true);
            this.enterprise = result.First();
        }

        public void GetPolicyGroup()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/policygroups";

            List<NuagePolicyGroupPS> result = this.CallRestAPI<NuagePolicyGroupPS>(url, this.token, true);
            this.policyGroups = result.First();
        }

        public void GetRedirectionTarget()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/redirectiontargets";

            List<NuageRedirectionTargetPS> result = this.CallRestAPI<NuageRedirectionTargetPS>(url, this.token, true);
            this.redirectionTargets = result.First();
        }

        public void GetSubnet()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/subnets";

            List<NuageSubnetPS> result = this.CallRestAPI<NuageSubnetPS>(url, this.token, true);
            this.subnets = result.First();
        }

        public void GetZone()
        {
            string url = this.baseUrl.ToString() + "nuage/api/v3_2/zones";

            List<NuageZonePS> result = this.CallRestAPI<NuageZonePS>(url, this.token, true);
            this.zones = result.First();
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
            Collection<PSObject> results = ps.Invoke<PSObject>();

            foreach (var psObject in results)
            {
                //Console.WriteLine("{0}", psObject.Properties["ID"].Value.ToString());
                //Console.WriteLine("{0}", psObject.ToString());
                T result = JsonConvert.DeserializeObject<T>(psObject.ToString());
                NuageObject.Add(result);
            }

            return NuageObject;
        }

    }
}
