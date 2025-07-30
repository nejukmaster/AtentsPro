using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class CharacterListItem : MonoBehaviour
    {
        [SerializeField] Image image;

        string characterName;

        public void SetCharacter(string characterName)
        {
            this.characterName = characterName;
            image.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.characterPortraitPath + characterName);
        }

        public void OnClick()
        {
            UINavigationSystem.GetInstance().ChangeState("_characterInfo", new UINavigateParam {
                pString = characterName
            });
        }
    }
}
