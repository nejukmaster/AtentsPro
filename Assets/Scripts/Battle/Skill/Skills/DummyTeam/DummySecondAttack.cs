using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class DummySecondAttack : Skill
    {
        public DummySecondAttack(float mpRequire, float cooltime) : base("DMY_02", mpRequire, cooltime){ }

        public override void Cast(ISkillCaster caster, IDamagable target)
        {
            base.Cast(caster, target);

            if (!(caster is IDamagable)) return;

            IDamagable damagable = (IDamagable)caster;
            if (caster is Character)
            {
                (caster as Character).ReserveSkillAction(() => { damagable.OnHeal(damagable.GetStatus().attack * 1.2f); });
                (caster as Character).ReserveCommandBuffer(new CharacterCommand{
                    action = () =>
                    {
                        (caster as Character).SetCanAttack(false);
                        (caster as Character).GetAnimator().SetTrigger(skillCode);
                    }
                });
                (caster as Character).ReserveCommandBuffer(new CharacterCommand
                {
                    action = () =>
                    {
                        (caster as Character).SetCanAttack(true);
                        (caster as Character).CommandBufferEndCallback();
                    }
                });
            }
            else
                damagable.OnHeal(caster.GetStatus().attack * 1.2f);
        }
        public override HashSet<IDamagable> GetTargetables(ISkillCaster caster)
        {
            if (!(caster is Character)) return null;

            HashSet<IDamagable> targetables = new HashSet<IDamagable>();
            targetables.Add((Character)caster);
            return targetables;
        }
    }
}
