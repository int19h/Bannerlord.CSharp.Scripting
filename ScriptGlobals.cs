using System;
using System.Collections.Generic;
using System.IO;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public sealed partial class ScriptGlobals : IDisposable {
        private static Func<string> _defaultToString = new object().ToString;
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
    }
}
