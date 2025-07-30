using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class BuffDisplayItem : MonoBehaviour
    {
        public Guid buffId;
        [SerializeField] Image buffImage;

        public void SetBuff(Buff buff)
        {
            buffId = buff.id;
            buffImage.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.buffIconPath + buff.GetType().Name);
        }
    }
}
