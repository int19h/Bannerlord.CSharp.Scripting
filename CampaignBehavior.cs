using System.IO;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public class CampaignBehavior : CampaignBehaviorBase {
        public override void RegisterEvents() {
            try {
                ScriptGlobals.Scripts.CampaignBehavior.RegisterEvents();
            } catch (FileNotFoundException) {
            }
        }

        public override void SyncData(IDataStore dataStore) {
            try {
                ScriptGlobals.Scripts.CampaignBehavior.SyncData(dataStore);
            } catch (FileNotFoundException) {
            }
        }
    }
}
