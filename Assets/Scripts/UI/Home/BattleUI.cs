using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class BattleUI : UINavigatable
    {
        [SerializeField] string stagePreviewBaseURI;

        [SerializeField] GameObject stageListItem;
        [SerializeField] Transform stageListContainer;

        [SerializeField] TextMeshProUGUI stagePreviewNameTmp;
        [SerializeField] TextMeshProUGUI stagePreviewDescriptionTmp;
        [SerializeField] Image stagePreviewImage;

        [SerializeField] int stageListItemPoolingCount;

        Queue<GameObject> stageListItemPool = new Queue<GameObject>();
        StageListItem highlightedStageListItem;

        public void BackToHome()
        {
            UINavigationSystem.GetInstance().ChangeState("_home", new UINavigateParam());
        }

        //스테이지 하이라이트 처리
        public void SetHighlightedStageListItem(StageListItem item)
        {
            highlightedStageListItem?.SetHighlight(false);
            highlightedStageListItem = item;
            highlightedStageListItem.SetHighlight(true);
            StageInfo info = item.GetStage();
            stagePreviewNameTmp.text = info.name;
            stagePreviewDescriptionTmp.text = info.description;
            stagePreviewImage.sprite = Resources.Load<Sprite>(stagePreviewBaseURI + item.GetStageId());
        }

        #region UINavigatable
        public override GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public override void StateOff()
        {
            var stageListItems = stageListContainer.GetComponentsInChildren<StageListItem>();
            for (int i = 0; i < stageListItems.Length; i++)
            {
                GameObject obj = stageListItems[i].gameObject;
                obj.SetActive(false);
                stageListItemPool.Enqueue(obj);
            }

            gameObject.SetActive(false);
        }

        public override void StateOn(UINavigateParam param)
        {
            gameObject.SetActive(true);

            WebClient.StageInfo[] stageList = GameManager.GetInstance().userInfo.enableStages;
            for(int i = 0;i < stageList.Length;i++)
            {
                GameObject obj = stageListItemPool.Dequeue();
                obj.SetActive(true);
                obj.GetComponent<StageListItem>().SetStage(stageList[i].name, stageList[i].isCleared);

                //첫번째 요소에 자동 하이라이트
                if (i==0)
                {
                    SetHighlightedStageListItem(obj.GetComponent<StageListItem>());
                }
            }
        }

        public override void Init()
        {
            for (int i = 0; i < stageListItemPoolingCount; i++)
            {
                GameObject obj = GameObject.Instantiate(stageListItem, stageListContainer);
                obj.SetActive(false);
                stageListItemPool.Enqueue(obj);
            }
        }
        #endregion
    }
}
