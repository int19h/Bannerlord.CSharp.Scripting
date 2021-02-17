using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public struct EnumerableWithLookup<T> : IEnumerable<T> {
        private readonly IEnumerable<T> source;

        public EnumerableWithLookup(IEnumerable<T> source) {
            this.source = source;
        }

        public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T this[string name] {
            get {
                var items = this.Where(item => $"{((dynamic?)item)?.Name}" == name).ToArray();
                if (items.Length == 1) {
                    return items[0];
                } else if (items.Length == 0) {
                    throw new KeyNotFoundException($"No {typeof(T).Name} named '{name}'");
                } else {
                    throw new KeyNotFoundException($"More than one {typeof(T).Name} named '{name}'");
                }
            }
        }

        public T[] this[params string[] names] {
            get {
                var self = this;
                return names.Select(name => self[name]).ToArray();
            }
        }
    }

    partial class ScriptGlobals {
        public static Hero Player => Hero.MainHero;

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
