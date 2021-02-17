using System.CodeDom;
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

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
            var scriptName = binder.Name + "()";
            var fileName = ScriptFiles.GetFileName(scriptName);
            if (fileName == null) {
                throw new FileNotFoundException($"Function script not found: {scriptName}");
            }

            args = args.Select(arg => arg is string s ? new StringLookup(s) : arg).ToArray();

            var provider = new CSharpCodeProvider();
            var codegenOptions = new CodeGeneratorOptions();
            var code = new StringWriter();
            code.WriteLine($"#load \"{fileName}\"");

            int posCount = binder.CallInfo.ArgumentCount - binder.CallInfo.ArgumentNames.Count;
            var argNames = Enumerable.Repeat((string?)null, posCount).Concat(binder.CallInfo.ArgumentNames).ToArray();

            var invokeExpr = new CodeMethodInvokeExpression(null, binder.Name);
            var invokerDelegate = new CodeTypeDelegate("_InvokeType") {
                ReturnType = new CodeTypeReference(typeof(object))
            };
            var invokerMethod = new CodeMemberMethod {
                Name = "_Invoke",
                ReturnType = new CodeTypeReference(typeof(object)),
                Statements = {
                    new CodeMethodReturnStatement(invokeExpr) {
                        LinePragma = new CodeLinePragma(fileName, 1),
                    },
                },
            };
            var returnInvokerStmt = new CodeMethodReturnStatement(
                new CodeCastExpression(invokerDelegate.Name, new CodeVariableReferenceExpression(invokerMethod.Name))
            );

            for (int i = 0; i < args.Length; ++i) {
                var argName = argNames[i];
                
                var argType = args[i]?.GetType() ?? typeof(object);
                while (argType.IsNotPublic) {
                    argType = argType.BaseType;
                }

                var param = new CodeParameterDeclarationExpression(argType, $"arg{i}");
                invokerDelegate.Parameters.Add(param);
                invokerMethod.Parameters.Add(param);

                var argExpr = new CodeSnippetExpression(param.Name);
                if (argName != null) {
                    argExpr.Value = $"{argName}: {argExpr.Value}";
                }
                invokeExpr.Parameters.Add(argExpr);
            }

            provider.GenerateCodeFromType(invokerDelegate, code, codegenOptions);
            provider.GenerateCodeFromMember(invokerMethod, code, codegenOptions);
            provider.GenerateCodeFromStatement(returnInvokerStmt, code, codegenOptions);

            var state = CSharpScript.RunAsync(code.ToString(), GetScriptOptions()).GetAwaiter().GetResult();
            var invoker = state.ReturnValue;
            try {
                result = invoker.GetType().InvokeMember("Invoke", BindingFlags.InvokeMethod, null, invoker, args);
            } catch (TargetInvocationException ex) {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
            return true;
        }
    }
}
