using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AtentsPro
{
    public class StageListItem : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI stageNameTmp;
        [SerializeField] GameObject highlightImage;
        [SerializeField] GameObject mask;

        string stageId;
        StageInfo stageInfo;

        public void SetStage(string stageId, bool bIsCleared)
        {
            this.stageId = stageId;
            this.stageInfo = DataSheetLoader.GetInstance().stageTable[stageId];
            mask.SetActive(bIsCleared);
            stageNameTmp.text = DataSheetLoader.GetInstance().stageTable[stageId].name;
        }

        public void OnClick()
        {
            GetComponentInParent<BattleUI>().SetHighlightedStageListItem(this);
        }

        public string GetStageId()
        {
            return this.stageId;
        }

        public StageInfo GetStage()
        {
            return stageInfo;
        }

        public void SetHighlight(bool pBool)
        {
            highlightImage.SetActive(pBool);
        }

        public void EnterStage()
        {
            GameManager.GetInstance().LoadLoadingScene(new SceneLoadParam {
                actionToLoad = () =>
                {
                    WebClient.GetInstance().responseCallback += (response) => {
                        if (response is WebClient.StageStartResponse)
                        {
                            var stageStart = (WebClient.StageStartResponse)response;
                            if (stageStart.canEnter)
                            {
                                Dictionary<string, WebClient.CharacterinfoResponse> characterDic = new Dictionary<string, WebClient.CharacterinfoResponse>();
                                foreach (var characterInfo in stageStart.characters)
                                {
                                    characterDic.Add(characterInfo.name, characterInfo);
                                }
                                GameManager.GetInstance().homeToBattle = new HomeToBattle
                                {
                                    stageName = stageStart.name,
                                    characterDic = characterDic,
                                    team = stageStart.team,
                                    substages = stageStart.substages,
                                };
                                SceneManager.LoadScene(AtentsProGlobalSettings.Instance.scenesPath + "Battle");
                            }
                        }
                    };
                    WebClient.GetInstance().MakeStageStartRequest(stageId);
                }
            });
        }
    }
}
