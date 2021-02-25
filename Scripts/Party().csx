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
