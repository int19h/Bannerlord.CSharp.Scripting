using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static Hero Me => Hero.MainHero;

        public static Clan MyClan => Me.Clan;

        public static Kingdom? MyKingdom => Me.Clan.Kingdom;

        public static IEnumerable<Town> MyFiefs => Me.Clan.Fiefs;

        public static IEnumerable<Town> MyTowns => MyFiefs.Where(fief => fief.IsTown);

        public static IEnumerable<Town> MyCastles => MyFiefs.Where(fief => fief.IsCastle);

        public static IEnumerable<Village> MyVillages => Me.Clan.Villages;

        public static Hero? MySpouse => Me.Spouse;

        public static IEnumerable<Hero> MyCompanions => Me.Clan.Companions;

        public static IEnumerable<Hero> MyFamily => Me.Clan.Lords;

        public static IEnumerable<Hero> MyChildren => Me.Children;

        public static IEnumerable<Hero> MyDescendants => Me.Descendants();

        public static IEnumerable<Hero> MyFriends => Me.Friends();

        public static IEnumerable<Hero> MyEnemies => Me.Enemies();

        public static MobileParty MyParty => Me.PartyBelongedTo;

        public static ItemRoster MyItems => MyParty.ItemRoster;
    }
}
