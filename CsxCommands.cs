using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Int19h.Csx {
    public static class CsxCommands {
        private static string ScriptsPath {
            get {
                var binPath = Path.GetDirectoryName(typeof(CsxCommands).Assembly.Location);
                var modPath = Path.GetDirectoryName(Path.GetDirectoryName(binPath));
                return Path.Combine(modPath, "scripts");
            }
        }

        private static string WithErrorHandling(Func<string> body) {
            try {
                return body();
            } catch (CommandException ex) {
                return ex.Message;
            } catch (CompilationErrorException ex) {
                return string.Join("\n", ex.Diagnostics);
            } catch (Exception ex) {
                return ex.ToString();
            }
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("list", "csx")]
        public static string List(List<string> args) => WithErrorHandling(() => {
            if (args.Count != 0) {
                throw new CommandException("Usage: csx.list");
            }

            var fileNames = Directory.GetFiles(ScriptsPath, "*.csx");
            var scriptNames = fileNames.Select(s => Path.GetFileNameWithoutExtension(s)).Where(s => !s.StartsWith("_"));
            return string.Join("\n", scriptNames);
        });

        private static IEnumerable<Assembly> GetLoadedAssemblies() =>
            AppDomain.CurrentDomain.GetAssemblies().Where(asm => !asm.IsDynamic && !string.IsNullOrEmpty(asm.Location));

        private static ScriptOptions GetScriptOptions() => ScriptOptions.Default
            .WithEmitDebugInformation(true)
            .WithReferences(GetLoadedAssemblies());

        [CommandLineFunctionality.CommandLineArgumentFunction("run", "csx")]
        public static string Run(List<string> args) => WithErrorHandling(() => {
            if (args.Count < 1) {
                throw new CommandException("Usage: csx.run <script-name> [<args>...]");
            }

            var fileName = Path.Combine(ScriptsPath, args[0] + ".csx");
            if (!File.Exists(fileName)) {
                throw new CommandException($"File not found: {fileName}");
            }
            args.RemoveAt(0);

            using (var globals = new ScriptGlobals(fileName, args)) {
                var code = $"#load \"{fileName}\"";
                var options = GetScriptOptions();
                var result = CSharpScript.EvaluateAsync(code, options, globals).GetAwaiter().GetResult();
                if (result != null && !globals.Log.IsEmpty) {
                    globals.Log.WriteLine(result);
                }
                return $"{result}";
            }
        });

        [CommandLineFunctionality.CommandLineArgumentFunction("eval", "csx")]
        public static string Eval(List<string> args) => WithErrorHandling(() => {
            if (args.Count < 1) {
                throw new CommandException("Usage: csx.eval <code>");
            }

            var options = GetScriptOptions().WithImports(
                "System",
                "System.Collections.Generic",
                "System.IO",
                "System.Linq",
                "System.Text",
                "TaleWorlds",
                "TaleWorlds.CampaignSystem",
                "TaleWorlds.CampaignSystem.Actions",
                "TaleWorlds.Core",
                "TaleWorlds.Library",
                "TaleWorlds.MountAndBlade",
                "TaleWorlds.ObjectSystem"
            );

            var code = string.Join(" ", args).Replace('\'', '"');
            var result = CSharpScript.EvaluateAsync(code, options).GetAwaiter().GetResult();
            return $"{result}";
        });

        private class CommandException : Exception {
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
}
