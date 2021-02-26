﻿using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static string CampaignId => Campaign.Current.UniqueGameId;

        public static Hero Me => Hero.MainHero;

        public static Clan MyClan => Me.Clan;

        public static Kingdom? MyKingdom => Me.Clan.Kingdom;

        public static ILookupTable<Town> MyFiefs => Me.Clan.Fiefs.ToLookupTable();

        public static ILookupTable<Town> MyTowns => MyFiefs[fief => fief.IsTown];

        public static ILookupTable<Town> MyCastles => MyFiefs[fief => fief.IsCastle];

        public static Village[] MyVillages => Me.Clan.Villages.ToArray();

        public static Hero? MySpouse => Me.Spouse;

        public static ILookupTable<Hero> MyCompanions => Me.Clan.Companions.ToLookupTable();

        public static ILookupTable<Hero> MyFamily => Me.Clan.Lords.ToLookupTable();

        public static ILookupTable<Hero> MyChildren => Me.Children.ToLookupTable();

        public static ILookupTable<Hero> MyDescendants => Me.Descendants();

        public static ILookupTable<Hero> MyFriends => Me.Friends();

        public static ILookupTable<Hero> MyEnemies => Me.Enemies();

        public static MobileParty MyParty => Me.PartyBelongedTo;

        public static ItemRoster MyItems => MyParty.ItemRoster;
    }
}
