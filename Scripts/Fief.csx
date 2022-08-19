IEnumerable<ItemObject> Items() => Game.Current.ObjectManager.GetObjectTypeList<ItemObject>();

void BuildAll(Town[] fiefs) {
    foreach (var fief in fiefs) {
        Log.WriteLine(fief);
        fief.BuildingsInProgress.Clear();
        foreach (var building in fief.Buildings) {
            building.CurrentLevel = 3;
        }
    }
}

void SetFoodStocks(Town[] fiefs, int value = int.MaxValue) {
    if (value < 0) {
        value = 0;
    }
    
    foreach (var fief in fiefs) {
        int newValue = Math.Min(value, fief.FoodStocksUpperLimit());
        Log.WriteLine($"{fief}: {fief.FoodStocks} -> {newValue}");
        fief.FoodStocks = newValue;
    }
}

void AddFoodToMarket(Town[] fiefs, int extraFood = 1000) {
    var foods = Items().Where(item => item.IsFood).ToArray();
    foreach (var fief in fiefs) {
        if (!fief.IsTown) {
            continue;
        }

        Log.WriteLine($"{fief}:");
        var roster = fief.Settlement.ItemRoster;

        foreach (var food in foods) {
            var index = roster.FindIndexOfItem(food);
            var count = index < 0 ? 0 : roster.GetElementNumber(index);
            Log.Write($"\t{food}: {count}");

            roster.AddToCounts(food, extraFood);

            index = roster.FindIndexOfItem(food);
            count = index < 0 ? 0 : roster.GetElementNumber(index);
            Log.WriteLine($" -> {count}");
        }
    }
}

/// <summary>
/// Shows how much the current owner of each fief values it.
/// </summary>
/// <remarks>
/// This is not the same as barter price - it's the base price before it is affected by any barter penalties
/// from perks, relation etc.
/// </remarks>
void ShowValue(Town[] fiefs)  {
    var maxLength = fiefs.Max(fief => fief.Name.ToString().Length);
    var lines =
        from fief in fiefs
        let value = Campaign.Current.Models.SettlementValueModel.CalculateSettlementValueForFaction(fief.Settlement, fief.OwnerClan)
        orderby value descending
        let icon = fief.IsTown ? "@" : "*"
        let name = $"{fief.Name}".PadRight(maxLength)
        select $"{icon} {name} {value,11:C0}";
    foreach (var line in lines) {
        Log.WriteLine(line);
    }
}

void ShowValue(Kingdom kingdom) => ShowValue(kingdom.Fiefs.ToArray());

void ShowValue() => ShowValue(All);

void DisbandGarrison(Town[] fiefs) {
    foreach (var fief in fiefs) {
        Log.WriteLine(fief);
        fief.GarrisonParty.MemberRoster.Reset();
    }
}

void PrioritizeFood(Town[] fiefs) {
    foreach (var fief in fiefs) {
        foreach (var building in fief.Buildings) {
            if (!building.IsCurrentlyDefault && building.BuildingType == DefaultBuildingTypes.IrrigationDaily) {
                Log.WriteLine($"{fief} -> {building.BuildingType}");
                Helpers.BuildingHelper.ChangeDefaultBuilding(building, fief);
                break;
            }
        }
    }
}

void Rebel(Town[] fiefs) {
    var rcb = Campaign.Current.CampaignBehaviorManager.GetBehavior<RebellionsCampaignBehavior>();
    foreach (var fief in fiefs) {
        Log.WriteLine(fief);
        IgnoreVisibility(() => rcb.StartRebellionEvent(fief.Settlement));
    }
}

void ListVillages(Town[] towns) {
    foreach (var town in towns) {
        if (!town.IsTown) {
            continue;
        }
        Log.WriteLine($"{town}");
        foreach (var village in Village.All) {
            if (village.TradeBound.Town == town) {
                Log.WriteLine($"    {village}:");
                foreach (var (item, value) in village.VillageType.Productions) {
                    Log.WriteLine($"      {item} ({value})");
                }
            }
        }
    }
}

void ShowProductions(Town[] towns) {
    IEnumerable<(ItemObject Item, float Value)> GetProductions(Town town) {
        var prods = new Dictionary<ItemObject, float>();
        foreach (var village in Village.All) {
            if (village.TradeBound.Town == town) {
                foreach (var (item, value) in village.VillageType.Productions) {
                    prods.TryGetValue(item, out var n);
                    n += value;
                    prods[item] = n;
                }
            }
        }
        foreach (var kv in prods) {
            yield return (kv.Key, kv.Value);
        }
    }

    IEnumerable<(Town Town, IEnumerable<(ItemObject Item, float Value)> Productions)> townProds =
        from town in towns
        where town.IsTown
        let prods = GetProductions(town)
        let total = prods.Sum(tp => tp.Value)
        orderby total descending
        select (town, prods);

    foreach (var (town, prods) in townProds) {
        Log.WriteLine($"{town}:");
        foreach (var p in prods.OrderByDescending(p => p.Value)) {
            Log.WriteLine($"    {p.Item} - {p.Value}");
        }
    }

    var prodItems = (
        from tp in townProds
        from p in tp.Productions
        orderby p.Item.StringId
        select p.Item
    ).Distinct();
    using (var f = System.IO.File.CreateText("c:/temp/production.csv")) {
        f.Write("\"\"");
        foreach (var item in prodItems) {
            f.Write($",{item.StringId}");
        }
        f.WriteLine();

        foreach (var (town, prods) in townProds) {
            f.Write('"' + town.Name.ToString() + '"');
            foreach (var item in prodItems) {
                var value = (
                    from p in prods
                    where p.Item == item
                    select p.Value
                ).FirstOrDefault();
                f.Write($",{value}");
            }
            f.WriteLine();
        }
    }
}