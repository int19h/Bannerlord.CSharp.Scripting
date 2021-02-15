// Kills all the nobles and mercenary leaders, except for those who belong
// to the player clan.

var nobles = (
    from clan in Campaign.Current.Clans
    where clan != Clan.PlayerClan && (clan.Kingdom != null || clan.IsClanTypeMercenary)
    from hero in clan.Heroes
    where hero.IsAlive
    select hero
).ToArray(); // snapshot

foreach (var hero in nobles) {
    Log.WriteLine($"Killing {hero}");
    KillCharacterAction.ApplyByMurder(hero);
}

return $"{nobles.Length} nobles killed!";
