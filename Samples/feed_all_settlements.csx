// Fills the granaries of all towns and castles, and for towns, also adds
// food to their respective marketplaces.

using System.Linq;
using System.Runtime.Remoting.Messaging;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

const int ExtraFood = 1000000;

var towns = Town.AllTowns.Union(Town.AllCastles).ToArray();
foreach (var town in towns) {
    Log.Write($"{town} food stocks: {town.FoodStocks}");
    town.FoodStocks = town.FoodStocksUpperLimit();
    Log.WriteLine($" -> {town.FoodStocks}");
}

var foods = ItemObject.All.Where(item => item.IsFood).ToArray();
var settlements = Settlement.All.Where(setl => setl.IsTown || setl.IsVillage).ToArray();
foreach (var settlement in settlements) {
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

return
    $"Maxed out food stocks for {towns.Length} towns and castles.\n" +
    $"Added food to item rosters of {settlements.Length} towns and villages.\n";
