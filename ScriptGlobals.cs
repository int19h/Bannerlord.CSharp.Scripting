﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public static partial class ScriptGlobals {
        private static LogWriter? _log;

        public static dynamic Scripts = new Scripts();

        public static IReadOnlyList<string> Arguments { get; private set; } = new string[0];

        public static LogWriter Log {
            get => _log ?? throw new InvalidOperationException("Logging not available");
        }

        internal static void PrepareForEval(TextWriter consoleWriter) {
            Arguments = new string[0];
            _log = new LogWriter(null, consoleWriter);
        }

        internal static void PrepareForRun(string scriptPath, TextWriter consoleWriter, IReadOnlyList<string> arguments) {
            Arguments = arguments;
            _log = new LogWriter(Path.ChangeExtension(scriptPath, ".log"), consoleWriter);
        }

        internal static void Cleanup() {
            _log?.Dispose();
        }
    }
}
