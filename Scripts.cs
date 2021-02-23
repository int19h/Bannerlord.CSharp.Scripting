using System;
using System.CodeDom.Compiler;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CSharp;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public class Scripts : DynamicObject {
        internal static ScriptOptions GetScriptOptions() {
            var rsp = RspFile.Generated.Parse();
            var refs = rsp.ResolveMetadataReferences(ScriptMetadataResolver.Default);
            return ScriptOptions.Default
                .WithEmitDebugInformation(true)
                .AddReferences(refs)
                .AddImports(rsp.CompilationOptions.Usings);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            result = new Script(binder.Name);
            return true;
        }
    }

    public class Script : DynamicObject {
        private readonly string name;
        private readonly string fileName;

        public Script(string name) {
            this.name = name;

            if (ScriptFiles.GetFileName(ScriptName) is var fileName and not null) {
                this.fileName = fileName;
            } else { 
                throw new FileNotFoundException($"Function script not found: {ScriptName}");
            }
        }

        private string ScriptName => $"{name}()";

        public override bool TryInvoke(InvokeBinder binder, object?[] args, out object? result) {
            result = Invoke(name, binder.CallInfo, args);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[] args, out object? result) {
            result = Invoke(binder.Name, binder.CallInfo, args);
            return true;
        }

        private object? Invoke(string funcName, CallInfo callInfo, object?[] args) {
            var provider = new CSharpCodeProvider();
            var codegenOptions = new CodeGeneratorOptions();
            var code = new StringWriter();
            code.WriteLine($"#load \"{fileName}\"");

            int posCount = callInfo.ArgumentCount - callInfo.ArgumentNames.Count;
            var argNames = Enumerable.Repeat((string?)null, posCount).Concat(callInfo.ArgumentNames).ToArray();
            code.WriteLine($"#line 1 \"{fileName}\"");
            code.Write($"return (Action<dynamic[]>)(args => {funcName}(");
            for (int i = 0; i < args.Length; ++i) {
                if (i != 0) {
                    code.Write(", ");
                }

                var argName = argNames[i];
                if (argName != null) {
                    code.Write($"{argName}: ");
                }
                code.Write($"args[{i}]");
            }
            code.WriteLine("));");

            var invoker = (Action<object?[]>)CSharpScript.EvaluateAsync(code.ToString(), Scripts.GetScriptOptions()).GetAwaiter().GetResult();
            var oldScriptPath = ScriptGlobals.ScriptPath;
            ScriptGlobals.ScriptPath = fileName;
            args = args.Select(arg => ScriptArgument.Create(arg)).ToArray();
            try {
                invoker(args);
                return true;
            } catch (TargetInvocationException ex) {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            } finally {
                ScriptGlobals.ScriptPath = oldScriptPath;
            }
        }
    }
}
