using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Int19h.Bannerlord.CSharp.Scripting {

    [DataContract]
    internal class OmniSharpConfig {
        private static readonly DataContractJsonSerializer serializer = new(typeof(OmniSharpConfig));

        [DataContract]
        public class ScriptObject {
            [DataMember]
            public string? RspFilePath;
        }

        [DataMember]
        public readonly ScriptObject Script = new();

        public static void Update() {
            var config = new OmniSharpConfig {
                Script = {
                    RspFilePath = Scripts.RspFilePath
                }
            };

            var json = new MemoryStream();
            serializer.WriteObject(json, config);
            json.Position = 0;

            if (!Directory.Exists(Scripts.UserLocation)) {
                try {
                    Directory.CreateDirectory(Scripts.UserLocation);
                } catch (Exception) {
                    return;
                }
            }

            var fileName = Path.Combine(Scripts.UserLocation, "omnisharp.json");
            try {
                using (var stream = File.Create(fileName)) {
                    json.CopyTo(stream);
                }
            } catch (Exception) {
                return;
            }
        }
    }
}
