using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class CharacterPartyUI : UINavigatable
    {
        [SerializeField] GameObject[] partyButtons;
        [SerializeField] GameObject characterContainer;

        [Header("오브젝트 풀 설정")]
        [SerializeField] GameObject characterPartyItem;
        [SerializeField] Transform characterPartyItemContainer;
        [SerializeField] int characterPartyItemPoolCount;
        ObjectPool characterPartyItemPool = new ObjectPool();

        public void OnPartyButtonClick(int index)
        {
            if(!characterContainer.activeSelf)
                characterContainer.SetActive(true);
            foreach(var item in characterPartyItemContainer.GetComponentsInChildren<CharacterPartyItem>())
            {
                characterPartyItemPool.Enqueue(item.gameObject, null);
            }
            for(int i = 0; i < GameManager.GetInstance().userInfo.owningCharacters.Length; i++)
            {
                characterPartyItemPool.Dequeue(null);
            }
            var items = characterPartyItemContainer.GetComponentsInChildren<CharacterPartyItem>();
            for(int i = 0; i < GameManager.GetInstance().userInfo.owningCharacters.Length; i++)
            {
                items[i].Init(GameManager.GetInstance().userInfo.owningCharacters[i], index, partyButtons[index].GetComponent<CharacterPartyButton>());
            }
        }

        public void OnBack()
        {
            UINavigationSystem.GetInstance().ChangeState("_character", new UINavigateParam());
        }

        #region UINavigatable
        public override GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public override void StateOff()
        {
            characterContainer.SetActive(false);
            gameObject.SetActive(false);
        }

        public override void StateOn(UINavigateParam param)
        {
            gameObject.SetActive(true);
            HomeNavigation.GetInstance().gameObject.SetActive(false);

            var lineup = GameManager.GetInstance().userInfo.characterLineup;
            for(int i = 0; i < lineup.Length; i++)
            {
                if(lineup[i] != "None")
                {
                    partyButtons[i].GetComponent<CharacterPartyButton>().characterImage.gameObject.SetActive(true);
                    partyButtons[i].GetComponent<CharacterPartyButton>().characterImage.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.characterFullbodyPath + lineup[i]);
                }
            }
        }

        public override void Init()
        {
            for(int i = 0; i < partyButtons.Length; i++)
            {
                int idx = i;
                partyButtons[i].GetComponent<CharacterPartyButton>().button.onClick.AddListener(() => { OnPartyButtonClick(idx); });
            }
            characterPartyItemPool.Init(characterPartyItem, characterPartyItemPoolCount, characterPartyItemContainer);
        }
        #endregion
    }
}
