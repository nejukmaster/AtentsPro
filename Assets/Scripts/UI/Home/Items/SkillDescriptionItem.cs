using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class SkillDescriptionItem : MonoBehaviour
    {
        [SerializeField] Image skillIcon;
        [SerializeField] TextMeshProUGUI skillName;
        [SerializeField] TextMeshProUGUI description;

        public void SetSkill(string skillCode)
        {
            this.skillIcon.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.skillIconPath + skillCode);

            this.skillName.text = DataSheetLoader.GetInstance().skillTable[skillCode].name;
            this.description.text = DataSheetLoader.GetInstance().skillTable[skillCode].description;
        }
    }
}
