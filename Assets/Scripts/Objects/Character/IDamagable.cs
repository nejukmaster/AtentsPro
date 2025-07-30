using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public interface IDamagable
    {
        public void OnDamage(float value);
        public void OnHeal(float value);
        public void OnDead();

        public float GetHp();
        public Status GetStatus();
        public void SetStatus(Status status);
        public Transform GetTransform();
    }
}
