using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static readonly All All;

        public static IdLookup Id(string id) => new IdLookup(id);

        public static Hero MainHero => Hero.MainHero;

        public static EnumerableWithLookup<Kingdom> Kingdoms => new(Kingdom.All);

        public static EnumerableWithLookup<Clan> Clans => new(Clan.All);

        public static EnumerableWithLookup<Hero> Heroes => new(Hero.All);

        public static EnumerableWithLookup<Settlement> Settlements => new(Settlement.All);

        public static EnumerableWithLookup<Town> Fiefs => new(Town.AllFiefs);

        public static EnumerableWithLookup<Town> Towns => new(Town.AllTowns);

        public static EnumerableWithLookup<Town> Castles => new(Town.AllCastles);

        public static EnumerableWithLookup<Village> Villages => new(Village.All);

        public static EnumerableWithLookup<MobileParty> Parties => new(MobileParty.All);
    }
}
