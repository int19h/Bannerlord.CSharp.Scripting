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
