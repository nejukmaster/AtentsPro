using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public interface ISkillCaster
    {
        public float GetMp();
        public bool ConsumMp(float value);

        public Status GetStatus();
        public void SetStatus(Status status);
    }
}
