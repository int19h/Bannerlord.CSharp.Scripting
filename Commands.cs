using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using TaleWorlds.Library;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public static class Commands {
        private static ScriptState? evalState = null;

        private static string WithErrorHandling(Action<TextWriter> body) {
            var output = new System.IO.StringWriter();
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

        [CommandLineFunctionality.CommandLineArgumentFunction("reset", "csx")]
        public static string Reset(List<string> args) => WithErrorHandling(output => {
            if (args.Count != 0) {
                throw new CommandException("Usage: csx.reset");
            }

            evalState = CSharpScript.RunAsync("", Scripts.GetScriptOptions()).GetAwaiter().GetResult();
            output.Write("Script state reset.");
        });


        [CommandLineFunctionality.CommandLineArgumentFunction("eval", "csx")]
        public static string Eval(List<string> args) => WithErrorHandling(output => {
            if (args.Count < 1) {
                throw new CommandException("Usage: csx.eval <code>");
            }

            if (evalState == null) {
                Reset(new());
            }

            var code = ToCode(args);
            try {
                ScriptGlobals.Prepare(output, null);
                evalState = evalState!.ContinueWithAsync(code, Scripts.GetScriptOptions()).GetAwaiter().GetResult();
            } finally {
                ScriptGlobals.Cleanup();
            }
            output.Write(evalState.ReturnValue);
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("dump", "csx")]
        public static string Dump(List<string> args) =>
            Eval(args.Prepend("Dump(").Append(")").ToList());

        [CommandLineFunctionality.CommandLineArgumentFunction("edit", "csx")]
        public static string Edit(List<string> args) =>
            Eval(args.Prepend("Edit(").Append(")").ToList());

        [CommandLineFunctionality.CommandLineArgumentFunction("list", "csx")]
        public static string List(List<string> args) => WithErrorHandling(output => {
            if (args.Count != 0) {
                throw new CommandException("Usage: csx.list");
            }

            output.WriteLine($"@ {ScriptFiles.Location}:");
            output.WriteLine();

            var funcScripts = ScriptFiles.Enumerate().Where(s => s.EndsWith("()"));
            foreach (var line in funcScripts.OrderBy(s => s)) {
                output.WriteLine(line);
            }
            output.WriteLine();

            var commandScripts = ScriptFiles.Enumerate().Except(funcScripts);
            foreach (var line in commandScripts.OrderBy(s => s)) {
                output.WriteLine(line);
            }
            output.WriteLine();
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("run", "csx")]
        public static string Run(List<string> args) => WithErrorHandling(output => {
            if (args.Count < 1) {
                throw new CommandException("Usage: csx.run <script>[.<function>]([<args>...])");
            }

            var code = ToCode(args.Prepend("Scripts."));
            try {
                ScriptGlobals.Prepare(output, null);
                CSharpScript.EvaluateAsync(code, Scripts.GetScriptOptions()).GetAwaiter().GetResult();
            } finally {
                ScriptGlobals.Cleanup();
            }
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
