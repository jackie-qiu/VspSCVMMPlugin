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
    public class Enterprise
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuageClient));
        private RestProxy restproxy { get; set; }

        public Enterprise(RestProxy proxy)
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
            this.restproxy = proxy;
           
        }

        public NuageEnterprise CreateEnterprise(string name)
        {
            NuageEnterprise net_partition = new NuageEnterprise();

            Dictionary<string, string> create_params = new Dictionary<string, string>();
            create_params.Add("name", name);

            List<NuageEnterprise> result = restproxy.CallRestPostAPI<NuageEnterprise>(
                                                 net_partition.post_resource(""),
                                                 net_partition.post_data(create_params)
                                                 );
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Create enterprise {0} Failed....", name);
                logger.Error(msg);
                throw new NuageException(msg);
            }
            net_partition = result.First();

            return net_partition;

        }

        public List<NuageEnterprise> GetEnterprises(string filter)
        {
            NuageEnterprise ent = new NuageEnterprise();

            List<NuageEnterprise> result = restproxy.CallRestGetAPI<NuageEnterprise>(ent.get_all_resources(), filter);
            if (result == null || result.Count() == 0)
            {
                string msg = string.Format("Get Enterprises Failed....");
                logger.Error(msg);
                throw new NuageException(msg);
            }

            return result;
        }

        public bool DeleteEnterprise(string id)
        {
            NuageEnterprise ent = new NuageEnterprise();

            return restproxy.CallRestDeleteAPI<NuageDomain>(ent.delete_resource(id));

        }

    }
}
