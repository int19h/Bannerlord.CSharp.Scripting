 bool IsHeir(Hero hero) =>
    hero == null ? false :
    hero == Hero.MainHero ? true :
    IsHeir(hero.Mother) || IsHeir(hero.Father);

void WriteHero(Hero hero, bool withSpouse = true, bool withGovernor = true) {
    Log.Write($"{hero} ({hero.Age:N0}/");
    Log.Write(hero.IsFemale ? "F" : "M");
    if (hero.IsFemale && hero.IsFertile && hero.Age >= 18 && hero.Age <= 45) {
        Log.Write("+");
    }
    Log.Write($"/{hero.Culture})");
    if (withGovernor && hero.GovernorOf != null) {
        Log.Write($", governor of {hero.GovernorOf}");
    }

    if (withSpouse && hero.Spouse != null) {
        Log.Write(", spouse of ");
        WriteHero(hero.Spouse, withSpouse: false);
    }
}

/// <summary>
/// Lists all living members of the clan, grouped by their culture.
/// </summary>
void ListMembers() {
    var heroesByCulture = 
        from hero in Hero.MainHero.Clan.Heroes
        where hero.IsAlive
        orderby $"{hero.Name}"
        group hero by hero.Culture into heroes
        orderby $"{heroes.Key}"
        select heroes;

    foreach (var heroes in heroesByCulture) {
        Log.WriteLine($"{heroes.Key}:");
        foreach (var hero in heroes) {
            Log.Write("  ");
            WriteHero(hero);
            Log.WriteLine();
        }
    }
}

/// <summary>
/// Lists all of the player clan's unmarried nobles of marriageable age.
/// </summary>
void ListUnmarried() {
    foreach (var hero in Hero.MainHero.Clan.Lords) {
        if (hero.IsAlive && hero.Age >= 18 && hero.Spouse == null) {
            if (IsHeir(hero)) {
                Log.Write("*");
            }
            WriteHero(hero);
            Log.WriteLine();
        }
    }
}

/// <summary>
/// Lists clan members that can be made governors, grouped by culture.
/// </summary>
void ListPotentialGovernors() {
    var heroesByCulture = 
        from hero in Hero.MainHero.Clan.Heroes
        where hero.IsAlive && hero.IsActive && !hero.IsChild
        orderby $"{hero.Name}"
        group hero by hero.Culture into heroes
        orderby $"{heroes.Key}"
        select heroes;

    foreach (var heroes in heroesByCulture) {
        Log.WriteLine($"{heroes.Key}:");
        foreach (var hero in heroes) {
            Log.Write("  ");
            WriteHero(hero);
            Log.WriteLine();
        }
    }
}

/// <summary>
/// Lists all living descendants of the player character that still belong to the same clan,
/// rendered as a family tree.
/// </summary>
void ListHeirs() {
    int indent = 0;

    void Process(Hero hero) {
        Log.Write(string.Concat(Enumerable.Repeat("  ", indent)));
        WriteHero(hero);
        Log.WriteLine();

        ++indent;
        foreach (var child in hero.Children) {
            if (child.IsAlive && child.Clan == hero.Clan) {
                Process(child);
            }
        }
        --indent;
    }

    Process(Hero.MainHero);
}