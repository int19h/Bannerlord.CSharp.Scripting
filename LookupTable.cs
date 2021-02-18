using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public interface ILookupTable<out T> : IEnumerable<T> {
        T this[Lookup lookup] { get; }
    }

    public struct LookupTable<T> : ILookupTable<T>
        where T : class {

        private readonly IEnumerable<T> source;

        public LookupTable(IEnumerable<T> source) {
            this.source = source;
        }

        public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator T[](LookupTable<T> en) => en.ToArray();

        public T this[Lookup lookup] {
            get {
                var items = this.Where(lookup.Matches).ToArray();
                if (items.Length == 1) {
                    return items[0];
                } else if (items.Length == 0) {
                    throw new KeyNotFoundException($"No {typeof(T).Name} with {lookup.What} == '{lookup}'");
                } else {
                    throw new KeyNotFoundException($"More than one {typeof(T).Name} with {lookup.What} == '{lookup}'");
                }
            }
        }

        public T[] this[params Lookup[] lookups] {
            get {
                var self = this;
                return lookups.Select(lookup => self[lookup]).ToArray();
            }
        }
    }

    internal struct LookupTables {
        internal struct Getter<TItem> {
            public static Func<ILookupTable<TItem>?> Get = () => null;
        }

        static LookupTables() {
            Getter<Kingdom>.Get = () => ScriptGlobals.Kingdoms;
            Getter<Clan>.Get = () => ScriptGlobals.Clans;
            Getter<Hero>.Get = () => ScriptGlobals.Heroes;
            Getter<Fief>.Get = () => ScriptGlobals.Fiefs;
            Getter<Town>.Get = () => ScriptGlobals.Fiefs;
            Getter<Village>.Get = () => ScriptGlobals.Villages;
            Getter<MobileParty>.Get = () => ScriptGlobals.Parties;
        }

        private static LookupTables Instance;

        private ILookupTable<TItem>? DoGet<TItem>() => Getter<TItem>.Get();

        public static ILookupTable<TItem>? Get<TItem>() => Instance.DoGet<TItem>();
    }
}
