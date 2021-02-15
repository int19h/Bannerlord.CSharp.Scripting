// Makes all towns owned by kingdoms revolt against their owners.

foreach (var town in Town.AllFiefs) {
    Log.Write(town);
    if (town.OwnerClan.Kingdom == null) {
        Log.WriteLine(" - not in a kingdom");
        continue;
    } else if (Arguments.Any(x => x.ToLower() == $"{town}".ToLower())) {
        Log.WriteLine(" - excluded");
        continue;
    }
    Log.WriteLine(" - REVOLTING!");

    var rcb = Campaign.Current.CampaignBehaviorManager.GetBehavior<RebellionsCampaignBehavior>();
    rcb.GetType().InvokeMember(
        "StartRebellionEvent",
        BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
        null,
        rcb,
        new object[] { town.Settlement },
        new ParameterModifier[0],
        null,
        null
    );
}
