using System;
using System.Collections;
using UnityEngine;

namespace AtentsPro
{
    public class CriticalRateReinforce : Buff
    {
        float force;
        public CriticalRateReinforce(float duration, float force) : base(System.Guid.NewGuid(), duration)
        {
            this.force = force;
        }

        public override void Effect(Character holder)
        {
            holder.AddTemporaryStatus(this.id, new Status {
                criticalRate = force
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
