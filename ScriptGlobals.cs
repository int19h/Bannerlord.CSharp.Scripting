using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Int19h.Csx {
    public sealed class ScriptGlobals : IDisposable {
        public IReadOnlyList<string> Arguments { get; }

        public Log Log { get; }

        public ScriptGlobals(string scriptPath, IReadOnlyList<string> arguments) {
            Arguments = arguments;
            Log = new Log(Path.ChangeExtension(scriptPath, ".log"));
        }

        public void Dispose() {
            Log.Dispose();
        }
    }
}
