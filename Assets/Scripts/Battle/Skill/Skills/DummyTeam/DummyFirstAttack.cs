using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class DummyFirstAttack : Skill
    {
        public DummyFirstAttack( float mpRequire, float cooltime) : base("DMY_01", mpRequire, cooltime){ }

        public override void Cast(ISkillCaster caster, IDamagable target)
        {
            base.Cast(caster, target);

            if(caster is Character)
            {
                Character character = (Character)caster;
                if (target.GetTransform())
                {
                    //C#의 람다는 로컬변수를 참조할때 해당 변수를 캡처하므로 originPos 그대로 사용해도 무방
                    //1타
                    character.ReserveSkillAction(() =>
                    {
                        BattleSystem.GetInstance().ReserveDamage(new DamageRequest((IDamageCauser)caster, target, 
                                                                                    50f + caster.GetStatus().attack*0.6f - target.GetStatus().defence*0.2f));
                    });
                    //2타
                    character.ReserveSkillAction(() =>
                    {
                        
                        BattleSystem.GetInstance().ReserveDamage(new DamageRequest((IDamageCauser)caster, target,
                                                                                    10f + caster.GetStatus().attack * 0.5f - target.GetStatus().defence * 0.2f));
                    });

                    //커맨드 버퍼 예약
                    character.ReserveCommandBuffer(new CharacterCommand{
                        action = () =>{
                            character.SetCanAttack(false);
                            character.RotateTo(target.GetTransform().position);
                            character.MoveTo(target.GetTransform().position, 0.65f, null, () =>
                            {
                                character.CommandBufferEndCallback();
                            });
                        }
                    });
                    character.ReserveCommandBuffer(new CharacterCommand{
                        action = () => { character.GetAnimator().SetTrigger(skillCode); }
                    });
                    character.ReserveCommandBuffer(new CharacterCommand
                    {
                        action = () =>
                        {
                            character.RotateTo(character.currentSlot.position);
                            character.MoveTo(character.currentSlot.position, 0.65f, null, () =>
                            {
                                character.SetCanAttack(true);
                                character.RotateTo(character.currentSlot.rotation);
                                character.CommandBufferEndCallback();
                            });
                        }
                    });
                }
            }
        }

        public override HashSet<IDamagable> GetTargetables(ISkillCaster caster)
        {
            if (!(caster is Character)) return null;

            HashSet<IDamagable> targetables = new HashSet<IDamagable>();
            foreach(Character enemy in BattleSystem.GetInstance().GetEnemies((Character)caster))
            {
                targetables.Add(enemy);
            }
            return targetables;
        }
    }
}
