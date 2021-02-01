// Prints all settlements and their current values.
// Note that this is not the same as barter price - it's the base price before
// it is affected by any barter penalties from perks, relation etc.

using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

var fiefs = Town.AllTowns.Union(Town.AllCastles).ToArray();

var values = (
    from fief in fiefs
    let value = Campaign.Current.Models.SettlementValueModel.CalculateValueForFaction(fief.Settlement, Hero.MainHero.Clan)
    orderby value descending
    select (fief.Settlement, (long)value)
).ToArray();

var maxLength = fiefs.Max(fief => fief.Name.ToString().Length);

foreach (var (settlement, value) in values) {
    Log.WriteLine($"{(settlement.IsTown ? "*" : "+")} {settlement.Name.ToString().PadRight(maxLength)} {value,11:C0}");
}
