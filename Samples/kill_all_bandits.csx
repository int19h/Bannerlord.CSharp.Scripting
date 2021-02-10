using System.Linq;
using System.Runtime.Remoting.Messaging;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

var banditParties = (
    from party in MobileParty.All
    let clan = party.ActualClan
    where clan != null && clan != Clan.PlayerClan && clan.IsBanditFaction
    select party
).ToArray(); // snapshot

foreach (var party in banditParties) {
    Log.WriteLine($"Destroying {party}");
    DestroyPartyAction.Apply(null, party);
}

return $"{banditParties.Length} bandit parties destroyed!";
