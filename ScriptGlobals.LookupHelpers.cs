using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static readonly All All;

        public static IdLookup Id(string id) => new IdLookup(id);

        public static Hero MainHero => Hero.MainHero;

        public static LookupTable<Kingdom> Kingdoms => new(Kingdom.All);

        public static LookupTable<Clan> Clans => new(Clan.All);

        public static LookupTable<Hero> Heroes => new(Hero.All);

        public static LookupTable<Settlement> Settlements => new(Settlement.All);

        public static LookupTable<Town> Fiefs => new(Town.AllFiefs);

        public static LookupTable<Town> Towns => new(Town.AllTowns);

        public static LookupTable<Town> Castles => new(Town.AllCastles);

        public static LookupTable<Village> Villages => new(Village.All);

        public static LookupTable<MobileParty> Parties => new(MobileParty.All);
    }
}
