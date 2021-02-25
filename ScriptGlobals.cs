﻿using System;
using System.Dynamic;
using System.IO;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public static partial class ScriptGlobals {
        private static LogWriter? log;

        public static readonly dynamic Scripts = new Scripts();

        public static readonly dynamic Shared = new ExpandoObject();

        public static string? ScriptPath { get; internal set; }

        public static LogWriter Log {
            get => log ?? throw new InvalidOperationException("Logging not available");
        }

        internal static void Prepare(TextWriter consoleWriter, string? scriptPath) {
            ScriptGlobals.ScriptPath = scriptPath;
            log = new LogWriter(consoleWriter);
        }

        internal static void Cleanup() {
            log?.Dispose();
            log = null;
        }
    }
}
