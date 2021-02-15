// Disbands garrisons in all fiefs that belong to the player.

foreach (var fief in Hero.MainHero.Clan.Fiefs) {
    Log.WriteLine(fief);
    fief.GarrisonParty.MemberRoster.Reset();
}
