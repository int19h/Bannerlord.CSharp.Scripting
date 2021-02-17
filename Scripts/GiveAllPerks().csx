void GiveAllPerks(Hero[] heroes) {
    foreach (var hero in heroes.Distinct()) {
        Log.WriteLine(hero);

        foreach (var perk in DefaultPerks.GetAllPerks()) {
            if (!hero.GetPerkValue(perk)) {
                Log.WriteLine($"  + {perk}");
                hero.SetPerkValue(perk, true);
            }
        }
    }
}
