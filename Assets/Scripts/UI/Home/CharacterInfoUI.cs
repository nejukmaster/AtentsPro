using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class CharacterInfoUI : UINavigatable
    {
        [SerializeField] GameObject loadingMask;

        [Header("스테이터스 TMP")]
        [SerializeField] TextMeshProUGUI maxHp;
        [SerializeField] TextMeshProUGUI maxMp;
        [SerializeField] TextMeshProUGUI attack;
        [SerializeField] TextMeshProUGUI defence;
        [SerializeField] TextMeshProUGUI criticalRate;
        [SerializeField] TextMeshProUGUI criticalDamage;
        [SerializeField] TextMeshProUGUI attackSpeed;

        [Header("캐릭터 삽화")]
        [SerializeField] Image characterIllust;

        [Header("오브젝트 풀 설정")]
        ObjectPool skillDescriptionItemPool = new ObjectPool();
        [SerializeField] int skillDescriptionItemCount;
        [SerializeField] GameObject skillDescriptionItem;
        [SerializeField] Transform skillDescriptionContainer;


        public void OnBack()
        {
            UINavigationSystem.GetInstance().ChangeState("_characterList",new UINavigateParam());
        }
        #region UINavigatable
        public override GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public override void StateOff()
        {
            loadingMask.SetActive(true);
            foreach(var skill in skillDescriptionContainer.GetComponentsInChildren<SkillDescriptionItem>())
            {
                skillDescriptionItemPool.Enqueue(skill.gameObject, null);
            }
            gameObject.SetActive(false);
        }

        public override void StateOn(UINavigateParam param)
        {
            gameObject.SetActive(true);
            WebClient.GetInstance().MakeCharacterInfoRequest(param.pString);
        }

        public override void Init()
        {
            WebClient.GetInstance().responseCallback += (response) =>
            {
                if (response is WebClient.CharacterinfoResponse)
                {
                    var info = (WebClient.CharacterinfoResponse)response;

                    maxHp.text = info.status.maxHp.ToString();
                    maxMp.text = info.status.maxMp.ToString();
                    attack.text = info.status.attack.ToString();
                    defence.text = info.status.defence.ToString();
                    criticalRate.text = info.status.criticalRate.ToString()+"%";
                    criticalDamage.text = info.status.criticalDamage.ToString();
                    attackSpeed.text = info.status.attackSpeed.ToString();
                    characterIllust.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.characterFullbodyPath+info.name);

                    for(int i = 0; i < info.skillSet.Length; i++)
                    {
                        skillDescriptionItemPool.Dequeue(null);
                    }
                    var items = skillDescriptionContainer.GetComponentsInChildren<SkillDescriptionItem>();
                    for (int i = 0; i < info.skillSet.Length; i++)
                    {
                        items[i].SetSkill(info.skillSet[i]);
                    }
                    loadingMask.SetActive(false);
                }
            };
            skillDescriptionItemPool.Init(skillDescriptionItem, skillDescriptionItemCount, skillDescriptionContainer);
        }
        #endregion
    }
}
