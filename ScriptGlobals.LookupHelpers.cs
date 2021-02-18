using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static readonly All All;

        public static IdLookup Id(string id) => new IdLookup(id);

        public static LookupTable<Kingdom> Kingdoms => new(Kingdom.All);

        public static LookupTable<Clan> Clans => new(Clan.All);

        public static LookupTable<Hero> Heroes => new(Hero.All);

        public static LookupTable<Hero> Nobles => new(Heroes.Where(hero => hero.IsNoble));

        public static LookupTable<Hero> Wanderers => new(Heroes.Where(hero => hero.IsWanderer));

        public static LookupTable<Settlement> Settlements => new(Settlement.All);

        public static LookupTable<Town> Fiefs => new(Town.AllFiefs);

        public static LookupTable<Town> Towns => new(Town.AllTowns);

        public static LookupTable<Town> Castles => new(Town.AllCastles);

        public static LookupTable<Village> Villages => new(Village.All);

        public static LookupTable<MobileParty> Parties => new(MobileParty.All);

        public static IEnumerable<Hero> Descendants(this Hero hero) {
            foreach (var child in hero.Children) {
                yield return child;
                foreach (var descendant in child.Descendants()) {
                    yield return descendant;
                }
            }
        }

        public static IEnumerable<Hero> Friends(this Hero hero) => Heroes.Where(other => other.IsFriend(hero));

        public static IEnumerable<Hero> Enemies(this Hero hero) => Heroes.Where(other => other.IsEnemy(hero));
    }
}
