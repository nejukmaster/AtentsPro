using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class ItemContainer : MonoBehaviour
    {
        public Action<WebClient.ItemInfo> clickCallback;

        [SerializeField] Image itemImage;
        [SerializeField] TextMeshProUGUI itemCountTmp;

        WebClient.ItemInfo itemInfo;

        public void SetItem(WebClient.ItemInfo itemInfo)
        {
            this.itemInfo = itemInfo;
            itemImage.gameObject.SetActive(true);
            itemCountTmp.gameObject.SetActive(true);
            itemImage.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.itemIconPath + itemInfo.id);
            itemCountTmp.text = itemInfo.count.ToString();
        }

        public void Clear()
        {
            itemImage.gameObject.SetActive(false);
            itemCountTmp.gameObject.SetActive(false);
        }

        public void OnClick()
        {
            clickCallback?.Invoke(itemInfo);
        }
    }
}
