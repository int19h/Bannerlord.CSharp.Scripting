void SetSkills(
    Hero[] heroes,
    int? oneHanded = null, int? twoHanded = null, int? polearm = null,
    int? bow = null, int? crossbow = null, int? throwing = null,
    int? riding = null, int? athletics = null, int? smithing = null,
    int? scouting = null, int? tactics = null, int? roguery = null,
    int? charm = null, int? leadership = null, int? trade = null,
    int? steward = null, int? medicine = null, int? engineering = null
) {
    foreach (var hero in heroes.Distinct()) {
        Log.WriteLine(hero);

        void SetSkill(SkillObject skill, int? value) {
            var oldValue = hero.GetSkillValue(skill);
            if (value is int newValue && newValue != oldValue) {
                newValue = Math.Min(newValue, 1023);
                Log.WriteLine($"  {skill}: {oldValue} -> {newValue}");
                hero.SetSkillValue(skill, newValue);
            }
        }

        SetSkill(DefaultSkills.OneHanded, oneHanded);
        SetSkill(DefaultSkills.TwoHanded, twoHanded);
        SetSkill(DefaultSkills.Polearm, polearm);

        SetSkill(DefaultSkills.Bow, bow);
        SetSkill(DefaultSkills.Crossbow, crossbow);
        SetSkill(DefaultSkills.Throwing, throwing);

        SetSkill(DefaultSkills.Riding, riding);
        SetSkill(DefaultSkills.Athletics, athletics);
        SetSkill(DefaultSkills.Crafting, smithing);

        SetSkill(DefaultSkills.Scouting, scouting);
        SetSkill(DefaultSkills.Tactics, tactics);
        SetSkill(DefaultSkills.Roguery, roguery);

        SetSkill(DefaultSkills.Charm, charm);
        SetSkill(DefaultSkills.Leadership, leadership);
        SetSkill(DefaultSkills.Trade, trade);

        SetSkill(DefaultSkills.Steward, steward);
        SetSkill(DefaultSkills.Medicine, medicine);
        SetSkill(DefaultSkills.Engineering, engineering);
    }
}

void SetSkills(Hero[] heroes, int all) => SetSkills(
    heroes,
    all, all, all,
    all, all, all,
    all, all, all,
    all, all, all,
    all, all, all,
    all, all, all
);    
