using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Int19h.Bannerlord.CSharp.Scripting {
    partial class ScriptGlobals {
        public static void Kill(this Hero hero) {
            if (hero.IsDead) {
                throw new InvalidOperationException($"{hero} is already dead");
            }
            KillCharacterAction.ApplyByMurder(hero);
        }

        public static void Kill(this IEnumerable<Hero> heroes) {
            foreach (var hero in heroes) {
                hero.Kill();
            }
        }

        public static void Kill(params Hero[] heroes) => heroes.Kill();

        public static void MakePregnant(this Hero hero) {
            if (hero.IsPregnant) {
                throw new InvalidOperationException($"{hero} is already pregnant");
            } else if (hero.Spouse == null) {
                throw new InvalidOperationException($"{hero} has no spouse");
            }
            MakePregnantAction.Apply(hero);
        }

        public static void MakePregnant(this IEnumerable<Hero> heroes) {
            foreach (var hero in heroes) {
                hero.MakePregnant();
            }
        }

        public static void MakePregnant(params Hero[] heroes) => heroes.MakePregnant();
    }
}
