using System;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public static partial class ScriptGlobals {
        public static readonly LogWriter Log = new LogWriter();

        public static readonly dynamic Scripts = new Scripts();

        public static readonly dynamic Shared = new ExpandoObject();

        public static string? ScriptPath { get; internal set; }

        public static void IgnoreVisibility(Expression<Action> expr) => expr.Compile()();

        public static T IgnoreVisibility<T>(Expression<Func<T>> expr) => expr.Compile()();

        public static T Get<T>(Lookup lookup) {
            var table = LookupTables.Get<T>();
            if (table == null) {
                throw new ArgumentException($"Unsupported type {typeof(T).Name}", nameof(T));
            }
            return table[lookup];
        }
        
        public static void Set<T>(out T target, T value) {
            target = value;
        }
    }
}
