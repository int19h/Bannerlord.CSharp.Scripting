void GiveAllPerks(Hero[] heroes) {
    foreach (var hero in heroes.Distinct()) {
        Log.WriteLine(hero);

        foreach (var perk in DefaultPerks.GetAllPerks()) {
            if (!hero.GetPerkValue(perk)) {
                Log.WriteLine($"  + {perk}");
                hero.SetPerkValue(perk, true);
            }
        }
    }
}

void SetAttributes(
    Hero[] heroes,
    int? vigor = null,
    int? endurance = null,
    int? control = null,
    int? cunning = null,
    int? social = null,
    int? intelligence = null
) {
    foreach (var hero in heroes.Distinct()) {
        Log.WriteLine(hero);

        void SetAttribute(CharacterAttributesEnum attr, int? value) {
            var oldValue = hero.GetAttributeValue(attr);
            if (value is int newValue && newValue != oldValue) {
                Log.WriteLine($"  {attr}: {oldValue} -> {newValue}");
                hero.SetAttributeValue(attr, newValue);
            }
        }

        SetAttribute(CharacterAttributesEnum.Vigor, vigor);
        SetAttribute(CharacterAttributesEnum.Endurance, endurance);
        SetAttribute(CharacterAttributesEnum.Control, control);
        SetAttribute(CharacterAttributesEnum.Cunning, cunning);
        SetAttribute(CharacterAttributesEnum.Social, social);
        SetAttribute(CharacterAttributesEnum.Intelligence, intelligence);
    }
}

void SetAttributes(Hero[] heroes, int all) =>
    SetAttributes(heroes, all, all, all, all, all, all);

void MaxAttributes(
    Hero[] heroes,
    bool vigor = false,
    bool endurance = false,
    bool control = false,
    bool cunning = false,
    bool social = false,
    bool intelligence = false
) {
    int? NewValue(bool max) => max ? 10 : (int?)null;
    SetAttributes(
        heroes,
        NewValue(vigor),
        NewValue(endurance),
        NewValue(control),
        NewValue(cunning),
        NewValue(social),
        NewValue(intelligence)
    );
}

void MaxAttributes(Hero[] heroes) =>
    MaxAttributes(heroes, true, true, true, true, true, true);

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

void MakePregnant(Hero[] heroes) {
    foreach (var hero in heroes) {
        if (hero.IsAlive && hero.IsFemale && !hero.IsPregnant &&
            hero.Spouse != null && hero.Age >= 18 && hero.Age <= 45
        ) {
            Log.WriteLine(hero);
            MakePregnantAction.Apply(hero);
        }
    }
}

void KillOtherNobles() {
    var nobles = (
        from clan in Campaign.Current.Clans
        where clan != Clan.PlayerClan && (clan.Kingdom != null || clan.IsClanTypeMercenary)
        from hero in clan.Heroes
        where hero.IsAlive
        select hero
    ).ToArray(); // snapshot

    foreach (var hero in nobles) {
        Log.WriteLine(hero);
        hero.Kill();
    }
}