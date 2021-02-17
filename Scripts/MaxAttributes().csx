#load "SetAttributes().csx"

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
