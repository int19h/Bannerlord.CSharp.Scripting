/// <summary>
/// Shows how much the current owner of each fief values it.
/// </summary>
/// <remarks>
/// This is not the same as barter price - it's the base price before it is affected by any barter penalties
/// from perks, relation etc.
/// </remarks>
void ShowFiefValues(Town[] fiefs)  {
    var values = 
        from fief in fiefs
        let value = Campaign.Current.Models.SettlementValueModel.CalculateValueForFaction(fief.Settlement, fief.OwnerClan)
        orderby value descending
        select (fief, (long)value);

    var maxLength = Town.AllFiefs.Max(fief => fief.Name.ToString().Length);

    foreach (var (fief, value) in values) {
        var icon = fief.IsTown ? "@" : "*";
        var name = $"{fief}".PadRight(maxLength);
        Log.WriteLine($"{icon} {name} {value,11:C0}");
    }
}

void ShowFiefValues() => ShowFiefValues(All);
