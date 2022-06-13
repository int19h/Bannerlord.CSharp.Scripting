using System.Collections;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using static System.FormattableString;

delegate object Getter(object obj);

void Snapshot() {
    int total = 0;
    using (var output = new StreamWriter("C:/Temp/snapshot.json")) {
        var idgen = new ObjectIDGenerator();
        var queue = new Queue<object>(new[] { Campaign.Current });
        var seen = new HashSet<object>();
        var types = new Dictionary<Type, Dictionary<string, Getter>>();

        void Enqueue(object obj) {
            var ns = obj.GetType().Namespace ?? "";
            if (!ns.StartsWith("TaleWorlds") && !ns.StartsWith("SandBox")) {
                return;
            }
            if (!seen.Add(obj)) {
                return;
            }
            queue.Enqueue(obj);
        }

        string GetId(object obj) {
            Enqueue(obj);
            switch (obj) {
            case MBObjectBase mbref:
                return "@" + (string.IsNullOrEmpty(mbref.StringId) ? mbref.Id.ToString() : mbref.StringId);
            case var oref:
                return "@@" + idgen.GetId(oref, out _);
            }
        }

        Dictionary<string, Getter> GettersOf(Type t) {
            if (types.TryGetValue(t, out var getters)) {
                return getters;
            }

            types[t] = getters = new Dictionary<string, Getter>();
            while (!t.IsPublic && !t.IsNestedPublic) {
                t = t.BaseType;
            }
            if (!t.IsValueType) {
                getters["class"] = (_ => t.Name);
            }

            var props = t
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .DistinctBy(prop => prop.Name);
            foreach (var prop in props) {
                if (!prop.CanRead || prop.DeclaringType == typeof(object) ||
                    prop.GetIndexParameters().Length != 0
                ) {
                    continue;
                }

                var param = Expression.Parameter(typeof(object));
                var getter = Expression.Lambda<Getter>(
                    Expression.Convert(
                        Expression.Property(Expression.Convert(param, t), prop),
                        typeof(object)
                    ), param
                ).Compile();
                getters[prop.Name] = getter;
            }
            return getters;
        }

        void Write(object value) {
            switch (value) {
            case null:
                output.Write("null");
                break;
            case true:
                output.Write("true");
                break;
            case false:
                output.Write("false");
                break;
            case byte _:
            case sbyte _:
            case short _:
            case ushort _:
            case int _:
            case uint _:
            case long _:
            case ulong _:
            case float _:
            case double _:
            case decimal _:
                output.Write(Invariant($"{value}"));
                break;
            case string s:
                WriteString(s);
                break;
            case Enum _:
            case TextObject _:
            case MBGUID _:
                WriteString(value.ToString());
                break;
            case IEnumerable seq:
                WriteSequence(seq.Cast<object>());
                break;
            case ValueType _:
                WriteObject(value);
                break;
            default:
                WriteString(GetId(value));
                break;
            }
        }

        void WriteString(string s) {
            output.Write('"');

            foreach (var ch in s) {
                switch (ch) {
                    case '\t':
                        output.Write(@"\t");
                        continue;
                    case '\n':
                        output.Write(@"\n");
                        continue;
                    case '\r':
                        output.Write(@"\r");
                        continue;
                    case '"':
                        output.Write(@"\""");
                        continue;
                    case '\\':
                        output.Write(@"\\");
                        continue;
                }

                var n = (int)ch;
                if (n < 32 || n > 127) {
                    output.Write(@"\u{0:X4}", n);
                } else {
                    output.Write(ch);
                }
            }

            output.Write('"');
        }

        void WriteKey(string s) {
            WriteString(s);
            output.Write(':');
        }

        void WriteObject(object obj) {
            ++total;
            output.Write('{');

            var getters = GettersOf(obj.GetType());

            bool needComma = false;
            foreach (var prop in getters) {
                object value;
                try {
                    value = prop.Value(obj);
                } catch (Exception) {
                    continue;
                }

                if (needComma) {
                    output.Write(',');
                } else {
                    needComma = true;
                }
                WriteKey(prop.Key);
                Write(value);
            }

            output.Write('}');
        }

        void WriteSequence(IEnumerable<object> seq) {
            output.Write('[');

            object[] items;
            try {
                items = seq.ToArray();
            } catch (Exception) {
                items = new object[0];
            }

            bool needComma = false;
            foreach (var item in items) {
                if (needComma) {
                    output.Write(',');
                } else {
                    needComma = true;
                }
                Write(item);
            }

            output.Write(']');
        }

        output.Write("{\n");
        for (;;) {
            var obj = queue.Dequeue();
            WriteKey(GetId(obj));
            WriteObject(obj);
            if (!queue.Any()) {
                break;
            }
            output.Write(",\n");
        }
        output.Write("}\n");
    }

    Log.WriteLine($"{total} objects written.");
}