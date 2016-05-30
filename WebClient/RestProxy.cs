using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Management.Automation;
using System.Collections.ObjectModel;
using log4net;
using log4net.Config;
using Newtonsoft.Json;

namespace Nuage.VSDClient
{
    public class RestProxy
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RestProxy));
        private Uri baseUrl { get; set; }
        private string username { get; set; }
        private string password { get; set; }
        private string organization { get; set; }
        private string version { get; set; }
        private string token { get; set; }

        public RestProxy(string username, string password, string organization, Uri baseUrl, string version)
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

            this.username = username;
            this.password = password;
            this.organization = organization;
            this.baseUrl = baseUrl;
            this.version = version;
        }

        public Boolean LoginVSD()
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + "/me";
            string authKey = nuageBase64.Base64Encode(this.username + ":" + this.password);

            List<NuageMe> result = this.CallRestAPI<NuageMe>(url, authKey, "Get", null, null);
            if (result == null || result.Count() == 0)
            {
                logger.Error("Login VSD Failed....");
                return false;
            }

            this.token = nuageBase64.Base64Encode(username + ":" + result.First().APIKey);
            logger.DebugFormat("Token: {0}", this.token);
            return true;

        }

        public List<T> CallRestGetAPI<T>(string resource, string filter)
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + resource;
            return CallRestAPI<T>(url, this.token, "Get", null, filter);
        }

        public Boolean CallRestPutAPI<T>(string resource, string content)
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + resource;
            CallRestAPI<T>(url, this.token, "Put", content, null);
            return true;
        }

        public List<T> CallRestPostAPI<T>(string resource, string content)
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + resource;
            return CallRestAPI<T>(url, this.token, "Post", content, null);
        }

        public Boolean CallRestDeleteAPI<T>(string resource)
        {
            string url = this.baseUrl.ToString() + "nuage/api/" + version + resource;
            CallRestAPI<T>(url, this.token, "Delete", null, null);
            return true;
        }

        private Boolean AddCertificatePolicyType(PowerShell powerShellContext)
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

        private List<T> CallRestAPI<T>(string url, string token, string action, string content, string filter)
        {

            List<T> NuageObject = new List<T>();
            PowerShell powerShellContext = PowerShell.Create();

            if (!AddCertificatePolicyType(powerShellContext))
            {
               powerShellContext.Dispose();
               throw new NuageException("Add Certificate policy type failed");
            }

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
                    $warning_message = 'Failed with status code ' + $_.Exception.Response.StatusCode.Value__ + ' and error message ' + $_.Exception.Response.StatusDescription
                    Write-Warning $warning_message
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
                    throw new NuageException("Invoke REST API failed.");
                }
                if (powerShellContext.Streams.Error.Count > 0)
                {
                    string error_msg = "";
                    foreach (var item in powerShellContext.Streams.Error)
                    {
                        logger.ErrorFormat("Invoke Powershell rest url {0} {1} failed {2}", action, url, item.ToString());
                        error_msg = item.ToString();
                    }
                    throw new NuageException(error_msg);
                }
                if (powerShellContext.Streams.Warning.Count > 0)
                {
                    string warn_msg = "";
                    foreach (var item in powerShellContext.Streams.Warning)
                    {
                        logger.WarnFormat("Invoke Powershell rest url {0} {1} warning {2}", action, url, item.ToString());
                        warn_msg = item.ToString();
                    }
                    throw new NuageException(warn_msg);
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
            catch (NuageException e)
            {
                throw e;
            }
            catch (Exception ex)
            {
                string error_message = string.Format("Invoke Powershell rest url {0} failed! {1}", url, ex.Message);
                logger.Error(error_message);
                throw new NuageException(error_message, ex);
            }
            finally
            {
                powerShellContext.Dispose();
            }

            return NuageObject;
        }
    }
}
