using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static readonly All All;

        public static IdLookup Id(string id) => new IdLookup(id);

        public static ILookupTable<Kingdom> Kingdoms => Game.Current.ObjectManager.GetObjectTypeList<Kingdom>().ToLookupTable();

        public static ILookupTable<Clan> Clans => Game.Current.ObjectManager.GetObjectTypeList<Clan>().ToLookupTable();

        public static ILookupTable<Hero> Heroes => Game.Current.ObjectManager.GetObjectTypeList<Hero>().ToLookupTable();

        public static ILookupTable<Hero> Nobles => Heroes[hero => hero.IsNoble];

        public static ILookupTable<Hero> Wanderers => Heroes[hero => hero.IsWanderer];

        public static ILookupTable<Settlement> Settlements => Game.Current.ObjectManager.GetObjectTypeList<Settlement>().ToLookupTable();

        public static ILookupTable<Town> Fiefs => Town.AllFiefs.ToLookupTable();

        public static ILookupTable<Town> Towns => Town.AllTowns.ToLookupTable();

        public static ILookupTable<Town> Castles => Town.AllCastles.ToLookupTable();

        public static ILookupTable<Village> Villages => Village.All.ToLookupTable();

        public static ILookupTable<MobileParty> Parties => Game.Current.ObjectManager.GetObjectTypeList<MobileParty>().ToLookupTable();

        public static ILookupTable<ItemObject> Items => Game.Current.ObjectManager.GetObjectTypeList<ItemObject>().ToLookupTable();

        public static IEnumerable<PerkObject> Perks => Game.Current.ObjectManager.GetObjectTypeList<PerkObject>().ToLookupTable();

        public static IEnumerable<CharacterAttribute> Attributes => Game.Current.ObjectManager.GetObjectTypeList<CharacterAttribute>().ToLookupTable();

        public static IEnumerable<TraitObject> Traits => Game.Current.ObjectManager.GetObjectTypeList<TraitObject>().ToLookupTable();

        public static IEnumerable<SkillObject> Skills => Game.Current.ObjectManager.GetObjectTypeList<SkillObject>().ToLookupTable();

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
