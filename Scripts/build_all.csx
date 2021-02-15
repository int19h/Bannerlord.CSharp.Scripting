// Upgrades all buildings to level 3 in all towns and castles the player owns.

foreach (var fief in Hero.MainHero.Clan.Fiefs) {
    Log.WriteLine(fief);
    fief.BuildingsInProgress.Clear();
    foreach (var building in fief.Buildings) {
        building.CurrentLevel = 3;
    }
}
