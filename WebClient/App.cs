using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuage.VSDClient
{
    class NuageClientTest
    {
        static void Main(string[] args)
        {
            Uri baseUrl = new Uri("https://192.168.239.12:8443/");
            string username = "csproot";
            string password = "csproot";


            nuageVSDSession nuSession = new nuageVSDSession(username, password, "csp", baseUrl);
            nuSession.start();

            Console.WriteLine("end");
            

        }


    }
}

