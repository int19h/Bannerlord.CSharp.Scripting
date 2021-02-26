void RegisterEvents() {
    CampaignEvents.DailyTickEvent.AddNonSerializedListener(null, () => Scripts.CampaignEvents.DailyTick());
}

void SyncData(IDataStore dataStore) {
}
