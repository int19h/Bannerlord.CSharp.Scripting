using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public interface ILookupTable<out T> : IReadOnlyCollection<T> {
        T this[int index] { get; }

        T this[Lookup lookup] { get; }

        ILookupTable<T> this[Predicate<T> include] { get; }
    }

    public static class LookupTable {
        public static ILookupTable<T> ToLookupTable<T>(this IEnumerable<T> items)
            where T : class
            => new LookupTable<T>(items);
    }

    public class LookupTable<T> : ILookupTable<T>
        where T : class {

        private readonly T[] items;

        public LookupTable(IEnumerable<T> items) {
            this.items = items.ToArray();
        }

        public LookupTable(LookupTable<T> other) {
            items = other.items;
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => items.Length;

        public T this[int index] => items[index];

        public T this[Lookup lookup] {
            get {
                var matching = this.Where(lookup.Matches).ToArray();
                if (matching.Length == 1) {
                    return matching[0];
                } else if (matching.Length == 0) {
                    throw new KeyNotFoundException($"No {typeof(T).Name} with {lookup.What} == '{lookup}'");
                } else {
                    throw new KeyNotFoundException($"More than one {typeof(T).Name} with {lookup.What} == '{lookup}'");
                }
            }
        }

        public ILookupTable<T> this[params Lookup[] lookups] {
            get {
                var self = this;
                return lookups.Select(lookup => self[lookup]).ToLookupTable();
            }
        }

        public ILookupTable<T> this[Predicate<T> include] =>
            this.Where(item => include(item)).ToLookupTable();
    }

    internal struct LookupTables {
        internal struct Getter<TItem> {
            public static Func<ILookupTable<TItem>?> Get = () => null;
        }

        static LookupTables() {
            Getter<Kingdom>.Get = () => ScriptGlobals.Kingdoms;
            Getter<Clan>.Get = () => ScriptGlobals.Clans;
            Getter<Hero>.Get = () => ScriptGlobals.Heroes;
            Getter<Settlement>.Get = () => ScriptGlobals.Settlements;
            Getter<Fief>.Get = () => ScriptGlobals.Fiefs;
            Getter<Town>.Get = () => ScriptGlobals.Fiefs;
            Getter<Village>.Get = () => ScriptGlobals.Villages;
            Getter<MobileParty>.Get = () => ScriptGlobals.Parties;
            Getter<ItemObject>.Get = () => ScriptGlobals.ItemObjects;
            Getter<PerkObject>.Get = () => ScriptGlobals.Perks;
            Getter<CharacterAttribute>.Get = () => ScriptGlobals.CharacterAttributes;
            Getter<TraitObject>.Get = () => ScriptGlobals.Traits;
            Getter<SkillObject>.Get = () => ScriptGlobals.Skills;
        }

        private static LookupTables Instance;

        private ILookupTable<TItem>? DoGet<TItem>() => Getter<TItem>.Get();

        public static ILookupTable<TItem>? Get<TItem>() => Instance.DoGet<TItem>();
    }
}
