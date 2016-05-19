using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    interface NuageServerBaseClass
    {
        string post_data(Dictionary<string, string> create_params);
        string post_resource(string parent_id);
        string delete_resource(string id);
        string put_resource(string id);
        string get_all_resources();
        string get_all_resources_in_parent(string parent_id);
    }
}
