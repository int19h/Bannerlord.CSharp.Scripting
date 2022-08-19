void Kill(Hero[] heroes) {
    foreach (var hero in heroes) {
        if (hero.IsAlive) {
            Log.WriteLine(hero);
            KillCharacterAction.ApplyByMurder(hero);
        }
    }
}

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

void GiveAllPerks(Hero[] heroes) {
    foreach (var hero in heroes.Distinct()) {
        Log.WriteLine(hero);

        foreach (var perk in Perks) {
            if (!hero.GetPerkValue(perk)) {
                Log.WriteLine($"  + {perk}");
                IgnoreVisibility(() => hero.SetPerkValueInternal(perk, true));
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

        void SetAttribute(CharacterAttribute attr, int? value) {
            var oldValue = hero.GetAttributeValue(attr);
            if (value is int newValue && newValue != oldValue) {
                Log.WriteLine($"  {attr.Name}: {oldValue} -> {newValue}");
                IgnoreVisibility(() => hero.SetAttributeValueInternal(attr, newValue));
            }
        }

        SetAttribute(DefaultCharacterAttributes.Vigor, vigor);
        SetAttribute(DefaultCharacterAttributes.Endurance, endurance);
        SetAttribute(DefaultCharacterAttributes.Control, control);
        SetAttribute(DefaultCharacterAttributes.Cunning, cunning);
        SetAttribute(DefaultCharacterAttributes.Social, social);
        SetAttribute(DefaultCharacterAttributes.Intelligence, intelligence);
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
                IgnoreVisibility(() => hero.SetSkillValue(skill, newValue));
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

void AddFocus(
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

        void AddFocus(SkillObject skill, int? value) {
            if (value is null) {
                return;
            }
            var oldValue = hero.HeroDeveloper.GetFocus(skill);
            var newValue = Math.Min(oldValue + value.Value, 5);
            if (oldValue != newValue) {
                Log.WriteLine($"  {skill}: {oldValue} -> {newValue}");
                hero.HeroDeveloper.AddFocus(skill, newValue - oldValue, checkUnspentFocusPoints: false);
            }
        }

        AddFocus(DefaultSkills.OneHanded, oneHanded);
        AddFocus(DefaultSkills.TwoHanded, twoHanded);
        AddFocus(DefaultSkills.Polearm, polearm);

        AddFocus(DefaultSkills.Bow, bow);
        AddFocus(DefaultSkills.Crossbow, crossbow);
        AddFocus(DefaultSkills.Throwing, throwing);

        AddFocus(DefaultSkills.Riding, riding);
        AddFocus(DefaultSkills.Athletics, athletics);
        AddFocus(DefaultSkills.Crafting, smithing);

        AddFocus(DefaultSkills.Scouting, scouting);
        AddFocus(DefaultSkills.Tactics, tactics);
        AddFocus(DefaultSkills.Roguery, roguery);

        AddFocus(DefaultSkills.Charm, charm);
        AddFocus(DefaultSkills.Leadership, leadership);
        AddFocus(DefaultSkills.Trade, trade);

        AddFocus(DefaultSkills.Steward, steward);
        AddFocus(DefaultSkills.Medicine, medicine);
        AddFocus(DefaultSkills.Engineering, engineering);
    }
}

void AddFocus(Hero[] heroes, int all) => AddFocus(
    heroes,
    all, all, all,
    all, all, all,
    all, all, all,
    all, all, all,
    all, all, all,
    all, all, all
);    

void SetTraits(
    Hero[] heroes,
    int? mercy = null,
    int? valor = null,
    int? honor = null,
    int? generosity = null,
    int? calculating = null
) {
    foreach (var hero in heroes.Distinct()) {
        Log.WriteLine(hero);

        void SetTrait(TraitObject trait, int? value) {
            var oldValue = hero.GetTraitLevel(trait);
            if (value is int newValue && newValue != oldValue) {
                newValue = Math.Max(Math.Min(newValue, 2), -2);
                Log.WriteLine($"  {trait.Name}: {oldValue} -> {newValue}");
                hero.SetTraitLevel(trait, newValue);
            }
        }

        SetTrait(DefaultTraits.Mercy, mercy);
        SetTrait(DefaultTraits.Valor, valor);
        SetTrait(DefaultTraits.Honor, honor);
        SetTrait(DefaultTraits.Generosity, generosity);
        SetTrait(DefaultTraits.Calculating, calculating);
    }
}

void SetTraits(Hero[] heroes, int all) => SetTraits(heroes, all, all, all, all, all);

void AddPower(Hero[] heroes, float power) {
    foreach (var hero in heroes) {
        var oldPower = hero.Power;
        hero.AddPower(power);
        Log.WriteLine($"{hero}: {oldPower} -> {hero.Power}");
    }
}

void Educate(Hero[] heroes) {
    var ecb = Campaign.Current.CampaignBehaviorManager.GetBehavior<EducationCampaignBehavior>();
    foreach (var hero in heroes) {
        if (!hero.IsChild) {
            continue;
        }
        Log.WriteLine(hero);
        IgnoreVisibility(() => ecb.OnHeroComesOfAge(hero));
    }
    SandBox.View.Map.MapScreen.Instance.MapNotificationView.ResetNotifications();
}

void SetAge(Hero[] heroes, int age) {
    foreach (var hero in heroes) {
        hero.SetBirthDay(Helpers.HeroHelper.GetRandomBirthDayForAge(age));
    }
}
