using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public abstract class NuageBase
    {
        abstract public string ID { get; set; }
        abstract public string parentType { get; set; }
        abstract public string parentID { get; set; }
        abstract public string externalID { get; set; }
    }
}
