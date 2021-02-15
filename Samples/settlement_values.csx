// Prints all settlements and their current values.
// Note that this is not the same as barter price - it's the base price before
// it is affected by any barter penalties from perks, relation etc.

var values = 
    from fief in Town.AllFiefs
    let value = Campaign.Current.Models.SettlementValueModel.CalculateValueForFaction(fief.Settlement, fief.OwnerClan)
    orderby value descending
    select (fief.Settlement, (long)value);

var maxLength = Town.AllFiefs.Max(fief => fief.Name.ToString().Length);

foreach (var (settlement, value) in values) {
    Log.WriteLine($"{(settlement.IsTown ? "*" : "+")} {settlement.Name.ToString().PadRight(maxLength)} {value,11:C0}");
}
