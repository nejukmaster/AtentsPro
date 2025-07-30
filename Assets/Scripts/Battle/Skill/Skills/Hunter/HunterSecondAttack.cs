using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace AtentsPro
{
    public class HunterSecondAttack : Skill
    {
        public HunterSecondAttack( float mpRequire, float cooltime) : base("HUN_02", mpRequire, cooltime){ }

        public override void Cast(ISkillCaster caster, IDamagable target)
        {
            base.Cast(caster, target);

            if (caster is Character)
            {
                Character character = (Character)caster;
                character.ReserveSkillAction(() =>
                {
                    character.AddBuff(new AttackReinforce(5.0f, character.GetBasicStatus().attack * 0.3f));
                });
                character.ReserveCommandBuffer(new CharacterCommand
                {
                    action = () => {
                        character.SetCanAttack(false);
                        character.GetAnimator().SetTrigger(skillCode); 
                    }
                });
                character.ReserveCommandBuffer(new CharacterCommand
                {
                    action = () => { 
                        character.SetCanAttack(true); 
                        character.CommandBufferEndCallback();
                    }
                });
            }
        }

        public override HashSet<IDamagable> GetTargetables(ISkillCaster caster)
        {
            HashSet<IDamagable> result = new HashSet<IDamagable>();
            result.Add(caster as IDamagable);
            return result;
        }
    }
}