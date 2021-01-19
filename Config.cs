using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace Int19h.Bannerlord.CSharp.Scripting {
    [XmlRoot("csx")]
    public class Config {
        public static readonly string FileName = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "Mount and Blade II Bannerlord",
            "Configs",
            "csx.xml"
        );

        private static readonly XmlSerializer _serializer = new(typeof(Config));

        [XmlArrayItem("Import")]
        public List<string> EvalImports { get; } = new();

        public static Config Load() {
            Stream? stream = null;
            try {
                try {
                    stream = File.OpenRead(FileName);
                } catch (Exception) {
                    // If config file is missing, try to create it as a copy of the default config.
                    var defaults = typeof(Config).Assembly.GetManifestResourceStream(typeof(Config), "Config.Default.xml");
                    try {
                        stream = File.Open(FileName, FileMode.CreateNew, FileAccess.ReadWrite);
                        defaults.CopyTo(stream);
                        stream.Position = 0;
                    } catch (Exception ex) {
                        // If config file couldn't be created from default config, just load the latter directly.
                        Debug.PrintError($"Error creating {FileName}: {ex.Message}", ex.StackTrace);
                        stream = defaults;
                        defaults = null;
                    } finally {
                        if (defaults != null) {
                            defaults.Dispose();
                        }
                    }
                }
                try {
                    return (Config)_serializer.Deserialize(stream);
                } catch (Exception ex) {
                    Debug.PrintError($"Error loading {FileName}: {ex.Message}", ex.StackTrace);
                    return new Config();
                }
            } finally {
                if (stream != null) {
                    stream.Dispose();
                }
            }
        }
    }
}
