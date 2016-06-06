/*
Copyright © 2016 Nuage. All rights reserved.
*/


using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns;
using Microsoft.SystemCenter.VirtualMachineManager.UIAddIns.ContextTypes;
using System.Collections.Generic;
using System.Linq;
using System.AddIn;
using Nuage.VSDClient.Main;

namespace Microsoft.VirtualManager.UI.AddIns.NuageVSP
{
    [AddIn("Nuage VCS Add-in")]
    public class NuageVSPContextual : ActionAddInBase
    {
        public override bool CheckIfEnabledFor(IList<ContextObject> contextObjects)
        {
            return contextObjects.OfType<VMContext>().Any();
        }
       
        public override void PerformAction(IList<ContextObject> contextObjects)
        {
            NuageVSPWindow mainWindow = new NuageVSPWindow(
                this.PowerShellContext, 
                contextObjects.OfType<VMContext>());

            mainWindow.Show();
        }
    }

    [AddIn("Nuage VCS Networking")]
    public class NuageVSPNonContextual : ActionAddInBase
    {
        public override void PerformAction(IList<ContextObject> contextObjects)
        {
            Login loginWindow = new Login(this.PowerShellContext);

            if(!loginWindow.IsLoginAutomatically())
                loginWindow.Show();
        }
    }
}
