using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    public class NuageCompare : IEqualityComparer<NuageBase>
    {


        public bool Equals(NuageBase x, NuageBase y)
        {
            return x.ID.Equals(y.ID);
        }

        public int GetHashCode(NuageBase obj)
        {
            return 1;
        }
    }
}
