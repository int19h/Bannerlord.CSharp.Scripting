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
void ListUnmarried(string culture = null, string kingdom = null, bool? foreign = null) {
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
            if (kingdom != null && $"{clan.Kingdom?.Name}" != kingdom) {
                continue;
            }
            if (foreign == true && clan.Kingdom == hero.Clan.Kingdom) {
                continue;
            }
            if (foreign == false && clan.Kingdom != hero.Clan.Kingdom) {
                continue;
            }

            foreach (var other in clan.Lords.OrderByDescending(h => h.Age)) {
                var rcb = Campaign.Current.GetCampaignBehavior<RomanceCampaignBehavior>();
                if (Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(hero, other)) {
                    if (culture == null || $"{other.Culture.Name}" == culture) {
                        candidates.Add(other);
                    }
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

/// <summary>
/// Displays the contents of non-empty stashes in all your fiefs.
/// </summary>
void ShowStashes() {
    foreach (var fief in MyClan.Fiefs.OrderBy(it => $"{it}")) {
        var stash = fief.Settlement.Stash;
        if (stash.Count == 0) {
            continue;
        }

        Log.WriteLine($"{fief}:");
        foreach (var item in stash.OrderBy(it => $"{it.EquipmentElement.Item.Name}")) {
            Log.WriteLine($"  {item.EquipmentElement.Item.Name} - {item.Amount}");
        }
    }
}

void ShowIncome() {
    var tips = CampaignUIHelper.GetGoldTooltip(MyClan);
    foreach (var tip in tips) {
        Log.WriteLine($"{tip.DefinitionLabel} {tip.ValueLabel}");
    }
}

void ShowPartiesIncome() {
    foreach (var hero in MyClan.Heroes) {
        if (!hero.IsPartyLeader) continue;
        var g1 = hero.Gold;
        var g2 = hero.PartyBelongedTo.PartyTradeGold;
        Log.WriteLine($"{hero} {g1} {g2}");
    }
}

void ResetPartiesGold() {
    foreach (var hero in MyClan.Heroes) {
        if (hero == Me || !hero.IsPartyLeader) continue;
        Log.WriteLine($"{hero}");
        hero.Gold = 10000;
    }
}