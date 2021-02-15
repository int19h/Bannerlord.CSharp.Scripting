// Makes the hero have all perks (including mutually exclusive ones!).

foreach (var perk in DefaultPerks.GetAllPerks()) {
    Log.WriteLine(perk);
    Hero.MainHero.SetPerkValue(perk, true);
}
