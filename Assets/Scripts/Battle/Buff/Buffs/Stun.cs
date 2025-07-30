using System;
using System.Collections;
using UnityEngine;

namespace AtentsPro
{
    public class Stun : Buff
    {
        public Stun(float duration) : base(System.Guid.NewGuid(), duration)
        {

        }

        public override void Effect(Character holder)
        {
            holder.ReserveCommandBuffer(new CharacterCommand{
                action = () =>
                {
                    IEnumerator Co(){
                        holder.SetCanAttack(false);
                        yield return new WaitForSeconds(duration);
                        holder.SetCanAttack(true);
                        holder.CommandBufferEndCallback();
                    }
                    holder.StartCoroutine(Co());
                }
            });
        }

        public override void OnEffect(Character character)
        {

        }

        public override void OnUneffect(Character character)
        {

        }
    }
}
