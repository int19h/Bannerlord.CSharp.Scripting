using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using TaleWorlds.Library;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public static class Commands {
        private static ScriptState? _evalState = null;
        private static ScriptGlobals _evalGlobals = new ScriptGlobals();

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

        private static IEnumerable<Assembly> GetLoadedAssemblies() =>
            AppDomain.CurrentDomain.GetAssemblies().Where(asm => !asm.IsDynamic && !string.IsNullOrEmpty(asm.Location));

        private static ScriptOptions GetScriptOptions() => ScriptOptions.Default
            .WithEmitDebugInformation(true)
            .WithReferences(GetLoadedAssemblies().Concat(new[] {
                typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly,
            }));
        
        private static ScriptOptions GetEvalScriptOptions() => GetScriptOptions()
            .WithImports(Config.Load().EvalImports);

        [CommandLineFunctionality.CommandLineArgumentFunction("help", "csx")]
        public static string Help (List<string> args) => WithErrorHandling(output => {
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

            _evalState = CSharpScript.RunAsync("", GetEvalScriptOptions(), _evalGlobals).GetAwaiter().GetResult();
            output.Write("Script state reset.");
        });


        [CommandLineFunctionality.CommandLineArgumentFunction("eval", "csx")]
        public static string Eval(List<string> args) => WithErrorHandling(output => {
            if (args.Count < 1) {
                throw new CommandException("Usage: csx.eval <code>");
            }

            if (_evalState == null) {
                Reset(new());
            }

            var code = string.Join(" ", args).Replace('\'', '"').Replace(".,", ";");
            _evalState = _evalState!.ContinueWithAsync(code, GetEvalScriptOptions()).GetAwaiter().GetResult();
            output.Write(_evalState.ReturnValue);
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("list", "csx")]
        public static string List(List<string> args) => WithErrorHandling(output => {
            if (args.Count != 0) {
                throw new CommandException("Usage: csx.list");
            }

            output.WriteLine($"@ {Scripts.Location}:");
            output.WriteLine();
            foreach (var line in Scripts.Enumerate()) {
                output.WriteLine(line);
            }
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("run", "csx")]
        public static string Run(List<string> args) => WithErrorHandling(output => {
            if (args.Count < 1) {
                throw new CommandException("Usage: csx.run <script-name> [<args>...]");
            }

            var scriptName = args[0];
            var fileName = Scripts.GetFileName(scriptName);
            if (fileName == null) {
                throw new CommandException($"Script not found: {scriptName}");
            }
            args.RemoveAt(0);

            using (var globals = new ScriptGlobals(fileName, output, args)) {
                var code = $"#load \"{fileName}\"";
                var result = CSharpScript.EvaluateAsync(code, GetScriptOptions(), globals).GetAwaiter().GetResult();
                if (result != null) {
                    globals.Log.Write(result);
                }
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
