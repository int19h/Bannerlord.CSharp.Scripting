using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public class SubModule : MBSubModuleBase {
        protected override void OnSubModuleLoad() {
            OmniSharpConfig.Update();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject) {
            base.OnGameStart(game, gameStarterObject);

            if (gameStarterObject is CampaignGameStarter campaignStarter) {
                campaignStarter.AddBehavior(new CampaignBehavior());
            }
        }
    }
}
