using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{

    public class NuageException : ApplicationException
    {
        public NuageException() { }
        public NuageException(string message) : base(message) { }
        public NuageException(string message, Exception inner) : base(message, inner) { }
     
    }
}
