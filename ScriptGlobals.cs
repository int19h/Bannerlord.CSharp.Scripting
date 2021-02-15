using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public sealed class ScriptGlobals : IDisposable {
        private readonly LogWriter? _log;

        public IReadOnlyList<string> Arguments { get; }

        public LogWriter Log => _log ?? throw new InvalidOperationException("Logging not available");

        public ScriptGlobals() {
            Arguments = new string[0];
        }

        public ScriptGlobals(string scriptPath, TextWriter consoleWriter, IReadOnlyList<string> arguments) {
            Arguments = arguments;
            _log = new LogWriter(Path.ChangeExtension(scriptPath, ".log"), consoleWriter);
        }

        public void Dispose() {
            _log?.Dispose();
        }

        public string Inspect(object o) {
            StringBuilder output = new();

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

            var props = o.GetType().GetProperties().OrderBy(prop => prop.Name);
            foreach (var prop in props) {
                if (prop.GetIndexParameters().Length != 0) {
                    continue;
                }

                object value;
                try {
                    value = prop.GetValue(o);
                } catch (Exception ex) {
                    value = $"<error: {ex.Message}>";
                }

                if (value is ICollection coll) {
                    try {
                        value = $"[{coll.Count} items]";
                    } catch (Exception) {
                    }
                }
                if (value is null) {
                    value = "null";
                } else if (value is IEnumerable and not string) {
                    value = "[items]";
                }

                output.Append(prop.Name);
                output.Append(": ");
                AppendType(prop.PropertyType);
                output.Append(" = ");
                output.Append(value);
                output.AppendLine();
            }

            return output.ToString();
        }

        public void Edit(params object[] objects) {
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
