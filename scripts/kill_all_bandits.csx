using System.Linq;
using System.Runtime.Remoting.Messaging;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

var banditParties = (
    from clan in Campaign.Current.Clans
    where clan != Clan.PlayerClan && clan.IsBanditFaction
    from party in clan.Parties
    select party
).ToArray(); // snapshot, since the list will change as we iterate and destroy them

foreach (var party in banditParties) {
    Log.WriteLine($"Destroying {party}");
    DestroyPartyAction.Apply(null, party);
}

return $"{banditParties.Length} bandit parties destroyed!";
