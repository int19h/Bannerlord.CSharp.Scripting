void ShowChances() {
    var conv = Campaign.Current.ConversationManager;
    foreach (var opt in conv.CurOptions) {
        conv.GetPersuasionChances(opt, out var succ, out var critSucc, out var critFail, out var fail);
        Log.WriteLine($"{opt.Text} ({opt.PersuationOptionArgs?.ArgumentStrength}):\n  S={succ} SS={critSucc} F={fail} FF={critFail}");
    }
}

void Persuade() {
    var pers = IgnoreVisibility(() => ConversationManager._persuasion);
    var newPers = new Persuasion(0, pers.SuccessValue, pers.FailValue, pers.CriticalSuccessValue, pers.CriticalFailValue, pers.Progress, PersuasionDifficulty.VeryEasy);
    IgnoreVisibility(() => Set(out ConversationManager._persuasion, newPers));
}

void ClearPersuasion() {
    var ldcb = Campaign.Current.CampaignBehaviorManager.GetBehavior<LordDefectionCampaignBehavior>();
    ldcb.ClearPersuasion();
}
