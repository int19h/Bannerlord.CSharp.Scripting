using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public class Lookups {
        private readonly ITuple tuple;

        public Lookups(ITuple tuple) {
            this.tuple = tuple;
        }

        public override string ToString() => tuple.ToString();

        public static implicit operator Kingdom[](Lookups ls) => ls.Expand(ScriptGlobals.Kingdoms).ToArray();

        public static implicit operator Clan[](Lookups ls) => ls.Expand(ScriptGlobals.Clans).ToArray();

        public static implicit operator Hero[](Lookups ls) => ls.Expand(ScriptGlobals.Heroes).ToArray();

        public static implicit operator Settlement[](Lookups ls) => ls.Expand(ScriptGlobals.Settlements).ToArray();

        public static implicit operator Town[](Lookups ls) => ls.Expand(ScriptGlobals.Fiefs).ToArray();

        public static implicit operator Village[](Lookups ls) => ls.Expand(ScriptGlobals.Villages).ToArray();

        public static implicit operator MobileParty[](Lookups ls) => ls.Expand(ScriptGlobals.Parties).ToArray();

        private IEnumerable<T> Expand<T>(EnumerableWithLookup<T> source) {
            for (int i = 0; i < tuple.Length; ++i) {
                switch (tuple[i]) {
                    case string s: {
                            yield return source[s];
                            break;
                        }
                    case Lookup lookup: {
                            yield return source[lookup];
                            break;
                        }
                    case T x: {
                            yield return x;
                            break;
                        }
                    case IEnumerable<T> xs: {
                            foreach (var x in xs) {
                                yield return x;
                            }
                            break;
                        }
                    case null:
                        throw new NullReferenceException($"Expected {typeof(T).Name}, but got null");
                    default:
                        throw new InvalidCastException($"Expected {typeof(T).Name}, but got {tuple[i].GetType().Name}");
                }
            }
        }
    }
}
