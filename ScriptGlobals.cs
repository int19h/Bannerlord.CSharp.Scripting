using System;
using System.Collections.Generic;
using System.IO;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public static partial class ScriptGlobals {
        private static LogWriter? _log;

        public static readonly dynamic Scripts = new Scripts();

        public static string? ScriptPath { get; internal set; }

        public static IReadOnlyList<string> Arguments { get; private set; } = new string[0];

        public static LogWriter Log {
            get => _log ?? throw new InvalidOperationException("Logging not available");
        }

        internal static void Prepare(TextWriter consoleWriter, string? scriptPath, IReadOnlyList<string>? arguments) {
            Arguments = arguments ?? new string[0];
            ScriptGlobals.ScriptPath = scriptPath;
            _log = new LogWriter(consoleWriter);
        }

        internal static void Cleanup() {
            _log?.Dispose();
            _log = null;
        }
    }
}
