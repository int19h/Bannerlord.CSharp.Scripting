#load "lib/Helpers.csx"

/// <summary>
/// Lists all living members of the clan, grouped by their culture.
/// </summary>
void ListMembers() {
    var heroesByCulture = 
        from hero in MyClan.Heroes
        where hero.IsAlive
        orderby $"{hero.Name}"
        group hero by hero.Culture into heroes
        orderby $"{heroes.Key}"
        select heroes;

    foreach (var heroes in heroesByCulture) {
        Log.WriteLine($"{heroes.Key}:");
        foreach (var hero in heroes) {
            Log.Write("  ");
            Log.WriteHero(hero, withGovernor: true, withSpouse: true);
            Log.WriteLine();
        }
    }
}

/// <summary>
/// Lists all of the player clan's unmarried nobles of marriageable age.
/// </summary>
void ListUnmarried() {
    foreach (var hero in MyClan.Lords.OrderBy(h => $"{h.Name}")) {
        if (!hero.IsAlive || hero.Age < 18 || hero.Spouse != null) {
            continue;
        }

        Log.WriteHero(hero, withGovernor: true, withSpouse: true);
        var candidates = new List<Hero>();

        foreach (var clan in Clan.All) {
            if (clan == MyClan) {
                continue;
            }

            foreach (var other in clan.Lords.OrderByDescending(h => h.Age)) {
                if (Romance.MarriageCourtshipPossibility(hero, other)) {
                    candidates.Add(other);
                    break;
                }
            }
        }

        if (!candidates.Any()) {
            Log.WriteLine("; no candidates.");
            continue;
        }

        Log.WriteLine("; can marry:");
        foreach (var other in candidates.OrderBy(h => h.Age)) {
            Log.Write("  ");
            Log.WriteHero(other, withClan: true);
            Log.WriteLine();
        }
    }
}

/// <summary>
/// Lists clan members that can be made governors, grouped by culture.
/// </summary>
void ListPotentialGovernors() {
    var heroesByCulture = 
        from hero in MyClan.Heroes
        where hero.IsAlive && hero.IsActive && !hero.IsChild
        orderby $"{hero.Name}"
        group hero by hero.Culture into heroes
        orderby $"{heroes.Key}"
        select heroes;

    foreach (var heroes in heroesByCulture) {
        Log.WriteLine($"{heroes.Key}:");
        foreach (var hero in heroes) {
            Log.Write("  ");
            Log.WriteHero(hero, withGovernor: true, withSpouse: true);
            Log.WriteLine();
        }
    }
}

/// <summary>
/// Lists all living descendants of specified character that still belong to the same clan,
/// rendered as a family tree. If no character is specified, lists heirs of the player.
/// </summary>
void ListHeirs(Hero root = null) {
    int indent = 0;

    void Process(Hero hero) {
        Log.Write(string.Concat(Enumerable.Repeat("  ", indent)));
        Log.WriteHero(hero, withGovernor: true, withSpouse: true);
        Log.WriteLine();

        ++indent;
        foreach (var child in hero.Children) {
            if (child.IsAlive && child.Clan == hero.Clan) {
                Process(child);
            }
        }
        --indent;
    }

    Process(root ?? Me);
}

/// <summary>
/// Lists all fiefs that have buildings that can be upgraded.
/// </summary>
void ListUpgradeableFiefs() {
    foreach (var fief in MyClan.Fiefs) {
        var loggedFief = false;
        foreach (var building in fief.Buildings) {
            if (building.CurrentLevel < 3 && !building.BuildingType.IsDefaultProject) {
                if (!loggedFief) {
                    Log.WriteLine(fief);
                    loggedFief = true;
                }
                Log.WriteLine($"  {building.Name} - {building.CurrentLevel}");
            }
        }
    }
}
