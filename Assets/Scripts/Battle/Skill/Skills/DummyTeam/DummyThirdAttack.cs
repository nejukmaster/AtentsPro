using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class DummyThirdAttack : Skill
    {
        public DummyThirdAttack(float mpRequire, float cooltime) : base("DMY_03", mpRequire, cooltime){ }

        public override void Cast(ISkillCaster caster, IDamagable target)
        {
            base.Cast(caster, target);

            if (!(caster is IDamagable)) return;

            IDamagable damagable = (IDamagable)caster;
            if (caster is Character)
            {
                (caster as Character).ReserveSkillAction(() => {
                    BattleSystem.GetInstance().ReserveDamage(new DamageRequest(caster as Character, target, 30f + caster.GetStatus().attack*1.2f)); 
                });
                (caster as Character).ReserveCommandBuffer(new CharacterCommand {
                    action = () =>
                    {
                        (caster as Character).SetCanAttack(false);
                        (caster as Character).MoveTo(target.GetTransform().position, 0.65f, null, () =>
                        {
                            (caster as Character).CommandBufferEndCallback();
                        });
                    }
                });
                (caster as Character).ReserveCommandBuffer(new CharacterCommand {
                    action = () =>
                    {
                        (caster as Character).GetAnimator().SetTrigger(skillCode);
                    }
                });
                (caster as Character).ReserveCommandBuffer(new CharacterCommand
                {
                    action = () =>
                    {
                        (caster as Character).RotateTo((caster as Character).currentSlot.position);
                        (caster as Character).MoveTo((caster as Character).currentSlot.position, 0.65f, null, () =>
                        {
                            (caster as Character).SetCanAttack(true);
                            (caster as Character).RotateTo((caster as Character).currentSlot.rotation);
                            (caster as Character).CommandBufferEndCallback();
                        });
                    }
                });
            }
        }
        public override HashSet<IDamagable> GetTargetables(ISkillCaster caster)
        {
            if (!(caster is Character)) return null;

            HashSet<IDamagable> targetables = new HashSet<IDamagable>();
            foreach (Character enemy in BattleSystem.GetInstance().GetEnemies((Character)caster))
            {
                targetables.Add(enemy);
            }
            return targetables;
        }
    }
}
