// Increases the number of hearths in all villages the player owns.

const int Hearth = 10000;

foreach (var village in Hero.MainHero.Clan.Villages) {
    if (village.Hearth < Hearth) {
        Log.WriteLine($"{village}: {village.Hearth} -> {Hearth}");
        village.Hearth = Hearth;
    }
}
