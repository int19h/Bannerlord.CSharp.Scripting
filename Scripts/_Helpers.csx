using System.IO;

static void WriteHero(this TextWriter writer, Hero hero, bool withAsc = true, bool withClan = false, bool withGovernor = false, bool withSpouse = false) {
    writer.Write($"{hero}");

    if (withClan) {
        writer.Write($" of {hero.Clan.Name}");
    }

    if (withAsc) {
        writer.Write($" ({hero.Age:N0}/");
        writer.Write(hero.IsFemale ? "F" : "M");
        if (hero.IsFemale && hero.IsFertile && hero.Age >= 18 && hero.Age <= 45) {
            writer.Write("+");
        }
        writer.Write($"/{hero.Culture})");
    }


    if (withGovernor && hero.GovernorOf != null) {
        writer.Write($", governor of {hero.GovernorOf}");
    }

    if (withSpouse && hero.Spouse != null) {
        writer.Write(", spouse of ");
        writer.WriteHero(hero.Spouse, withAsc: withAsc, withGovernor: withGovernor);
    }
}
