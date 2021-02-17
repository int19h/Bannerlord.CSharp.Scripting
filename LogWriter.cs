using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public sealed class LogWriter : TextWriter {
        private readonly string? _defaultPath;
        private List<TextWriter> _writers = new();

        public LogWriter(string? defaultPath, TextWriter consoleWriter) {
            _defaultPath = defaultPath;
            _writers.Add(consoleWriter);
        }

        public void ToFile() {
            if (_defaultPath == null) {
                throw new InvalidOperationException("No default log path specified");
            }
            ToFile(_defaultPath);
        }

        public void ToFile(string path) {
            _writers.Add(File.CreateText(path));
        }

        public override Encoding Encoding => _writers[0].Encoding;

        protected override void Dispose(bool disposing) {
            if (disposing && _writers != null) {
                foreach (var writer in _writers.Skip(1)) {
                    writer.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        public override void Write(char value) {
            foreach (var writer in _writers) {
                writer.Write(value);
            }
        }

        public override void Write(char[] buffer, int index, int count) {
            foreach (var writer in _writers) {
                writer.Write(buffer, index, count);
            }
        }
    }
}
