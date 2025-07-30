using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AtentsPro
{
    public class SkillExecuteButton : MonoBehaviour
    {
        [SerializeField] GameObject mask;
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] Image Icon;

        [SerializeField] int skillIndex;
        [SerializeField] Character target;

        private List<Marker> instancedMarkers = new List<Marker>();

        private void Update()
        {
            if(target != null)
            {
                float skillCool = target.GetSkillCooltime(skillIndex);
                if (skillCool > 0)
                {
                    SetMask(true);
                    text.SetText(((Int32)skillCool).ToString());
                }
                else
                {
                    SetMask(false);
                }
            }
            else
            {
                Icon.color = new Color(0f, 0f, 0f, 0f);
            }
        }
        public void SetTarget(Character target)
        {
            this.target = target;
            Sprite icon = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.skillIconPath + target.GetSkill(skillIndex).skillCode);
            Icon.sprite = icon;
            Icon.color = new Color(1.0f,1.0f,1.0f,1.0f);
        }
        public void SetMask(bool pBool)
        {
            mask.SetActive(pBool);
        }
        public void OnClick()
        {
            //ĳ���Ϳ� ��Ŀ���� �Ǿ����� ������� ó��
            if (target == null || !GetComponentInParent<SkillUI>().bCanSkillCast) return;

            //ĳ������ ��ų�� ����� �������� ������� ó��
            if(target.GetSkill(skillIndex).GetTargetables(target) == null)
            {
                target.UseSkill(skillIndex, null);
                return;
            }

            //����� �����ϴ� ��ų ó��
            UINavigationSystem.GetInstance().ChangeState("skill", new UINavigateParam());
            HashSet<IDamagable> targetables = target.GetSkill(skillIndex).GetTargetables(target);
            foreach(IDamagable targetable in targetables)
            {
                if (!(targetable is MonoBehaviour)) break;

                Vector2 screenPos = Camera.main.WorldToScreenPoint(targetable.GetTransform().position + new Vector3(0f, 0.5f, 0f));
                Marker marker = MarkerSystem.GetInstance().DequeueMarker("skill_targeting", screenPos);
                marker.SetTracking((obj) =>
                {
                    Vector2 newPos = Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0f, 0.5f, 0f));
                    return newPos;
                });
                marker.SetMarkedObj(targetable.GetTransform().gameObject);
                marker.clickAction = (obj) => { 
                    target.UseSkill(skillIndex, obj.GetComponent<Character>()); 
                    foreach(Marker m in instancedMarkers)
                    {
                        MarkerSystem.GetInstance().EnqueueMarker(m);
                    }
                    UINavigationSystem.GetInstance().ChangeState("main", new UINavigateParam());
                    instancedMarkers.Clear();
                };
                instancedMarkers.Add(marker);
            }
        }
    }
}
