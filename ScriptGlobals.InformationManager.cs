using System.IO;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static void Print(string line) {
            var message = new InformationMessage(line);
            InformationManager.DisplayMessage(message);
        }

        public static void MessageBox(string text, string title, bool pause = false) {
            var inquiry = new InquiryData(title, text, true, false, "OK", null, null, null);
            InformationManager.ShowInquiry(inquiry, pause);
        }

        public static void MessageBox(string text, bool pause = false, [CallerFilePath] string? callerFilePath = null, [CallerLineNumber] int? callerLineNumber = null) {
            var title =
                string.IsNullOrEmpty(callerFilePath) ?
                "csx.eval" :
                $"{Path.GetFileName(callerFilePath)}({callerLineNumber})";
            MessageBox(text, title, pause);
        }
    }
}
