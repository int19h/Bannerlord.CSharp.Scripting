using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static void Kill(this Hero hero) => KillCharacterAction.ApplyByMurder(hero);

        public static void Kill(this IEnumerable<Hero> heroes) {
            foreach (var hero in heroes) {
                hero.Kill();
            }
        }

        public static void Kill(params Hero[] heroes) => heroes.Kill();
    }
}