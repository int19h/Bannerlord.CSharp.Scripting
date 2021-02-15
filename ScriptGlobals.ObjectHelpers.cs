using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        private static Func<string> _defaultToString = new object().ToString;

        public static string Dump(object o) {
            if (o == null) {
                return "null";
            }

            var output = new StringBuilder();

            bool HasDefaultToString(object value) {
                Func<string> toString = value.ToString;
                return toString.Method == _defaultToString.Method;
            }

            void AppendType(Type type) {
                var typeArgs = type.GetGenericArguments();
                if (!typeArgs.Any()) {
                    output.Append(type.Name);
                    return;
                }

                var typeName = type.Name;
                var backtick = typeName.LastIndexOf('`');
                if (backtick >= 0) {
                    typeName = typeName.Remove(backtick);
                }
                output.Append(typeName);

                output.Append('<');
                foreach (var typeArg in typeArgs) {
                    AppendType(typeArg);
                }
                output.Append('>');
            }

            void AppendCast(object value) {
                output.Append("(");
                AppendType(value.GetType());
                output.Append(") ");
            }

            void AppendStringLiteral(string s) {
                output.Append("'");
                foreach (var c in s) {
                    if (c == '\n') {
                        output.Append("\\n");
                    } else if (c == '"' || c == '\'' || c == ';' || c < 32 || c > 127) {
                        output.Append($"\\u{(ushort)c:X04}");
                    } else {
                        output.Append(c);
                    }
                }
                output.Append("'");
            }

            void AppendValue(Func<object> getter) {
                object value;
                try {
                    value = getter();
                } catch (Exception ex) {
                    output.Append($"<error: {ex.Message}>");
                    return;
                }

                if (value is null) {
                    output.Append("null");
                } else if (value is sbyte or short or int or long or byte or ushort or uint or ulong or float or double or decimal) {
                    output.Append(value);
                } else if (value is bool b) {
                    output.Append(b ? "true" : "false");
                } else if (value is char c) {
                    AppendStringLiteral(new string(c, 1));
                    output.Append("[0]");
                } else if (value is string s) {
                    AppendStringLiteral(s);
                } else if (value is IEnumerable en) {
                    int? count = null;
                    if (en is ICollection coll) {
                        try {
                            count = coll.Count;
                        } catch (Exception) {
                        }
                    }

                    AppendCast(value);
                    if (count is int n) {
                        output.Append($"[{n} items]");
                    } else {
                        output.Append("[...]");
                    }
                } else {
                    string? repr = null;
                    if (!HasDefaultToString(value)) {
                        try {
                            repr = value.ToString();
                        } catch (Exception) {
                        }
                    }

                    AppendCast(value);
                    if (repr != null) {
                        AppendStringLiteral(repr);
                    } else {
                        output.Append("...");
                    }
                }
            }

            var type = o.GetType();
            output.Append(": ");
            AppendType(type);
            output.AppendLine(" = {");

            var props = type.GetProperties().OrderBy(prop => prop.Name);
            foreach (var prop in props) {
                if (prop.GetIndexParameters().Length != 0) {
                    continue;
                }

                output.Append("  ");
                output.Append(prop.Name);
                output.Append(": ");
                AppendType(prop.PropertyType);
                output.Append(" = ");
                AppendValue(() => prop.GetValue(o));
                output.AppendLine(",");
            }

            if (o is IEnumerable items and not string) {
                int i = 0;
                foreach (var item in items) {
                    output.Append($"  [{i}] = ");
                    AppendValue(() => item);
                    output.AppendLine(",");
                    if (++i >= 1000) {
                        output.AppendLine("  ...");
                        break;
                    }
                }
            }

            output.Append("}");
            return output.ToString();
        }

        public static void Edit(params object[] objects) {
            var uiThread = new Thread(() => {
                var grid = new PropertyGrid {
                    Dock = DockStyle.Fill,
                    SelectedObjects = objects,
                };
                var form = new Form {
                    Controls = { grid },
                    Text = objects.Length == 1 ? $"{objects[0]}" : $"{objects.Length} objects",
                };
                Application.Run(form);
            });
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
            uiThread.Join();
        }
    }
}
