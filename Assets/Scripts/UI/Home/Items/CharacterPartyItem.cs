using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class CharacterPartyItem : MonoBehaviour
    {
        int index;
        string character;
        CharacterPartyButton binded;
        [SerializeField] Image characterImage;

        public void Init(string character, int index, CharacterPartyButton bindedBtn)
        {
            this.index = index;
            this.character = character;
            this.binded = bindedBtn;

            characterImage.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.characterPortraitPath + character);
        }

        public void OnClick()
        {
            UINavigationSystem.GetInstance().loadingUI.SetActive(true);
            WebClient.GetInstance().MakeUpdateCharacterLineupRequest(index, character, (res) => {
                GameManager.GetInstance().userInfo.characterLineup = res.characterLineup;
                binded.characterImage.gameObject.SetActive(true);
                binded.characterImage.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.characterFullbodyPath + character);
                UINavigationSystem.GetInstance().loadingUI.SetActive(false);
            });
        }
    }
}
