using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class MizukiThirdAttack : Skill
    {
        public MizukiThirdAttack( float mpRequire, float cooltime) : base("MZK_03", mpRequire, cooltime){ }

        public override void Cast(ISkillCaster caster, IDamagable target)
        {
            base.Cast(caster, target);

            if (caster is Character)
            {
                (caster as Character).ReserveSkillAction(()=> {
                    (target as Character).AddBuff(new SoulBind(10f));
                });
                (caster as Character).ReserveCommandBuffer(new CharacterCommand{
                    action = () => { (caster as Character).GetAnimator().SetTrigger(skillCode); }
                });
            }
        }

        public override HashSet<IDamagable> GetTargetables(ISkillCaster caster)
        {
            HashSet<IDamagable> result = new HashSet<IDamagable>();
            if (!(caster is Character)) return result;
            foreach (Character enemy in BattleSystem.GetInstance().GetEnemies(caster as Character))
            {
                result.Add(enemy);
            }
            return result;
        }
    }
}