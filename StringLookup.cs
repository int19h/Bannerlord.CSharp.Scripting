using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public class StringLookup {
        private readonly string s;

        public StringLookup(string s) {
            this.s = s;
        }

        public override string ToString() => s;

        public static implicit operator StringLookup(string s) => new StringLookup(s);

        public static implicit operator string(StringLookup sl) => sl.ToString();

        public static implicit operator Kingdom(StringLookup sl) => ScriptGlobals.Kingdoms[sl];

        public static implicit operator Clan(StringLookup sl) => ScriptGlobals.Clans[sl];

        public static implicit operator Hero(StringLookup sl) => ScriptGlobals.Heroes[sl];

        public static implicit operator Settlement(StringLookup sl) => ScriptGlobals.Settlements[sl];

        public static implicit operator Town(StringLookup sl) => ScriptGlobals.Fiefs[sl];

        public static implicit operator Village(StringLookup sl) => ScriptGlobals.Villages[sl];

        public static implicit operator MobileParty(StringLookup sl) => ScriptGlobals.Parties[sl];
    }
}
