using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class RecruitUI : UINavigatable
    {
        [SerializeField] Image eventBanner;
        [SerializeField] GameObject nextEventBtn;
        [SerializeField] GameObject prevEventBtn;

        [Header("결과 화면")]
        [SerializeField] GameObject recruitResult;
        [SerializeField] Image recruitResultImage;
        [SerializeField] float recruitResultAnimationSpeed;

        int currentEventIndex = 0;

        string recruitedCharacter;

        public void OnBack()
        {
            UINavigationSystem.GetInstance().ChangeState("_home", new UINavigateParam());
        }
        public void OnRecruitBtnClick()
        {
            UINavigationSystem.GetInstance().loadingUI.SetActive(true);
            WebClient.GetInstance().MakeRecruitRequest(GameManager.GetInstance().globalInfo.events[currentEventIndex], (response) => {
                UINavigationSystem.GetInstance().loadingUI.SetActive(false);
                if (response.status == "success")
                {
                    recruitedCharacter = response.recruitedCharacter;
                    GameManager.GetInstance().userInfo.owningCharacters = response.updatedUserInfo.owningCharacters;
                    GameManager.GetInstance().userInfo.currency = response.updatedUserInfo.currency;
                    ShowRecruitResult();
                }
            });
        }
        public void OnNextEventBtnClick()
        {
            currentEventIndex++;
            SetBanner(GameManager.GetInstance().globalInfo.events[currentEventIndex]);
            UpdateEventBtns();
        }

        public void OnPrevEventBtnClick()
        {
            currentEventIndex--;
            SetBanner(GameManager.GetInstance().globalInfo.events[currentEventIndex]);
            UpdateEventBtns();
        }

        public void OnRecruitResultExit()
        {
            recruitResult.SetActive(false);
        }

        public void UpdateEventBtns()
        {
            if(currentEventIndex == 0)
            {
                prevEventBtn.SetActive(false);
            }
            else
            {
                prevEventBtn.SetActive(true);
            }
            if(currentEventIndex == GameManager.GetInstance().globalInfo.events.Length - 1)
            {
                nextEventBtn.SetActive(false);
            }
            else
            {
                nextEventBtn.SetActive(true);
            }
        }

        public void SetBanner(string eventId)
        {
            UINavigationSystem.GetInstance().loadingUI.SetActive(true);

            Sprite sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.eventBannerPath + eventId);

            eventBanner.sprite = sprite;
            UINavigationSystem.GetInstance().loadingUI.SetActive(false);
        }

        public void ShowRecruitResult()
        {
            StartCoroutine(RecruitResultCo(recruitedCharacter));
        }

        IEnumerator RecruitResultCo(string recruitedCharacter)
        {
            recruitResult.SetActive(true);
            recruitResultImage.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.characterFullbodyPath+recruitedCharacter);

            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one;
            float elapsed = 0f;

            while (elapsed < recruitResultAnimationSpeed)
            {
                float t = elapsed / recruitResultAnimationSpeed;
                recruitResultImage.rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
                elapsed += Time.deltaTime;
                yield return null;

            }
            recruitResultImage.rectTransform.localScale = endScale;
        }

        #region UINavigatable
        public override GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public override void StateOff()
        {
            gameObject.SetActive(false);
        }

        public override void StateOn(UINavigateParam param)
        {
            gameObject.SetActive(true);
            HomeNavigation.GetInstance().SetActive(false);
            SetBanner(GameManager.GetInstance().globalInfo.events[currentEventIndex]);
            UpdateEventBtns();
        }
        #endregion
    }
}
