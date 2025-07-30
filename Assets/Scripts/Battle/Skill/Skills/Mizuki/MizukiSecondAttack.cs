using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class MizukiSecondAttack : Skill
    {
        public MizukiSecondAttack( float mpRequire, float cooltime) : base("MZK_02", mpRequire, cooltime){ }

        public override void Cast(ISkillCaster caster, IDamagable target)
        {
            base.Cast(caster, target);

            Status casterStatus = caster.GetStatus();

            if (caster is Character)
            {
                (caster as Character).ReserveSkillAction(()=> { 
                    var mates = BattleSystem.GetInstance().GetTeam(caster as Character);
                    foreach(var mate in mates)
                    {
                        mate.AddBuff(new CriticalRateReinforce(10, mate.GetStatus().criticalRate));
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