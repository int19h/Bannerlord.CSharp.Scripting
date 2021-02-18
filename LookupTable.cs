using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public interface ILookupTable<out T> : IEnumerable<T> {
        T this[Lookup lookup] { get; }
    }

    internal struct LookupTables {
        internal struct Getter<TItem> {
            public static Func<ILookupTable<TItem>?> Get = () => null;
        }

        static LookupTables() {
            Getter<Kingdom>.Get = () => ScriptGlobals.Kingdoms;
            Getter<Clan>.Get = () => ScriptGlobals.Clans;
            Getter<Hero>.Get = () => ScriptGlobals.Heroes;
            Getter<Fief>.Get = () => ScriptGlobals.Fiefs;
            Getter<Town>.Get = () => ScriptGlobals.Fiefs;
            Getter<Village>.Get = () => ScriptGlobals.Villages;
            Getter<MobileParty>.Get = () => ScriptGlobals.Parties;
        }

        private static LookupTables Instance;

        private ILookupTable<TItem>? DoGet<TItem>() => Getter<TItem>.Get();

        public static ILookupTable<TItem>? Get<TItem>() => Instance.DoGet<TItem>();
    }
}
