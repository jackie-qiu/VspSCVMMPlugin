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
    [AddIn("Nuage VCS Networking")]
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

    [AddIn("Nuage VCS Add-in")]
    public class NuageVSPNonContextual : ActionAddInBase
    {
        public override void PerformAction(IList<ContextObject> contextObjects)
        {
            MainWindow mainWindow = new MainWindow(this.PowerShellContext);

            mainWindow.Show();
        }
    }
}
