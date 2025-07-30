using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public class CharacterListUI : UINavigatable
    {
        [Header("오브젝트 풀 설정")]
        ObjectPool characterListItemPool = new ObjectPool();
        [SerializeField] int characterListItemPoolCount;
        [SerializeField] GameObject characterPortraitItem;
        [SerializeField] Transform characterPortraitContainer;

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
            foreach(CharacterListItem item in characterPortraitContainer.GetComponentsInChildren<CharacterListItem>())
            {
                characterListItemPool.Enqueue(item.gameObject, null);
            }
            this.gameObject.SetActive(false);
        }

        public override void StateOn(UINavigateParam param)
        {
            this.gameObject.SetActive(true);
            HomeNavigation.GetInstance().SetActive(false);
            foreach(var chr in GameManager.GetInstance().userInfo.owningCharacters)
            {
                characterListItemPool.Dequeue((o) =>
                {
                    CharacterListItem item = o.GetComponent<CharacterListItem>();
                    item.SetCharacter(chr);
                });
            }
        }

        public override void Init()
        {
            characterListItemPool.Init(characterPortraitItem, characterListItemPoolCount, characterPortraitContainer);
        }
        #endregion
    }
}
