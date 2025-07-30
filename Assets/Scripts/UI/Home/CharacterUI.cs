using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class CharacterUI : UINavigatable
    {

        public void OnBack()
        {
            UINavigationSystem.GetInstance().ChangeState("_home", new UINavigateParam());
        }

        public void OnCharacterList()
        {
            UINavigationSystem.GetInstance().ChangeState("_characterList", new UINavigateParam());
        }

        public void OnCharacterParty()
        {
            UINavigationSystem.GetInstance().ChangeState("_characterParty", new UINavigateParam());
        }

        #region UINavigatable
        public override GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public override void StateOff()
        {
            this.gameObject.SetActive(false);
        }

        public override void StateOn(UINavigateParam param)
        {
            this.gameObject.SetActive(true);
            HomeNavigation.GetInstance().SetActive(true);
        }
        #endregion
    }
}
