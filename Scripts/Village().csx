void SetHearths(Village[] villages, int hearths = 1000) {
    foreach (var village in villages) {
        Log.WriteLine($"{village}: {village.Hearth} -> {Hearth}");
        village.Hearth = hearths;
    }
}

void AddHearths(Village[] villages, int hearths = 1000) {
    foreach (var village in villages) {
        var newHearth = village.Hearth + hearths;
        Log.WriteLine($"{village}: {village.Hearth} -> {newHearth}");
        village.Hearth = newHearth;
    }
}
