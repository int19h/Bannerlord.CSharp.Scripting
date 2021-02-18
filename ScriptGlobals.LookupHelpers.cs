using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static readonly All All;

        public static IdLookup Id(string id) => new IdLookup(id);

        public static ILookupTable<Kingdom> Kingdoms => Kingdom.All.ToLookupTable();

        public static ILookupTable<Clan> Clans => Clan.All.ToLookupTable();

        public static ILookupTable<Hero> Heroes => Hero.All.ToLookupTable();

        public static ILookupTable<Hero> Nobles => Heroes[hero => hero.IsNoble];

        public static ILookupTable<Hero> Wanderers => Heroes[hero => hero.IsWanderer];

        public static ILookupTable<Settlement> Settlements => Settlement.All.ToLookupTable();

        public static ILookupTable<Town> Fiefs => Town.AllFiefs.ToLookupTable();

        public static ILookupTable<Town> Towns => Town.AllTowns.ToLookupTable();

        public static ILookupTable<Town> Castles => Town.AllCastles.ToLookupTable();

        public static ILookupTable<Village> Villages => Village.All.ToLookupTable();

        public static ILookupTable<MobileParty> Parties => MobileParty.All.ToLookupTable();

        public static ILookupTable<Hero> Descendants(this Hero hero) {
            IEnumerable<Hero> GetDescendants(Hero hero) {
                foreach (var child in hero.Children) {
                    yield return child;
                    foreach (var descendant in GetDescendants(hero)) {
                        yield return descendant;
                    }
                }
            }
            return GetDescendants(hero).ToLookupTable();
        }

        public static ILookupTable<Hero> Friends(this Hero hero) => Heroes[other => other.IsFriend(hero)];

        public static ILookupTable<Hero> Enemies(this Hero hero) => Heroes[other => other.IsEnemy(hero)];
    }
}
