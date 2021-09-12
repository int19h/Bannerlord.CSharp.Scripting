using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using TaleWorlds.Library;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public static class Commands {
        private static ScriptState? evalState = null;
        private static TextWriter? loggingTo = null;

        private static string WithErrorHandling(Action<TextWriter> body) {
            using (var output = new System.IO.StringWriter()) {
                try {
                    body(output);
                } catch (CommandException ex) {
                    output.WriteLine(ex.Message);
                } catch (CompilationErrorException ex) {
                    foreach (var diag in ex.Diagnostics) {
                        output.WriteLine(diag);
                    }
                } catch (Exception ex) {
                    output.WriteLine(ex);
                }
                return output.ToString();
            }
        }

        private static string ToCode(IEnumerable<string> args) =>
            string.Join(" ", args).Replace('\'', '"').Replace(".,", ";");

        [CommandLineFunctionality.CommandLineArgumentFunction("help", "csx")]
        public static string Help(List<string> args) => WithErrorHandling(output => {
            if (args.Count != 0) {
                throw new CommandException("Usage: csx.help");
            }
            try {
                Process.Start("https://github.com/int19h/csx/blob/master/README.md");
            } catch (Exception) {
            }
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("list", "csx")]
        public static string List(List<string> args) => WithErrorHandling(output => {
            if (args.Count != 0) {
                throw new CommandException("Usage: csx.list");
            }

            foreach (var path in Scripts.GetSearchPaths()) {
                string[] fileNames;
                try {
                    fileNames = Directory.GetFiles(path, "*.csx");
                } catch (Exception) {
                    continue;
                }

                output.WriteLine($"@ {path}:");
                foreach (var fileName in fileNames) {
                    var scriptName = Path.GetFileNameWithoutExtension(fileName);
                    output.WriteLine($"  {scriptName}");
                }
                output.WriteLine();
            }
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("reset", "csx")]
        public static string Reset(List<string> args) => WithErrorHandling(output => {
            if (args.Count != 0) {
                throw new CommandException("Usage: csx.reset");
            }

            var script = CSharpScript.Create("", Scripts.GetScriptOptions());
            Scripts.IgnoreVisibility(script);
            evalState = script.RunAsync().GetAwaiter().GetResult();
            output.Write("Script state reset.");
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("log_to", "csx")]
        public static string LogTo(List<string> args) => WithErrorHandling(output => {
            if (args.Count != 1) {
                throw new CommandException("Usage: csx.log_to {<filename> | -}");
            }

            if (loggingTo is not null) {
                ScriptGlobals.Log.RemoveWriter(loggingTo);
                loggingTo.Dispose();
                loggingTo = null;
            }

            var fileName = args[0];
            if (fileName != "-") {
                loggingTo = File.CreateText(fileName);
                ScriptGlobals.Log.AddWriter(loggingTo);
            }
        });

        private static void Eval(IEnumerable<string> args, TextWriter output) {
            if (evalState == null) {
                Reset(new());
                if (evalState == null) {
                    throw new NullReferenceException();
                }
            }

            var code = ToCode(args);
            using (ScriptGlobals.Log.WithWriter(output)) {
                var script = evalState.Script.ContinueWith(code);
                Scripts.IgnoreVisibility(script);
                evalState = script.RunFromAsync(evalState).GetAwaiter().GetResult();
            }
            output.Write(evalState.ReturnValue);
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("eval", "csx")]
        public static string Eval(List<string> args) => WithErrorHandling(output => {
            if (args.Count < 1) {
                throw new CommandException("Usage: csx.eval {<expression> | <statement>.,}");
            }
            Eval(args, output);
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("dump", "csx")]
        public static string Dump(List<string> args) => WithErrorHandling(output => {
            if (args.Count < 1) {
                throw new CommandException("Usage: csx.dump <expression>");
            }
            Eval(args.Prepend("Dump(").Append(")"), output);
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("edit", "csx")]
        public static string Edit(List<string> args) => WithErrorHandling(output => {
            if (args.Count < 1) {
                throw new CommandException("Usage: csx.edit <expression>");
            }
            Eval(args.Prepend("Edit(").Append(")"), output);
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("run", "csx")]
        public static string Run(List<string> args) => WithErrorHandling(output => {
            if (args.Count < 1) {
                throw new CommandException("Usage: csx.run <script>[.<function>]([<args>...])");
            }
            Eval(args.Prepend("Scripts."), output);
        });
    }

    internal class CommandException : Exception {
        public CommandException() {
        }

        public CommandException(string message) : base(message) {
        }

        public CommandException(string message, Exception innerException) : base(message, innerException) {
        }

        protected CommandException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
