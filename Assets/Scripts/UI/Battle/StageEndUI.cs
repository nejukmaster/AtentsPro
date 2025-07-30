using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AtentsPro
{
    public class StageEndUI : UINavigatable
    {
        [SerializeField] GameObject loadingMask;
        [SerializeField] Transform itemRewardContainer;
        [SerializeField] GameObject rewardDisplayItem;
        [SerializeField] Image title;
        [SerializeField] float itemDisplaySpeed;

        IEnumerator ItemDisplayCo(List<WebClient.ItemInfo> items)
        {
            for(int i = 0; i < items.Count; i++)
            {
                DisplayItem(items[i]);
                yield return new WaitForSeconds(itemDisplaySpeed);
            }
        }

        public void DisplayItem(WebClient.ItemInfo item)
        {
            GameObject obj = GameObject.Instantiate(rewardDisplayItem, itemRewardContainer);
            Sprite itemSprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.itemIconPath + item.id);
            obj.GetComponentInChildren<Image>().sprite = itemSprite;
            obj.GetComponentInChildren<TextMeshProUGUI>().text = item.count.ToString();
        }

        public void OnConfirm()
        {
            GameManager.GetInstance().homeToBattle = new HomeToBattle();
            GameManager.GetInstance().LoadLoadingScene(new SceneLoadParam
            {
                actionToLoad = () =>
                {
                    WebClient.GetInstance().responseCallback += (response) =>
                    {
                        if (response is WebClient.UserInfoResponse)
                        {
                            var info = (WebClient.UserInfoResponse)response;
                            GameManager.GetInstance().userInfo = info;
                            SceneManager.LoadScene(AtentsProGlobalSettings.Instance.scenesPath + "Home");
                        }
                    };
                    WebClient.GetInstance().MakeUserInfoRequest(GameManager.GetInstance().userId);
                }
            });
        }

        #region UINavigatable 추상 메서드
        public override GameObject GetGameObject()
        {
            return gameObject;
        }

        public override void StateOff() { }

        public override void StateOn(UINavigateParam param)
        {
            gameObject.SetActive(true);
            loadingMask.SetActive(true);
            if(param.pBool)
            {
                title.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.uiTexturePath+"StageClearTitle");
                WebClient.GetInstance().responseCallback += (res) =>
                {
                    if (res is (WebClient.EndStageResponse))
                    {
                        loadingMask.SetActive(false);
                        StartCoroutine(ItemDisplayCo((res as WebClient.EndStageResponse).rewards));
                    }
                };
            }
            else
            {
                title.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.uiTexturePath + "StageFailedTitle");
            }
        }
        #endregion
    }
}
