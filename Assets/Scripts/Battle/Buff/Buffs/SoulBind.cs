using System;
using System.Collections;
using UnityEngine;

namespace AtentsPro
{
    public class SoulBind : Buff
    {
        public SoulBind(float duration) : base(System.Guid.NewGuid(), duration)
        {

        }

        public override void Effect(Character holder)
        {
            holder.AddTemporaryStatus(this.id, new Status {
                attack = -holder.GetStatus().attack,
                defence = -holder.GetStatus().defence
            });
        }

        public override void OnEffect(Character holder)
        {

        }

        public override void OnUneffect(Character holder)
        {
            holder.RemoveTemporaryStatus(this.id);
        }
    }
}
