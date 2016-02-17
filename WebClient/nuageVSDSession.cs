using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using log4net;
using log4net.Config;

namespace Nuage.VSDClient
{
    public class nuageVSDSession
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(nuageVSDSession));

        private string username { get; set; }
        private string password { get; set; }
        private string organization { get; set; }
        private string token { get; set; }
        private Uri baseUrl { get; set; }
        private NuageMe[] mes { get; set; }
        public  NuageEnterprise[] enterprise { get; set; }
        private NuageDomain[] domains { get; set; }
        private NuageZone[] zones { get; set; }
        private NuagePolicyGroup[] policyGroups { get; set; }
        private NuageRedirectionTarget[] redirectionTargets { get; set; }
        private NuageSubnet[] subnets { get; set; }

        public nuageVSDSession(string username, string password, string organization, Uri baseUrl)
        {
            FileInfo config = new FileInfo(".\\log.conf");
            XmlConfigurator.Configure(config);

            this.username = username;
            this.password = password;
            this.organization = organization;
            this.baseUrl = baseUrl;

 
        }

        public void start()
        {
            
            try
            {
                RunAsync().Wait();
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                {
                    logger.Error(ex.ToString());
                }
            }

        }

        public async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                string authKey = nuageBase64.Base64Encode(username + ":" + password);

                client.BaseAddress = baseUrl;

                // Add the http header
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("X-Nuage-Organization", this.organization);
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                client.DefaultRequestHeaders.Add("Authorization", "XREST " + authKey);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Config SSL Certificate Validation
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

                // List data response
                try
                {
                    HttpResponseMessage reponse = await client.GetAsync("nuage/api/v3_2/me"); //Blocking call
                    if (reponse.IsSuccessStatusCode)
                    {
                        this.mes = await reponse.Content.ReadAsAsync<NuageMe[]>();
                        this.token = nuageBase64.Base64Encode(username + ":" + mes[0].APIKey);
                        logger.DebugFormat("Token: {0}", this.token);
                        client.DefaultRequestHeaders.Remove("Authorization");
                        client.DefaultRequestHeaders.Add("Authorization", "XREST " + this.token);
                    }
                    else
                    {
                        logger.Error("Login VSD failed, please check the username and password!");
                        return;
                    }

                    //Get enterprises
                    reponse = await client.GetAsync("nuage/api/v3_2/enterprises"); //Blocking call
                    if (reponse.IsSuccessStatusCode)
                    {
                        this.enterprise = await reponse.Content.ReadAsAsync<NuageEnterprise[]>();

                        foreach (var item in this.enterprise)
                        {
                            logger.DebugFormat("Enterprise Name: {0}, ID :{1}", item.name, item.ID);
                        }

                    }
                    else
                    {
                        logger.Error("Get enterprises from VSD failed");
                        return;
                    }

                    //Get Domains
                    reponse = await client.GetAsync("nuage/api/v3_2/domains"); //Blocking call
                    if (reponse.IsSuccessStatusCode)
                    {
                        this.domains = await reponse.Content.ReadAsAsync<NuageDomain[]>();

                        foreach (var item in this.domains)
                        {
                            logger.DebugFormat("Domain Name: {0}, ID :{1}", item.name, item.ID);
                        }

                    }
                    else
                    {
                        logger.Error("Get Domains from VSD failed");
                        return;
                    }

                    //Get Zones
                    reponse = await client.GetAsync("nuage/api/v3_2/zones"); //Blocking call
                    if (reponse.IsSuccessStatusCode)
                    {
                        this.zones = await reponse.Content.ReadAsAsync<NuageZone[]>();

                        foreach (var item in this.zones)
                        {
                            logger.DebugFormat("Zones Name: {0}, ID :{1}", item.name, item.ID);
                        }

                    }
                    else
                    {
                        logger.Error("Get Zones from VSD failed");
                        return;
                    }

                    //Get Subnets
                    reponse = await client.GetAsync("nuage/api/v3_2/subnets"); //Blocking call
                    if (reponse.IsSuccessStatusCode)
                    {
                        this.subnets = await reponse.Content.ReadAsAsync<NuageSubnet[]>();

                        foreach (var item in this.subnets)
                        {
                            logger.DebugFormat("Subnets Name: {0}, ID :{1}", item.name, item.ID);
                        }

                    }
                    else
                    {
                        logger.Error("Get Subnets from VSD failed");
                        return;
                    }

                    ////Get PolicyGroups
                    reponse = await client.GetAsync("nuage/api/v3_2/policygroups"); //Blocking call
                    if (reponse.IsSuccessStatusCode)
                    {
                        this.policyGroups = await reponse.Content.ReadAsAsync<NuagePolicyGroup[]>();

                        foreach (var item in this.policyGroups)
                        {
                            logger.DebugFormat("PolicyGroups Name: {0}, ID :{1}", item.name, item.ID);
                        }

                    }
                    else
                    {
                        logger.Error("Get PolicyGroups from VSD failed");
                        return;
                    }

                    ////Get RedirectionTargets
                    reponse = await client.GetAsync("nuage/api/v3_2/redirectiontargets"); //Blocking call
                    if (reponse.IsSuccessStatusCode)
                    {
                        this.redirectionTargets = await reponse.Content.ReadAsAsync<NuageRedirectionTarget[]>();

                        foreach (var item in this.redirectionTargets)
                        {
                            logger.DebugFormat("RedirectionTargets Name: {0}, ID :{1}", item.name, item.ID);
                        }

                    }
                    else
                    {
                        logger.Error("Get RedirectionTargets from VSD failed");
                        return;
                    }

                }
                catch (HttpRequestException e)
                {
                    logger.ErrorFormat("Get metadaa from VSD {0} failed.", baseUrl);
                }


            }
        }

        //for testing purpose only, accept any dodgy certificate... 
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
