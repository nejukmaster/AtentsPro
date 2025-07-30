using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    [System.Serializable]
    public abstract class Skill
    {
        public string skillCode;

        private float mpRequire;
        public float MpRequire { get { return mpRequire; } }

        private float cooltime;
        public float Cooltime { get { return cooltime; } }

        public Skill(string skillCode, float mpRequire, float cooltime)
        {
            this.skillCode = skillCode;
            this.mpRequire = mpRequire;
            this.cooltime = cooltime;
        }

        public virtual void Cast(ISkillCaster caster, IDamagable target)
        {
            
        }

        public abstract HashSet<IDamagable> GetTargetables(ISkillCaster caster);
    }
}
