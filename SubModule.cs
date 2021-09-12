using System.IO;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CSharp.RuntimeBinder;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public class SubModule : MBSubModuleBase {
        protected override void OnSubModuleLoad() {
            OmniSharpConfig.Update();
        }

        protected override void OnGameStart(Game game, IGameStarter starter) {
            base.OnGameStart(game, starter);

            if (starter is CampaignGameStarter campaignStarter) {
                campaignStarter.AddBehavior(new CampaignBehavior());
            }

            dynamic? subModule;
            try {
                subModule = ScriptGlobals.Scripts.SubModule;
            } catch (CompilationErrorException) {
                subModule = null;
            }
            try {
                subModule?.OnGameStart(game, starter);
            } catch (RuntimeBinderException) {
            }
        }
    }
}
