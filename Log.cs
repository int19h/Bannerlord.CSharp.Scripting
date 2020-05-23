using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Int19h.Csx {
    public sealed class Log : TextWriter {
        private readonly string _path;
        private readonly StringWriter _consoleWriter = new StringWriter();
        private TextWriter[] _writers;

        public Log(string path) {
            _path = path;
        }

        internal string Output => _consoleWriter.ToString();

        internal bool IsEmpty => _consoleWriter.GetStringBuilder().Length == 0;

        private TextWriter[] Writers {
            get {
                if (_writers == null) {
                    var fileWriter = File.CreateText(_path);
                    _writers = new TextWriter[] { _consoleWriter, fileWriter };
                }
                return _writers;
            }
        }

        public override Encoding Encoding => _consoleWriter.Encoding;

        protected override void Dispose(bool disposing) {
            if (disposing && _writers != null) {
                foreach (var writer in _writers) {
                    writer.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        public override void Write(char value) {
            foreach (var writer in Writers) {
                writer.Write(value);
            }
        }

        public override void Write(char[] buffer, int index, int count) {
            foreach (var writer in Writers) {
                writer.Write(buffer, index, count);
            }
        }
    }
}
