using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AtentsPro
{
    [System.Serializable]
    public class SubstageSlot
    {
        public Vector3 position;
        public Quaternion rotation = Quaternion.identity;
    }
    [System.Serializable]
    public class Substage
    {
        public SubstageSlot[] teamPos = new SubstageSlot[4];
        public SubstageSlot[] enemyPos;

        public string[] enemiesCharacter;

        public void Enter()
        {

        }
    }
}
