void Destroy(MobileParty[] parties) {
    foreach (var party in parties) {
        Log.WriteLine($"{party}");
        DestroyPartyAction.Apply(null, party);
    }
}

void DestroyAllBandits() {
    var banditParties = 
        from party in MobileParty.All
        let clan = party.ActualClan
        where clan != null && clan != Clan.PlayerClan && clan.IsBanditFaction
        select party;
    Destroy(banditParties.ToArray());
}

void GiveFood(MobileParty[] parties, int amount) {
    var foodIds = new[] { "grain", "fish", "grape", "olives", "butter", "meat", "date_fruit", "cheese", "beer", "wine", "oil" };
    var foods = from id in foodIds select ItemObjects.Single(it => it.StringId == id);
    foreach (var party in parties) {
        Log.WriteLine($"{party}");
        foreach (var food in foods) {
            var oldValue = party.ItemRoster.GetItemNumber(food);
            party.ItemRoster.AddToCounts(food, amount);
            var newValue = party.ItemRoster.GetItemNumber(food);
            Log.WriteLine($"  {food}: {oldValue} -> {newValue}");
        }
    }
}
