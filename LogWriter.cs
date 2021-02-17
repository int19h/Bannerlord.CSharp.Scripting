using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public sealed class LogWriter : TextWriter {
        private List<TextWriter> writers = new();

        public LogWriter(TextWriter consoleWriter) {
            writers.Add(consoleWriter);
        }

        public void ToFile() {
            if (ScriptGlobals.ScriptPath == null) {
                throw new InvalidOperationException($"{nameof(ScriptGlobals.ScriptPath)} not specified");
            }
            ToFile(Path.ChangeExtension(ScriptGlobals.ScriptPath, ".log"));
        }

        public void ToFile(string path) {
            writers.Add(File.CreateText(path));
        }

        public override Encoding Encoding => writers[0].Encoding;

        protected override void Dispose(bool disposing) {
            if (disposing && writers != null) {
                foreach (var writer in writers.Skip(1)) {
                    writer.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        public override void Write(char value) {
            foreach (var writer in writers) {
                writer.Write(value);
            }
        }

        public override void Write(char[] buffer, int index, int count) {
            foreach (var writer in writers) {
                writer.Write(buffer, index, count);
            }
        }
    }
}
