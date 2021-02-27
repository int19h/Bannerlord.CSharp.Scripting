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
    var foods = ItemObject.All.Where(item => item.IsFood).ToArray();
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
        let value = Campaign.Current.Models.SettlementValueModel.CalculateValueForFaction(fief.Settlement, fief.OwnerClan)
        orderby value descending
        let icon = fief.IsTown ? "@" : "*"
        let name = $"{fief.Name}".PadRight(maxLength)
        select $"{icon} {name} {value,11:C0}";
    foreach (var line in lines) {
        Log.WriteLine(line);
    }
}

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
        rcb.GetType().InvokeMember(
            "StartRebellionEvent",
            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            rcb,
            new object[] { fief.Settlement },
            new ParameterModifier[0],
            null,
            null
        );
    }
}
