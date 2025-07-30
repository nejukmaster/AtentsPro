using AtentsPro;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AtentsPro
{
    public class SkillUI : UINavigatable
    {
        public bool bCanSkillCast;

        [SerializeField] SkillExecuteButton[] skillExecuteButtons;
        [SerializeField] GameObject skillTargetingMarker;
        [SerializeField] GameObject characterStatusMarker;

        private void Start()
        {
            EventSystem.GetInstance().characterClickedEvent += this.SetTarget;
            MarkerSystem.GetInstance().InitMarker("skill_targeting",skillTargetingMarker,10);
            MarkerSystem.GetInstance().InitMarker("character_status", characterStatusMarker, 20);
        }
        public void SetTarget(Character target)
        {
            //아군을 클릭한 경우가 아닐경우 Early Return
            if (!BattleSystem.GetInstance().GetTeam().Contains(target)) return;

            for(int i = 0; i < skillExecuteButtons.Length; i++)
            {
                skillExecuteButtons[i].SetTarget(target);
            }
        }

        #region UINavigatable
        public override GameObject GetGameObject()
        {
            return gameObject;
        }

        public override void StateOn(UINavigateParam param)
        {
            bCanSkillCast = true;
        }

        public override void StateOff()
        {
            bCanSkillCast = false;
        }
        #endregion
    }
}
