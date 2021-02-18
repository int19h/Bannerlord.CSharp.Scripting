using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public struct EnumerableWithLookup<T> : ILookupTable<T>
        where T : class {

        private readonly IEnumerable<T> source;

        public EnumerableWithLookup(IEnumerable<T> source) {
            this.source = source;
        }

        public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator T[](EnumerableWithLookup<T> en) => en.ToArray();

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
}
