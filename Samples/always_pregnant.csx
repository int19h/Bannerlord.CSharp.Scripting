// Makes the player or their spouse (depending on the gender) pregnant as soon
// as they give birth. Doesn't persist across saved games - must be reapplied.
// Note that, regardless of the gender, the mother has to be married - otherwise
// the game will crash on birth!

using System.Linq;
using System.Runtime.Remoting.Messaging;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;


void DailyTick() {
    var target = Hero.MainHero.IsFemale ? Hero.MainHero : Hero.MainHero.Spouse;
    if (target != null && target.Spouse != null && !target.IsPregnant) {
        MakePregnantAction.Apply(target);
    }
}

CampaignEvents.DailyTickEvent.AddNonSerializedListener(null, DailyTick);
