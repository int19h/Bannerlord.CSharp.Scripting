using TaleWorlds.MountAndBlade;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public class SubModule : MBSubModuleBase {
        protected override void OnSubModuleLoad() {
            ScriptFiles.Initialize();
        }
    }
}
