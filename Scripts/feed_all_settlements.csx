// Maxes out food stocks of all settlements. For towns and villages, also adds food to their markets.

const int ExtraFood = 1000000;

foreach (var town in Town.AllFiefs) {
    Log.Write($"{town} food stocks: {town.FoodStocks}");
    town.FoodStocks = town.FoodStocksUpperLimit();
    Log.WriteLine($" -> {town.FoodStocks}");
}

var foods = ItemObject.All.Where(item => item.IsFood).ToArray();
foreach (var settlement in Settlement.All) {
    if (!settlement.IsTown && !settlement.IsVillage) {
        continue;
    }

    Log.WriteLine($"{settlement} item roster:");
    var roster = settlement.ItemRoster;

    foreach (var food in foods) {
        var index = roster.FindIndexOfItem(food);
        var count = index < 0 ? 0 : roster.GetElementNumber(index);
        Log.Write($"\t{food}: {count}");

        settlement.ItemRoster.AddToCounts(food, ExtraFood);

        index = roster.FindIndexOfItem(food);
        count = index < 0 ? 0 : roster.GetElementNumber(index);
        Log.WriteLine($" -> {count}");
    }
}
