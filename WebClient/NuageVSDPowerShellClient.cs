using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Reflection;
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
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuageVSDPowerShellSession));
        public int FAILED = -1;
        public int SUCCESS = 0;
        private string username { get; set; }
        private string password { get; set; }
        private string organization { get; set; }
        private string version { get; set; }
        private string token { get; set; }
        private Uri baseUrl { get; set; }
        private List<NuageMe> mes { get; set; }
        private List<NuageEnterprise> enterprise { get; set; }
        private List<NuageDomain> domains { get; set; }
        private List<NuageZone> zones { get; set; }
        private List<NuagePolicyGroup> policyGroups { get; set; }
        private List<NuageRedirectionTarget> redirectionTargets { get; set; }
        private List<NuageSubnet> subnets { get; set; }
        private List<NuageFloatingIP> floatingIPs { get; set; }
        private PowerShell powerShellContext;
        private NuagePowerShell nuagePS;
        
        public NuageVSDPowerShellSession(string username, string password, string organization, Uri baseUrl, string version)
        {
            string addinPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                FileInfo configFile = new FileInfo(System.IO.Path.Combine(addinPath + "\\log.conf"));
                XmlConfigurator.Configure(configFile);
            }
            catch (FileNotFoundException ex)
            {
                logger.InfoFormat("Log4net config file not found {0}", ex.Message);
            }

            powerShellContext = PowerShell.Create();

            this.username = username;
            this.password = password;
            this.organization = organization;
            this.baseUrl = baseUrl;
            this.version = version;

            nuagePS = new NuagePowerShell();



        }

        private Boolean AddCertificatePolicyType()
        {
            string restScripts = @"
               Add-Type @'
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

                [System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy";
            
            powerShellContext.AddScript(restScripts);
            try
            {
                powerShellContext.Invoke<PSObject>();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Add poershell certificate policy type failed {0}", ex.Message);
                return false;
            }

            return true;
        }

        public Boolean LoginVSD()
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/me";
            string authKey = nuageBase64.Base64Encode(this.username + ":" + this.password);

            List<NuageMe> result = this.CallRestGetAPI<NuageMe>(url, authKey, null);
            if (result == null || result.Count() == 0)
            {
                logger.Error("Login VSD Failed....");
                return false;
            }

            this.mes = result;
            this.token = nuageBase64.Base64Encode(username + ":" + mes.First().APIKey);
            logger.DebugFormat("Token: {0}", this.token);
            return true;

        }
        public List<NuageDomain> GetDomains()
        {
            if (this.domains != null)
                return this.domains;
            return null;
        }
        public Boolean GetDomainsRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/domains";

            List<NuageDomain> result = this.CallRestGetAPI<NuageDomain>(url, this.token, null);
            if (result == null || result.Count() == 0)
            {
                logger.Error("Get Domains Failed....");
                return false;
            }

            this.domains = result;
            return true;

            
        }
        public List<NuageEnterprise> GetEnterrpise()
        {
            if (this.enterprise != null)
                return this.enterprise;
            return null;
        }
        public Boolean GetEnterrpiseRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/enterprises";

            List<NuageEnterprise> result = this.CallRestGetAPI<NuageEnterprise>(url, this.token, null);
            if (result == null || result.Count() == 0)
            {
                logger.Error("Get Enterrpise Failed....");
                return false;
            }

            this.enterprise = result;
            return true;
        }
        public List<NuagePolicyGroup> GetPolicyGroups()
        {
            if (this.policyGroups != null)
                return this.policyGroups;
            return null;
        }
        public Boolean GetPolicyGroupRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/policygroups";

            List<NuagePolicyGroup> result = this.CallRestGetAPI<NuagePolicyGroup>(url, this.token, null);
            if (result == null || result.Count() == 0)
            {
                logger.Error("There is no policy group.");
                return false;
            }

            this.policyGroups = result;
            return true;
            
        }

        public List<NuageFloatingIP> GetFloatingIPs()
        {
            if (this.floatingIPs != null)
                return this.floatingIPs;
            return null;
        }
        public Boolean GetFloatingIPsRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/floatingips";

            List<NuageFloatingIP> result = this.CallRestGetAPI<NuageFloatingIP>(url, this.token, null);
            if (result == null || result.Count() == 0)
            {
                logger.Debug("There is no floating ip.");
                return false;
            }

            this.floatingIPs = new List<NuageFloatingIP>();
            foreach (NuageFloatingIP item in result)
            {
                if (item.assigned.Equals("false")) 
                    this.floatingIPs.Add(item);
            }
            return true;

        }
        public List<NuageRedirectionTarget> GetRedirectionTargets()
        {
            if (this.redirectionTargets != null)
                return this.redirectionTargets;
            return null;
        }
        public Boolean GetRedirectionTargetRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/redirectiontargets";

            List<NuageRedirectionTarget> result = this.CallRestGetAPI<NuageRedirectionTarget>(url, this.token, null);
            if (result == null || result.Count() == 0)
            {
                logger.Debug("There is no redirection target");
                return false;
                
            }

            this.redirectionTargets = result;
            return true;
           
        }
        public List<NuageSubnet> GetSubnets()
        {
            if (this.subnets != null)
                return this.subnets;
            return null;
        }
        public Boolean GetSubnetRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/subnets";

            List<NuageSubnet> result = this.CallRestGetAPI<NuageSubnet>(url, this.token, null);
            if (result == null || result.Count() == 0)
            {
                logger.Error("Get Subnet Failed....");
                return false;
                
            }

            this.subnets = new List<NuageSubnet>();

            foreach (NuageSubnet item in result )
            {
                if (!(item.name.Equals("BackHaulSubnet")))
                {
                    this.subnets.Add(item);
                }
            }
            return true;
        }
        public List<NuageZone> GetZones()
        {
            if (this.zones != null)
                return this.zones;
            return null;
        }
        public Boolean GetZoneRestApi()
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/zones";

            List<NuageZone> result = this.CallRestGetAPI<NuageZone>(url, this.token, null);
            if (result == null || result.Count() == 0)
            {
                logger.Error("Get Zone Failed....");
                return false;
            }

            this.zones = new List<NuageZone>();

            foreach (NuageZone item in result)
            {
                //Remove unused BackHaulSubnet
                if (!(item.name.Equals("BackHaulZone") || item.name.Equals("Shared Zone template")))
                {
                    this.zones.Add(item);
                }
            }
            return true;

        }

        public NuageVport CreateVport(string subnetId, string name, string floatingipID)
        {
            NuageVport request = new NuageVport();

            request.type = "VM";
            request.name = name;
            request.description = name;
            request.addressSpoofing = "INHERITED";
            if (!String.IsNullOrEmpty(floatingipID))
                request.associatedFloatingIPID = floatingipID;

            string request_json = JsonConvert.SerializeObject(request);

            logger.DebugFormat("Create vPort with content: {0}", request_json);

            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/subnets/" + subnetId + "/vports";

            List<NuageVport> result = this.CallRestPostAPI<NuageVport>(url, this.token, request_json, true);
            if (result != null && result.Count > 0)
                return result.First();

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

            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/vms";

            List<NuageVms> result = this.CallRestPostAPI<NuageVms>(url, this.token, request_json, false);
            if (result != null && result.Count > 0)
                return result.First();

            return null;

        }

        public List<string> GetVportInPolicyGroup(string policyGroupID)
        {
            List<string> vportIds = new List<string>();
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/policygroups/" + policyGroupID + "/vports";
            logger.Info(string.Format("Get vport in policy group ID '{0}'", policyGroupID));

            List<NuageVport> result = this.CallRestGetAPI<NuageVport>(url, this.token, null);
            if (result != null && result.Count > 0)
            {
                foreach (NuageVport port in result)
                    vportIds.Add(port.ID);
            }

            return vportIds;

        }

        public NuagePolicyGroup GetPolicyGroupInVMInterface(string vmInterfaceID)
        {
            List<string> vportIds = new List<string>();
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/vminterfaces/" + vmInterfaceID + "/policygroups";
            logger.Info(string.Format("Get policy group vm interface ID '{0}'", vmInterfaceID));

            List<NuagePolicyGroup> result = this.CallRestGetAPI<NuagePolicyGroup>(url, this.token, null);
            if (result != null && result.Count > 0)
            {
                return result.First<NuagePolicyGroup>();
            }

            return null;

        }

        public Boolean UpdateVportInPolicyGroup(string policyGroupID, List<string> vportIds)
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/policygroups/" + policyGroupID + "/vports";
            logger.Info(string.Format("Put {0} vports into policy group ID '{1}'",vportIds.Count(), policyGroupID));

            string request_json = JsonConvert.SerializeObject(vportIds);

            logger.DebugFormat("Put vPort into policy group with content: {0}", request_json);

            return this.CallRestPutAPI<NuageVport>(url, this.token, request_json);


        }

        public NuageVms GetVirtualMachineByUUID(string UUID)
        {

            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/vms";
            string filter = string.Format("UUID IS '{0}'",UUID);

            List<NuageVms> result = this.CallRestGetAPI<NuageVms>(url, this.token, filter);
            if (result != null && result.Count > 0)
                return result.First();

            return null;

        }

        public Boolean DeleteVirtualMachine(NuageVms vm)
        {

            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/vms/" + vm.ID + "?responseChoice=1";

            return this.CallRestDeleteAPI<NuageVms>(url, this.token);

        }

        public Boolean DeleteVPort(string vPortIDs)
        {

            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/vports/" + vPortIDs + "?responseChoice=1";

            return this.CallRestDeleteAPI<NuageVms>(url, this.token);

        }

        private List<T> CallRestGetAPI<T>(string url, string token, string filter)
        {
            return CallRestAPI<T>(url, token, "Get", null, filter);
        }

        private Boolean CallRestPutAPI<T>(string url, string token, string content)
        {
            CallRestAPI<T>(url, token, "Put", content, null);
            return true;
        }

        private List<T> CallRestPostAPI<T>(string url, string token, string content, bool tojson)
        {
            return CallRestAPI<T>(url, token, "Post", content, null);
        }

        private Boolean CallRestDeleteAPI<T>(string url, string token)
        {
            CallRestAPI<T>(url, token, "Delete", null, null);
            return true;
        }

        private List<T> CallRestAPI<T>(string url, string token, string action, string content, string filter)
        {
           
            List<T> NuageObject = new List<T>();

            if (!AddCertificatePolicyType())
                return null;
            
            string RestScript = @"
                $headers = New-Object 'System.Collections.Generic.Dictionary[[String],[String]]'
                $headers.Add('Content-Type','application/json')
                $headers.Add('X-Nuage-Organization','ORGANIZATION')
                $headers.Add('X-Requested-With','XMLHttpRequest')
                $headers.Add('Authorization', 'XREST TOKEN')
                $headers.Add('X-Nuage-Filter',""FILTER"")
                #$headers.Add('X-Nuage-ProxyUser', '')
                $body=@'
                BODY
'@
                $request = 'ACTION'
                $result = ''
                Try 
                {
                    if ($request -eq 'Get')
                    {
                        $result=(Invoke-WebRequest -Method Get -ContentType application/json -Uri URL -TimeoutSec 10 -Headers $headers).content
                    }
                    ElseIf ($request -eq 'Post')
                    {
                        $result=(Invoke-WebRequest -Method Post -ContentType application/json -Uri URL -TimeoutSec 10 -Headers $headers -Body $body).content
                    }
                    ElseIf ($request -eq 'Put')
                    {
                        $result=(Invoke-WebRequest -Method Put -ContentType application/json -Uri URL -TimeoutSec 10 -Headers $headers -Body $body).content
                    }
                    ElseIf ($request -eq 'Delete')
                    {
                        #When issuing more than two Invoke-RestMethod commands with the PUT or DELETE method against the same server, 
                        #the third one fails with error: “Invoke-RestMethod : The operation has timed out.” 
                        #$result=Invoke-RestMethod -Method Delete -ContentType application/json -Uri URL -TimeoutSec 10 -Headers $headers
                        $result=(Invoke-WebRequest -Uri URL -Method DELETE -Headers $headers).content
                    }
                    Else
                    {
                        Write-Error ""Unknown_Request""
                    }

                }
                Catch [System.Net.WebException]
                {
                    Write-Warning $_.Exception.Status
                    Write-Warning $_.Exception.Response.StatusCode.Value__
                    Write-Warning $_.Exception.Response.StatusDescription
                }
                Catch
                {
                    Write-Error $_.Exception.Message
                }

                return $result";

            string RestScriptFormatted = RestScript.Replace("URL", url);
            RestScriptFormatted = RestScriptFormatted.Replace("ORGANIZATION", this.organization);
            RestScriptFormatted = RestScriptFormatted.Replace("TOKEN", token);
            RestScriptFormatted = RestScriptFormatted.Replace("ACTION", action);
            if (filter != null)
            {
                RestScriptFormatted = RestScriptFormatted.Replace("FILTER", filter);
            }
            else
            {
                RestScriptFormatted = RestScriptFormatted.Replace("FILTER", "");
            }

            //Replace json body if the operation is POST
            if (action.Equals("Post") || action.Equals("Put"))
            {
                RestScriptFormatted = RestScriptFormatted.Replace("BODY", content);
            }
            else
            {
                RestScriptFormatted = RestScriptFormatted.Replace("BODY", "");
            }
            //logger.DebugFormat("The rest powershell scripts to excuete {0}", RestScriptFormatted);

            powerShellContext.AddScript(RestScriptFormatted);
            try
            {
                powerShellContext.Streams.Error.Clear();
                powerShellContext.Streams.Warning.Clear();
                Collection<PSObject> results = powerShellContext.Invoke<PSObject>();
                if (results == null)
                {
                    logger.ErrorFormat("Invoke Powershell rest url {0} failed!", url);
                    return null;
                }
                if (powerShellContext.Streams.Error.Count > 0)
                {
                    foreach (var item in powerShellContext.Streams.Error)
                    {
                        logger.ErrorFormat("Invoke Powershell rest url {0} {1} failed {2}",action, url, item.ToString());
                    }
                    return null;
                }
                if (powerShellContext.Streams.Warning.Count > 0)
                {
                    foreach (var item in powerShellContext.Streams.Warning)
                    {
                        logger.WarnFormat("Invoke Powershell rest url {0} {1} warning {2}", action, url, item.ToString());
                    }
                }
                foreach (var psObject in results)
                {
                    //Console.WriteLine("{0}", psObject.Properties["ID"].Value.ToString());
                    //Console.WriteLine("{0}", psObject.ToString());
                    if (psObject == null)
                        continue;

                    List<T> result = JsonConvert.DeserializeObject<List<T>>(psObject.ToString());
                    if (result != null)
                    {
                        NuageObject = result;
                    }
                }
            }
            catch(Exception ex)
            {
                logger.ErrorFormat("Invoke Powershell rest url {0} failed! {1}",url, ex.Message);
                return null;
            }

            return NuageObject;
        }

        public void SetHyperVOVSPort(string vmName, string hypervHost, string hypervUsername, string HypervPasswd)
        {
            
            IEnumerable<PSObject> output;
            string errors;

            string scripts = @"
                Import-Module C:\OVS.psm1
                Set-VMNetworkAdapterOVSPortDirect -vmName VMNAME -OVSPortName VMNAME
";
            string RestScriptFormatted = scripts.Replace("VMNAME", vmName);
            logger.InfoFormat("Seting Hyper-V OVS port name with script '{0}'", RestScriptFormatted);

            bool result = nuagePS.RunPowerShellScriptRemote(RestScriptFormatted, hypervHost, hypervUsername, 
                                   HypervPasswd, out output, out errors);
            if (!result)
            {
                logger.ErrorFormat("Set HyperV OVS Port failed with error {0}", errors);
                return;
            }

            return;
        }

    }
}
