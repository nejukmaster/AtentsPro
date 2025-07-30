using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class MizukiFirstAttack : Skill
    {
        public MizukiFirstAttack( float mpRequire, float cooltime) : base("MZK_01", mpRequire, cooltime){ }

        public override void Cast(ISkillCaster caster, IDamagable target)
        {
            base.Cast(caster, target);

            Status casterStatus = caster.GetStatus();

            if (caster is Character)
            {
                (caster as Character).ReserveSkillAction(()=> { 
                    var enemies = BattleSystem.GetInstance().GetEnemies(caster as Character);
                    foreach(var enemy in enemies)
                    {
                        enemy.AddBuff(new AttackReinforce(10f, -(5 + casterStatus.attack * 0.2f)));
                    }
                });
                (caster as Character).ReserveCommandBuffer(new CharacterCommand{
                    action = () => { (caster as Character).GetAnimator().SetTrigger(skillCode); }
                });
            }
        }

        public override HashSet<IDamagable> GetTargetables(ISkillCaster caster)
        {
            return null;
        }
    }
}