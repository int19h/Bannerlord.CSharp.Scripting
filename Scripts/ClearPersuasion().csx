void ClearPersuasion() {
    var ldcb = Campaign.Current.CampaignBehaviorManager.GetBehavior<LordDefectionCampaignBehavior>();
    ldcb.ClearPersuasion();
}
