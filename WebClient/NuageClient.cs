using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Nuage.VSDClient
{
    public class NuageClient
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuageClient));
        private string default_l3domain_template_name { get; set; }
        private RestProxy restproxy { get; set; }
        private Enterprise enterprise { get; set; }
        private Domain l3domain { get; set; }

        public NuageClient(string username, string password, string organization, Uri baseUrl, string version)
        {
            restproxy = new RestProxy(username, password, organization, baseUrl, version);

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

            this.default_l3domain_template_name = "SCVMM_L3_Template";

            l3domain = new Domain(restproxy);
            enterprise = new Enterprise(restproxy);
        }

        public Boolean LoginVSD()
        {
            return restproxy.LoginVSD();
        }

        public NuageDomainTemplate CreateDefaultL3DomainTemplate(string ent_id)
        {
            return l3domain.CreateL3DomainTemplate(ent_id, this.default_l3domain_template_name);
        }
        public NuageDomainTemplate GetDefaultL3DomainTemplate(string ent_id)
        {
            string filter = string.Format("NAME IS '{0}'", this.default_l3domain_template_name);

            List<NuageDomainTemplate> result = l3domain.GetL3DomainTemplates(ent_id, filter);
            if (result == null || result.Count() == 0)
            {
                logger.Error("Get Default L3 domain template Failed....");
                return null;
            }

            return result.First<NuageDomainTemplate>();
        }   

    }
}
