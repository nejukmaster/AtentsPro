using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class DummyStages : Stage
    {
        

        public override bool IsSubstageEnd()
        {
            bool flag = true;
            foreach(var team in BattleSystem.GetInstance().GetTeam())
            {
                flag &= team.IsCharacterCommandBufferReady();
            }
            return BattleSystem.GetInstance().GetEnemies().Count <= 0 && flag;
        }
        public override bool IsFailed()
        {
            return BattleSystem.GetInstance().GetTeam().Count <= 0;
        }
    }
}
