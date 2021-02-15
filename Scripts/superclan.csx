// Turns all heroes in your clan into supermen, maxing out their attributes
// and skills, and giving them all perks.

const int AttributePoints = 10;
const int SkillXp = 1_000_000_000;

foreach (var noble in Hero.MainHero.Clan.Heroes) {
    if (noble == Hero.MainHero || !noble.IsAlive || noble.Age < 18) {
        continue;
    }

    Log.WriteLine(noble);

    for (int i = 0; i < (int)CharacterAttributesEnum.End; ++i) {
        var attr = (CharacterAttributesEnum)i;
        Log.WriteLine($"  {attr} = {AttributePoints}");
        noble.SetAttributeValue(attr, AttributePoints);
    }

    foreach (var skill in DefaultSkills.GetAllSkills()) {
        Log.WriteLine($"  {skill} += {SkillXp}");
        noble.AddSkillXp(skill, SkillXp);
    }

    Log.Write("  ");
    foreach (var perk in DefaultPerks.GetAllPerks()) {
        if (!noble.GetPerkValue(perk)) {
            Log.Write($"+{perk}, ");
            noble.SetPerkValue(perk, true);
        }
    }
    Log.WriteLine();
}
