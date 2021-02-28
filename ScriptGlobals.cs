using System;
using System.Dynamic;
using System.IO;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public static partial class ScriptGlobals {
        public static readonly LogWriter Log = new LogWriter();

        public static readonly dynamic Scripts = new Scripts();

        public static readonly dynamic Shared = new ExpandoObject();

        public static string? ScriptPath { get; internal set; }
    }
}
