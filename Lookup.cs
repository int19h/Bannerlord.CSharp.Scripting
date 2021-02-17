using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public abstract class Lookup {
        public abstract string What { get; }

        public abstract bool Matches<T>(T x);

        public static implicit operator Lookup(string s) => new NameLookup(s);

        public static implicit operator Kingdom(Lookup lookup) => ScriptGlobals.Kingdoms[lookup];

        public static implicit operator Kingdom[](Lookup lookup) => new[] { (Kingdom)lookup };

        public static implicit operator Clan(Lookup lookup) => ScriptGlobals.Clans[lookup];

        public static implicit operator Clan[](Lookup lookup) => new[] { (Clan)lookup };

        public static implicit operator Hero(Lookup lookup) => ScriptGlobals.Heroes[lookup];

        public static implicit operator Hero[](Lookup lookup) => new[] { (Hero)lookup };

        public static implicit operator Settlement(Lookup lookup) => ScriptGlobals.Settlements[lookup];

        public static implicit operator Settlement[](Lookup lookup) => new[] { (Settlement)lookup };

        public static implicit operator Town(Lookup lookup) => ScriptGlobals.Fiefs[lookup];

        public static implicit operator Town[](Lookup lookup) => new[] { (Town)lookup };

        public static implicit operator Village(Lookup lookup) => ScriptGlobals.Villages[lookup];

        public static implicit operator Village[](Lookup lookup) => new[] { (Village)lookup };

        public static implicit operator MobileParty(Lookup lookup) => ScriptGlobals.Parties[lookup];

        public static implicit operator MobileParty[](Lookup lookup) => new[] { (MobileParty)lookup };
    }
}
