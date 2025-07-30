using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace AtentsPro
{
    public class HunterThirdAttack : Skill
    {
        public HunterThirdAttack( float mpRequire, float cooltime) : base("HUN_03", mpRequire, cooltime){ }

        public override void Cast(ISkillCaster caster, IDamagable target)
        {
            base.Cast(caster, target);

            if (caster is Character)
            {
                Character character = (Character)caster;
                for(int i = 0; i < 5; i++) 
                    character.ReserveSkillAction(() => {
                        BattleSystem.GetInstance().ReserveDamage(new DamageRequest(character, target,
                                                                  (10f + character.GetStatus().attack * 0.3f) * character.GetStatus().criticalDamage));
                    });
                for (int i = 0; i < 5; i++)
                    character.ReserveCommandBuffer(new CharacterCommand
                    {
                        action = () => { character.GetAnimator().SetTrigger(skillCode); }
                    });
            }
        }

        public override HashSet<IDamagable> GetTargetables(ISkillCaster caster)
        {
            HashSet<IDamagable> result = new HashSet<IDamagable>();
            if (!(caster is Character)) return result;
            foreach(Character enemy in BattleSystem.GetInstance().GetEnemies(caster as Character))
            {
                result.Add(enemy);
            }
            return result;
        }
    }
}