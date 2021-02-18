using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static Hero Me => Hero.MainHero;

        public static Clan MyClan => Me.Clan;

        public static Kingdom? MyKingdom => Me.Clan.Kingdom;

        public static LookupTable<Town> MyFiefs => new(Me.Clan.Fiefs);

        public static Town[] MyTowns => MyFiefs[fief => fief.IsTown];

        public static Town[] MyCastles => MyFiefs[fief => fief.IsCastle];

        public static Village[] MyVillages => Me.Clan.Villages.ToArray();

        public static Hero? MySpouse => Me.Spouse;

        public static Hero[] MyCompanions => Me.Clan.Companions.ToArray();

        public static Hero[] MyFamily => Me.Clan.Lords.ToArray();

        public static Hero[] MyChildren => Me.Children.ToArray();

        public static Hero[] MyDescendants => Me.Descendants();

        public static Hero[] MyFriends => Me.Friends();

        public static Hero[] MyEnemies => Me.Enemies();

        public static MobileParty MyParty => Me.PartyBelongedTo;

        public static ItemRoster MyItems => MyParty.ItemRoster;
    }
}
