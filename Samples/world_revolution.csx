// Makes all towns owned by kingdoms, except those that belong to the player clan,
// revolt against their owners.

using System.Linq;
using System.Runtime.Remoting.Messaging;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

foreach (var town in Town.AllTowns.ToArray()) {
    if (town.OwnerClan == Clan.PlayerClan || town.OwnerClan.Kingdom == null) {
        continue;
    }
    town.Settlement.Militia = 100000;
    town.Loyalty = -100;
    town.Security = -100;
}
