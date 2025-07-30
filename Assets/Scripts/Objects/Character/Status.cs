using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    [System.Serializable]
    public struct Status
    {
        public float maxHp;
        public float maxMp;

        public float attack;
        public float defence;
        public float criticalRate;
        public float criticalDamage;
        public float attackSpeed;

        [HideInInspector] public float hp;
        [HideInInspector] public float mp;
    }
}
