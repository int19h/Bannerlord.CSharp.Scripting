// Destroys all kingdoms, and makes all clans independent.
void CityStates() {
    foreach (var clan in Clan.All) {
        if (clan.Kingdom == null) {
            continue;
        }

        var color = clan.Kingdom.PrimaryBannerColor;
        ChangeKingdomAction.ApplyByLeaveWithRebellionAgainstKingdom(clan, false);
        clan.UpdateBannerColor(uint.MaxValue, color);
        clan.Banner?.ChangePrimaryColor(uint.MaxValue);
        clan.Banner?.ChangeIconColors(color);
    }

    foreach (var kingdom in Kingdom.All) {
        DestroyKingdomAction.Apply(kingdom);
    }
}
