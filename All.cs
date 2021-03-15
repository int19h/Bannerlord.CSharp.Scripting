using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public struct All {
        public static implicit operator Kingdom[](All all) => ScriptGlobals.Kingdoms.ToArray();

        public static implicit operator Clan[](All all) => ScriptGlobals.Clans.ToArray();

        public static implicit operator Hero[](All all) => ScriptGlobals.Heroes.ToArray();

        public static implicit operator Settlement[](All all) => ScriptGlobals.Settlements.ToArray();

        public static implicit operator Town[](All all) => ScriptGlobals.Fiefs.ToArray();

        public static implicit operator Village[](All all) => ScriptGlobals.Villages.ToArray();

        public static implicit operator MobileParty[](All all) => ScriptGlobals.Parties.ToArray();

        public static implicit operator ItemObject[](All all) => ScriptGlobals.Items.ToArray();
    }
}
