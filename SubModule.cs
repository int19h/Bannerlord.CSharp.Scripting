using System;
using System.IO;
using TaleWorlds.MountAndBlade;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public class SubModule : MBSubModuleBase {
        protected override void OnSubModuleLoad() {
            /// Enable binding redirects from our app.config - needed for System.Span in Roslyn 3.x.
            //ConfigurationManager.OpenExeConfiguration(GetType().Assembly.Location);

            Scripts.Initialize();
        }
    }
}
