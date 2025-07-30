using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public abstract class Buff
    {
        public float duration;

        public Guid id;

        public Buff(Guid guid, float duration)
        {
            this.id = guid;
            this.duration = duration;
        }
        //버프 처음 등록했을 때
        public abstract void OnEffect(Character holder);
        //버프가 등록되있는 동안 틱마다 호출
        public abstract void Effect(Character holder);
        //버프가 해제 되었을 때.
        public abstract void OnUneffect(Character holder);
    }
}
