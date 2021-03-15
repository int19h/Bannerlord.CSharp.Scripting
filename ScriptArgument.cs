using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public class ScriptArgument {
        protected readonly object?[] Values;

        protected ScriptArgument(object?[] values) {
            Trace.Assert(values.Length > 0);
            Values = values;
        }

        private static IEnumerable<object?> Flatten(object value) {
            switch (value) {
                case IEnumerable<object?> xs:
                    foreach (var x in xs) {
                        yield return x;
                    }
                    break;
                case ITuple tuple:
                    for (int i = 0; i < tuple.Length; ++i) {
                        foreach (var x in Flatten(tuple[i])) {
                            yield return x;
                        }
                    }
                    break;
                default:
                    yield return value;
                    break;
            }
        }

        public static object? Wrap(object? value) {
            if (value is null or All or IDataStore) {
                return value;
            }

            var items = Flatten(value).ToArray();
            dynamic arg =
                items.Length == 1 && items[0] is var x and not null ?
                new UnrestrictedScriptArgument.Scalar(x) :
                new UnrestrictedScriptArgument(items);
            foreach (dynamic? item in items) {
                arg = arg.RestrictBy(item);
            }
            return arg;
        }

        private static ValueTuple<TTarget>? Convert<TTarget>(object value) => null;

        private static ValueTuple<TTarget>? Convert<TTarget>(TTarget value) => ValueTuple.Create(value);

        protected static TTarget? LookUp<TTarget>(object? value) {
            if (value is null) {
                return (TTarget?)value;
            } else if (Convert((dynamic)value) is ValueTuple<TTarget> result) {
                return result.Item1;
            }

            if (LookupTables.Get<TTarget>() is var table and not null) {
                switch (value) {
                    case string s:
                        return table[s];
                    case Lookup lookup:
                        return table[lookup];
                }
            }

            throw new InvalidOperationException($"Invalid {typeof(TTarget).Name} lookup: {value}");
        }

        protected TTarget?[] LookUp<TTarget>()
            => Values.Select(value => LookUp<TTarget>(value)).ToArray();

        internal dynamic RestrictBy(object? o) => new ScriptArgument(Values);

        internal dynamic RestrictBy(string s) => this;

        internal dynamic RestrictBy(Lookup lookup) => this;

        public class Scalar : ScriptArgument {
            internal Scalar(object value) : base(new[] { value }) { }

            internal object Value => Values[0]!;
        }
    }

    public class ScriptArgument<T1> : ScriptArgument { 
        internal ScriptArgument(object?[] values) : base(values) { }

        public static implicit operator T1?[](ScriptArgument<T1> arg) => arg.LookUp<T1>();

        internal new dynamic RestrictBy(string s) => this;

        internal new dynamic RestrictBy(Lookup lookup) => this;

        internal dynamic RestrictBy(T1 item) => new ScriptArgument<T1>(Values);

        public new class Scalar : ScriptArgument.Scalar {
            public Scalar(object value) : base(value) { }

            public static implicit operator T1?(ScriptArgument<T1>.Scalar arg) => LookUp<T1>(arg.Value);

            public static implicit operator T1?[](ScriptArgument<T1>.Scalar arg) => arg.LookUp<T1>();
        }
    }

    public class ScriptArgument<T1, T2> : ScriptArgument<T1> {
        internal ScriptArgument(object?[] values) : base(values) { }

        public static implicit operator T2?[](ScriptArgument<T1, T2> arg) => arg.LookUp<T2>();

        internal new dynamic RestrictBy(string s) => this;

        internal new dynamic RestrictBy(Lookup lookup) => this;

        internal dynamic RestrictBy(T2 item) => new ScriptArgument<T2>(Values);

        public new class Scalar : ScriptArgument<T1>.Scalar {
            public Scalar(object value) : base(value) { }

            public static implicit operator T2?(ScriptArgument<T1, T2>.Scalar arg) => LookUp<T2>(arg.Value);

            public static implicit operator T2?[](ScriptArgument<T1, T2>.Scalar arg) => arg.LookUp<T2>();
        }
    }

    public class ScriptArgument<T1, T2, T3> : ScriptArgument<T1, T2> {
        internal ScriptArgument(object?[] values) : base(values) { }

        public static implicit operator T3?[](ScriptArgument<T1, T2, T3> arg) => arg.LookUp<T3>();

        internal new dynamic RestrictBy(string s) => this;

        internal new dynamic RestrictBy(Lookup lookup) => this;

        internal dynamic RestrictBy(T3 item) => new ScriptArgument<T3>(Values);

        public new class Scalar : ScriptArgument<T1, T2>.Scalar {
            public Scalar(object value) : base(value) { }

            public static implicit operator T3?(ScriptArgument<T1, T2, T3>.Scalar arg) => LookUp<T3>(arg.Value);

            public static implicit operator T3?[](ScriptArgument<T1, T2, T3>.Scalar arg) => arg.LookUp<T3>();
        }
    }

    public class ScriptArgument<T1, T2, T3, T4> : ScriptArgument<T1, T2, T3> {
        internal ScriptArgument(object?[] values) : base(values) { }

        public static implicit operator T4?[](ScriptArgument<T1, T2, T3, T4> arg) => arg.LookUp<T4>();

        internal new dynamic RestrictBy(string s) => this;

        internal new dynamic RestrictBy(Lookup lookup) => this;

        internal dynamic RestrictBy(T4 item) => new ScriptArgument<T4>(Values);

        public new class Scalar : ScriptArgument<T1, T2, T3>.Scalar {
            public Scalar(object value) : base(value) { }

            public static implicit operator T4?(ScriptArgument<T1, T2, T3, T4>.Scalar arg) => LookUp<T4>(arg.Value);

            public static implicit operator T4?[](ScriptArgument<T1, T2, T3, T4>.Scalar arg) => arg.LookUp<T4>();
        }
    }

    public class ScriptArgument<T1, T2, T3, T4, T5> : ScriptArgument<T1, T2, T3, T4> {
        internal ScriptArgument(object?[] values) : base(values) { }

        public static implicit operator T5?[](ScriptArgument<T1, T2, T3, T4, T5> arg) => arg.LookUp<T5>();

        internal new dynamic RestrictBy(string s) => this;

        internal new dynamic RestrictBy(Lookup lookup) => this;

        internal dynamic RestrictBy(T5 item) => new ScriptArgument<T5>(Values);

        public new class Scalar : ScriptArgument<T1, T2, T3, T4>.Scalar {
            public Scalar(object value) : base(value) { }

            public static implicit operator T5?(ScriptArgument<T1, T2, T3, T4, T5>.Scalar arg) => LookUp<T5>(arg.Value);

            public static implicit operator T5?[](ScriptArgument<T1, T2, T3, T4, T5>.Scalar arg) => arg.LookUp<T5>();
        }
    }

    public class ScriptArgument<T1, T2, T3, T4, T5, T6> : ScriptArgument<T1, T2, T3, T4, T5> {
        internal ScriptArgument(object?[] values) : base(values) { }

        public static implicit operator T6?[](ScriptArgument<T1, T2, T3, T4, T5, T6> arg) => arg.LookUp<T6>();

        internal new dynamic RestrictBy(string s) => this;

        internal new dynamic RestrictBy(Lookup lookup) => this;

        internal dynamic RestrictBy(T6 item) => new ScriptArgument<T6>(Values);

        public new class Scalar : ScriptArgument<T1, T2, T3, T4, T5>.Scalar {
            public Scalar(object value) : base(value) { }

            public static implicit operator T6?(ScriptArgument<T1, T2, T3, T4, T5, T6>.Scalar arg) => LookUp<T6>(arg.Value);

            public static implicit operator T6?[](ScriptArgument<T1, T2, T3, T4, T5, T6>.Scalar arg) => arg.LookUp<T6>();
        }
    }

    public class ScriptArgument<T1, T2, T3, T4, T5, T6, T7> : ScriptArgument<T1, T2, T3, T4, T5, T6> {
        internal ScriptArgument(object?[] values) : base(values) { }

        public static implicit operator T7?[](ScriptArgument<T1, T2, T3, T4, T5, T6, T7> arg) => arg.LookUp<T7>();

        internal new dynamic RestrictBy(string s) => this;

        internal new dynamic RestrictBy(Lookup lookup) => this;

        internal dynamic RestrictBy(T7 item) => new ScriptArgument<T7>(Values);

        public new class Scalar : ScriptArgument<T1, T2, T3, T4, T5, T6>.Scalar {
            public Scalar(object value) : base(value) { }

            public static implicit operator T7?(ScriptArgument<T1, T2, T3, T4, T5, T6, T7>.Scalar arg) => LookUp<T7>(arg.Value);

            public static implicit operator T7?[](ScriptArgument<T1, T2, T3, T4, T5, T6, T7>.Scalar arg) => arg.LookUp<T7>();
        }
    }

    public class ScriptArgument<T1, T2, T3, T4, T5, T6, T7, T8> : ScriptArgument<T1, T2, T3, T4, T5, T6, T7> {
        internal ScriptArgument(object?[] values) : base(values) { }

        public static implicit operator T8?[](ScriptArgument<T1, T2, T3, T4, T5, T6, T7, T8> arg) => arg.LookUp<T8>();

        internal new dynamic RestrictBy(string s) => this;

        internal new dynamic RestrictBy(Lookup lookup) => this;

        internal dynamic RestrictBy(T8 item) => new ScriptArgument<T8>(Values);

        public new class Scalar : ScriptArgument<T1, T2, T3, T4, T5, T6, T7>.Scalar {
            public Scalar(object value) : base(value) { }

            public static implicit operator T8?(ScriptArgument<T1, T2, T3, T4, T5, T6, T7, T8>.Scalar arg) => LookUp<T8>(arg.Value);

            public static implicit operator T8?[](ScriptArgument<T1, T2, T3, T4, T5, T6, T7, T8>.Scalar arg) => arg.LookUp<T8>();
        }
    }

    public class ScriptArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ScriptArgument<T1, T2, T3, T4, T5, T6, T7, T8> {
        internal ScriptArgument(object?[] values) : base(values) { }

        public static implicit operator T9?[](ScriptArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9> arg) => arg.LookUp<T9>();

        internal new dynamic RestrictBy(string s) => this;

        internal new dynamic RestrictBy(Lookup lookup) => this;

        internal dynamic RestrictBy(T9 item) => new ScriptArgument<T9>(Values);

        public new class Scalar : ScriptArgument<T1, T2, T3, T4, T5, T6, T7, T8>.Scalar {
            public Scalar(object value) : base(value) { }

            public static implicit operator T9?(ScriptArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Scalar arg) => LookUp<T9>(arg.Value);

            public static implicit operator T9?[](ScriptArgument<T1, T2, T3, T4, T5, T6, T7, T8, T9>.Scalar arg) => arg.LookUp<T9>();
        }
    }

    internal class UnrestrictedScriptArgument : ScriptArgument<Kingdom, Clan, Hero, Settlement, Town, Village, MobileParty, ItemObject> {
        internal UnrestrictedScriptArgument(object?[] values) : base(values) { }

        private dynamic Unrestricted<T>() => new ScriptArgument<T, Kingdom, Clan, Hero, Town, Village, MobileParty>(Values);

        internal new dynamic RestrictBy(string s) => Unrestricted<string>();

        internal dynamic RestrictBy(NameLookup lookup) => Unrestricted<NameLookup>();

        internal dynamic RestrictBy(IdLookup lookup) => Unrestricted<IdLookup>();

        internal dynamic RestrictBy<T>(T value) => new ScriptArgument<T>(Values);

        public new class Scalar : ScriptArgument<Kingdom, Clan, Hero, Settlement, Town, Village, MobileParty, ItemObject>.Scalar {
            public Scalar(object value) : base(value) { }

            private dynamic Unrestricted<T>() => new ScriptArgument<T, Kingdom, Clan, Hero, Settlement, Town, Village, MobileParty, ItemObject>.Scalar(Value);

            internal new dynamic RestrictBy(string s) => Unrestricted<string>();

            internal dynamic RestrictBy(NameLookup lookup) => Unrestricted<NameLookup>();

            internal dynamic RestrictBy(IdLookup lookup) => Unrestricted<IdLookup>();

            internal dynamic RestrictBy<T>(T value) => new ScriptArgument<T>.Scalar(Value);
        }
    }
}
