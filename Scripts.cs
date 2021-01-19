using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.Library;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public static class Scripts {
        public static readonly string Location = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "Mount and Blade II Bannerlord",
            "Scripts"
        );

        public static IEnumerable<string> Enumerate() => Directory
            .GetFiles(Location, "*.csx")
            .Select(s => Path.GetFileNameWithoutExtension(s))
            .Where(s => !s.StartsWith("_"));

        public static string? GetFileName(string scriptName) =>
            Enumerate().Contains(scriptName) ? Path.Combine(Location, scriptName + ".csx") : null;

        public static string? GetLogFileName(string scriptName) =>
            Enumerate().Contains(scriptName) ? Path.Combine(Location, scriptName + ".log") : null;

        public static void Initialize() {
            if (Directory.Exists(Location)) {
                return;
            }

            try {
                Directory.CreateDirectory(Location);
            } catch (Exception ex) {
                Debug.PrintError($"Error creating {Location}: {ex.Message}", ex.StackTrace);
                return;
            }

            var prefix = typeof(Scripts).Namespace + ".Samples.";
            foreach (var resourceName in typeof(Scripts).Assembly.GetManifestResourceNames()) {
                if (!resourceName.StartsWith(prefix)) {
                    continue;
                }

                var fileName = Path.Combine(Location, resourceName.Remove(0, prefix.Length));
                using (var source = typeof(Scripts).Assembly.GetManifestResourceStream(resourceName)) {
                    try {
                        using (var target = File.Open(fileName, FileMode.CreateNew)) {
                            source.CopyTo(target);
                        }
                    } catch (Exception ex) {
                        Debug.PrintError($"Error creating {fileName}: {ex.Message}", ex.StackTrace);
                    }
                }
            }
        }
    }
}
