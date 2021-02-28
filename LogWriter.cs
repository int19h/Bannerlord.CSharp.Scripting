using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public sealed class LogWriter : TextWriter {
        private List<TextWriter> writers = new();

        private sealed class RemoveOnDispose : IDisposable {
            private readonly LogWriter log;
            private readonly TextWriter writer;

            public RemoveOnDispose(LogWriter log, TextWriter writer) {
                this.log = log;
                this.writer = writer;
            }

            public void Dispose() {
                log.RemoveWriter(writer);
                writer.Flush();
            }
        }

        internal IDisposable WithWriter(TextWriter writer) {
            AddWriter(writer);
            return new RemoveOnDispose(this, writer);
        }

        internal void AddWriter(TextWriter writer) {
            writers.Add(writer);
        }

        internal void RemoveWriter(TextWriter writer) {
            writer.Flush();
            writers.Remove(writer);
        }

        public override Encoding Encoding => writers[0].Encoding;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                foreach (var writer in writers) {
                    writer.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public override void Flush() {
            base.Flush();
            foreach (var writer in writers) {
                writer.Flush();
            }
        }

        public override void Write(char value) {
            foreach (var writer in writers) {
                writer.Write(value);
            }
            Flush();
        }

        public override void Write(char[] buffer, int index, int count) {
            foreach (var writer in writers) {
                writer.Write(buffer, index, count);
            }
            Flush();
        }
    }
}
