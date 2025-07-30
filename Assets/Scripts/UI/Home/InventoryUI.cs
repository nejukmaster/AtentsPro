using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class InventoryUI : UINavigatable
    {
        [SerializeField] ItemContainer[] containers;


        [Header("아이템 상세정보 UI")]
        [SerializeField] GameObject itemInfoUI;
        [SerializeField] TextMeshProUGUI itemNameTmp;
        [SerializeField] TextMeshProUGUI itemDescriptionTmp;
        [SerializeField] Button useButton;

        [Header("아이템 사용시 결과창")]
        [SerializeField] GameObject useItemResultUI;
        [SerializeField] TextMeshProUGUI useItemResultTmp;

        WebClient.ItemInfo selectedItem;

        public void OnBack()
        {
            UINavigationSystem.GetInstance().ChangeState("_home", new UINavigateParam());
        }

        public void UpdateInventory()
        {
            for(int i = 0; i < containers.Length; i++)
            {
                if (i < GameManager.GetInstance().userInfo.inventory.Length)
                {
                    containers[i].SetItem(GameManager.GetInstance().userInfo.inventory[i]);
                    containers[i].clickCallback = (info) => {
                        itemInfoUI.SetActive(true);
                        itemNameTmp.text = DataSheetLoader.GetInstance().itemTable[info.id].name;
                        itemDescriptionTmp.text = DataSheetLoader.GetInstance().itemTable[info.id].description;
                        useButton.gameObject.SetActive(DataSheetLoader.GetInstance().itemTable[info.id].canUse);
                        selectedItem = info;
                    };
                }
                else
                {
                    containers[i].Clear();
                    containers[i].clickCallback = null;
                }
            }
        }

        public void OnItemInfoConfirm()
        {
            itemInfoUI.SetActive(false);
        }

        public void OnItemInfoUse()
        {
            UINavigationSystem.GetInstance().loadingUI.SetActive(true);
            WebClient.GetInstance().MakeUseItemRequest(selectedItem.id, (response) => {
                useItemResultUI.SetActive(true);
                useItemResultTmp.SetText(response.message);
                GameManager.GetInstance().userInfo.currency = response.updatedUserInfo.currency;
                GameManager.GetInstance().userInfo.inventory = response.updatedUserInfo.inventory;
                UpdateInventory();
                UINavigationSystem.GetInstance().loadingUI.SetActive(false);
            });
        }

        public void OnUseItemResultConfirm()
        {
            useItemResultUI.SetActive(false);
        }
        #region UINavigatable
        public override GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public override void StateOff()
        {
            gameObject.SetActive(false);
            HomeNavigation.GetInstance().SetActive(true);
        }

        public override void StateOn(UINavigateParam param)
        {
            gameObject.SetActive(true);
            HomeNavigation.GetInstance().SetActive(false);
            UpdateInventory();
        }
        #endregion
    }
}
