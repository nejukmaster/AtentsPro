using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class DummyBasicAttack : Skill
    {
        public DummyBasicAttack( float mpRequire, float cooltime) : base("DMY_00", mpRequire, cooltime){ }

        public override void Cast(ISkillCaster caster, IDamagable target)
        {
            base.Cast(caster, target);

            Status casterStatus = caster.GetStatus();
            Status targetStatus = target.GetStatus();

            float damage = (30.0f + casterStatus.attack * 0.6f) - targetStatus.defence * 0.3f;  //�ۿ� ������ ���
            damage = damage * (Random.value < casterStatus.criticalRate ? casterStatus.criticalDamage : 1.0f);    //ũ��Ƽ�� ���

            if (caster is Character)
            {
                (caster as Character).ReserveSkillAction(()=> { BattleSystem.GetInstance().ReserveDamage(new DamageRequest((IDamageCauser)caster, target, damage)); });
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
