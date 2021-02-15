// Makes every married female noble in player's clan pregnant, whenever they're not.
// Doesn't persist across saved games - must be reapplied on every load.

void DailyTick() {
    foreach (var noble in Hero.MainHero.Clan.Lords) {
        if (noble.IsAlive && noble.IsFemale && !noble.IsPregnant &&
            noble.Spouse != null && noble.Age >= 18 && noble.Age <= 45
        ) {
            MakePregnantAction.Apply(noble);
        }
    }
}

CampaignEvents.DailyTickEvent.AddNonSerializedListener(null, DailyTick);
