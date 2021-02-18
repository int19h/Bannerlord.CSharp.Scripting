using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using static Int19h.Bannerlord.CSharp.Scripting.Helpers;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public static class ScriptArgument {
        private static ScalarScriptArgument<T> Create<T>(T value) where T : notnull
            => new(value);

        public static dynamic? Wrap(dynamic? value) {
            if (value is null) {
                return null;
            } else if (value is string or Lookup) {
                return Create(value);
            } else if (value is ITuple tuple) {
                dynamic arg = Create(value);
                for (int i = 0; i < tuple.Length; ++i) {
                    dynamic item = tuple[i];
                    if (item is null) {
                        arg = arg.RestrictByNull();
                    } else if (item is IEnumerable<object> xs) {
                        foreach (dynamic x in xs) {
                            arg = arg.RestrictBy(x);
                        }
                    } else {
                        arg = arg.RestrictBy(item);
                    }
                }
                return arg;
            } else {
                return value;
            }
        }
    }

    public class ScriptArgument<T>
        where T : notnull {


        public readonly T Value;

        public ScriptArgument(T value) {
            Value = value;
        }

        protected static TTarget LookUp<TTarget>(dynamic value) {
            if (IsAssignableTo<TTarget>(value)) {
                return value;
            }

            if (LookupTables.Get<TTarget>() is var table and not null) {
                switch (value) {
                    case string s:
                        return table[s];
                    case Lookup lookup:
                        return table[lookup];
                }
            }

            throw new InvalidCastException();
        }

        protected static TTarget[] LookUpMany<TTarget>(object value) {
            if (value is not ITuple tuple || LookupTables.Get<TTarget>() == null) {
                return new[] { LookUp<TTarget>(value) };
            }

            IEnumerable<TTarget> Unpack() {
                for (int i = 0; i < tuple.Length; ++i) {
                    var item = tuple[i];
                    if (item is IEnumerable<TTarget> xs) {
                        foreach (var x in xs) {
                            yield return x;
                        }
                    } else {
                        yield return LookUp<TTarget>(item);
                    }
                }
            }

            return Unpack().ToArray();
        }

        public static implicit operator T[](ScriptArgument<T> arg) => LookUpMany<T>(arg.Value);

        internal ScriptArgument<T> RestrictBy<TOther>(TOther item) => new(Value);

        internal ScriptArgument<T> RestrictBy(string s) => this;

        internal ScriptArgument<T> RestrictBy(Lookup lookup) => this;

        internal ScriptArgument<T> RestrictByNull() => this;
    }

    public class ScriptArgument<T, T1> : ScriptArgument<T>
        where T : notnull {

        public ScriptArgument(T value) : base(value) { }

        public static implicit operator T1[](ScriptArgument<T, T1> arg) => LookUpMany<T1>(arg.Value);

        internal ScriptArgument<T, T1> RestrictBy(T1 item) => new(Value);
    }

    public class ScriptArgument<T, T1, T2> : ScriptArgument<T, T1>
        where T : notnull {

        public ScriptArgument(T value) : base(value) { }

        public static implicit operator T2[](ScriptArgument<T, T1, T2> arg) => LookUpMany<T2>(arg.Value);

        internal ScriptArgument<T, T2> RestrictBy(T2 item) => new(Value);
    }

    public class ScriptArgument<T, T1, T2, T3> : ScriptArgument<T, T1, T2>
        where T : notnull {

        public ScriptArgument(T value) : base(value) { }

        public static implicit operator T3[](ScriptArgument<T, T1, T2, T3> arg) => LookUpMany<T3>(arg.Value);

        internal ScriptArgument<T, T3> RestrictBy(T3 item) => new(Value);
    }

    public class ScriptArgument<T, T1, T2, T3, T4> : ScriptArgument<T, T1, T2, T3>
        where T : notnull {

        public ScriptArgument(T value) : base(value) { }

        public static implicit operator T4[](ScriptArgument<T, T1, T2, T3, T4> arg) => LookUpMany<T4>(arg.Value);

        internal ScriptArgument<T, T4> RestrictBy(T4 item) => new(Value);
    }

    public class ScriptArgument<T, T1, T2, T3, T4, T5> : ScriptArgument<T, T1, T2, T3, T4>
        where T : notnull {

        public ScriptArgument(T value) : base(value) { }

        public static implicit operator T5[](ScriptArgument<T, T1, T2, T3, T4, T5> arg) => LookUpMany<T5>(arg.Value);

        internal ScriptArgument<T, T5> RestrictBy(T5 item) => new(Value);
    }

    public class ScriptArgument<T, T1, T2, T3, T4, T5, T6> : ScriptArgument<T, T1, T2, T3, T4, T5>
        where T : notnull {

        public ScriptArgument(T value) : base(value) { }

        public static implicit operator T6[](ScriptArgument<T, T1, T2, T3, T4, T5, T6> arg) => LookUpMany<T6>(arg.Value);

        internal ScriptArgument<T, T6> RestrictBy(T6 item) => new(Value);
    }

    public class ScriptArgument<T, T1, T2, T3, T4, T5, T6, T7> : ScriptArgument<T, T1, T2, T3, T4, T5, T6>
        where T : notnull {

        public ScriptArgument(T value) : base(value) { }

        public static implicit operator T7[](ScriptArgument<T, T1, T2, T3, T4, T5, T6, T7> arg) => LookUpMany<T7>(arg.Value);

        internal ScriptArgument<T, T7> RestrictBy(T7 item) => new(Value);
    }

    public class UniversalScriptArgument<T> : ScriptArgument<T, Kingdom, Clan, Hero, Town, Village, MobileParty>
        where T : notnull {
        public UniversalScriptArgument(T value) : base(value) { }
    }

    public class ScalarScriptArgument<T> : UniversalScriptArgument<T>
        where T : notnull {

        public ScalarScriptArgument(T value) : base(value) { }

        public static implicit operator T(ScalarScriptArgument<T> arg) => arg.Value;

        public static implicit operator Kingdom(ScalarScriptArgument<T> arg) => ((Kingdom[])arg)[0];

        public static implicit operator Clan(ScalarScriptArgument<T> arg) => ((Clan[])arg)[0];

        public static implicit operator Hero(ScalarScriptArgument<T> arg) => ((Hero[])arg)[0];

        public static implicit operator Town(ScalarScriptArgument<T> arg) => ((Town[])arg)[0];

        public static implicit operator Village(ScalarScriptArgument<T> arg) => ((Village[])arg)[0];

        public static implicit operator MobileParty(ScalarScriptArgument<T> arg) => ((MobileParty[])arg)[0];

        internal new UniversalScriptArgument<T> RestrictBy(string s) => new(Value);

        internal new UniversalScriptArgument<T> RestrictBy(Lookup lookup) => new(Value);

        internal new UniversalScriptArgument<T> RestrictByNull() => new(Value);
    }
}
