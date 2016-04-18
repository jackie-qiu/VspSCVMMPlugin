using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using log4net;
using log4net.Config;

namespace Nuage.VSDClient
{
    public class NuagePowerShell
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(NuagePowerShell));
        public NuagePowerShell()
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
        }

        public SecureString ConvertToSecureString(String input)
        {
            SecureString _output = new SecureString();
            input.ToCharArray().ToList().ForEach((q) => _output.AppendChar(q));
            return _output;
        }

        public bool RunPowerShellScript(string script, out IEnumerable<PSObject> output, out string errors)
        {
            return RunPowerShellScriptInternal(script, out output, out errors, null);
        }

        public bool RunPowerShellScriptRemote(string script, string computer, string username, string password, out IEnumerable<PSObject> output, out string errors)
        {
            output = Enumerable.Empty<PSObject>();

            var credentials = new PSCredential(username, ConvertToSecureString(password));
            var connectionInfo = new WSManConnectionInfo(false, computer, 5985, "/wsman", "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", credentials);
            var runspace = RunspaceFactory.CreateRunspace(connectionInfo);

            try
            {
                runspace.Open();
            }
            catch (Exception e)
            {
                errors = e.Message;
                return false;
            }

            return RunPowerShellScriptInternal(script, out output, out errors, runspace);
        }

        public bool RunPowerShellScriptInternal(string script, out IEnumerable<PSObject> output, out string errors, Runspace runspace)
        {
            output = Enumerable.Empty<PSObject>();

            using (var ps = PowerShell.Create())
            {
                ps.Runspace = runspace;
                ps.AddScript(script);

                try
                {
                    output = ps.Invoke();
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Error occurred in PowerShell script: {0}" + e);
                    errors = e.Message;
                    return false;
                }

                if (ps.Streams.Error.Count > 0)
                {
                    errors = String.Join(Environment.NewLine, ps.Streams.Error.Select(e => e.ToString()));
                    return false;
                }

                errors = String.Empty;
                return true;
            }
        }
    }
}
