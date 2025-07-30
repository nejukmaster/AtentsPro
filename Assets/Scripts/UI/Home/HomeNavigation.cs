using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class HomeNavigation : MonoBehaviour
    {
        public static HomeNavigation instance;
        public static HomeNavigation GetInstance() { return instance; }

        private void Awake()
        {
            instance = this;
        }

        public void SetActive(bool pBool)
        {
            gameObject.SetActive(pBool);
        }

        public void OnCharacterClick()
        {
            UINavigationSystem.GetInstance().ChangeState("_character", new UINavigateParam());
        }

        public void OnRecruitClick()
        {
            UINavigationSystem.GetInstance().ChangeState("_recruit", new UINavigateParam());
        }

        public void OnInventoryClick()
        {
            UINavigationSystem.GetInstance().ChangeState("_inventory", new UINavigateParam());
        }
    }
}
