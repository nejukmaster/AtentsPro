using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace AtentsPro
{
    public class HunterFirstAttack : Skill
    {
        public HunterFirstAttack( float mpRequire, float cooltime) : base("HUN_01", mpRequire, cooltime){ }

        public override void Cast(ISkillCaster caster, IDamagable target)
        {
            base.Cast(caster, target);

            if (caster is Character)
            {
                Character character = (Character)caster;
                character.ReserveSkillAction(() =>
                {
                    BattleSystem.GetInstance().ReserveDamage(new DamageRequest((IDamageCauser)caster, target,
                                                                                caster.GetStatus().attack * 0.7f));
                    if(target is Character)
                    {
                        (target as Character).AddBuff(new Stun(1.0f));
                    }
                });

                character.ReserveCommandBuffer(new CharacterCommand
                {
                    action = () => {
                        character.SetCanAttack(false);
                        character.RotateTo(target.GetTransform().position);
                        character.MoveTo(target.GetTransform().position, 0.65f, null, () =>
                        {
                            character.CommandBufferEndCallback();
                        });
                    }
                });
                character.ReserveCommandBuffer(new CharacterCommand
                {
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