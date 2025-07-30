using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtentsPro
{
    public class HomeUI : UINavigatable
    {
        [SerializeField] TextMeshProUGUI nickname;
        [SerializeField] TextMeshProUGUI level;
        [SerializeField] TextMeshProUGUI gold;
        [SerializeField] TextMeshProUGUI jem;
        [SerializeField] Image profileImage;
        [SerializeField] Image exp;

        [Header("홈 캐릭터 프리뷰")]
        [SerializeField] Transform homeCharacterTransform;
        [SerializeField] GameObject homeCharacterPrefab;
        Character homeCharacter;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OpenBattleUI()
        {
            UINavigationSystem.GetInstance().ChangeState("_battle", new UINavigateParam());
            HomeNavigation.GetInstance().SetActive(false);
        }

        #region UINavigatable
        public override GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public override void StateOff()
        {
            homeCharacter.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public override void StateOn(UINavigateParam param)
        {
            HomeNavigation.GetInstance().SetActive(true);

            gameObject.SetActive(true);
            nickname.text = GameManager.GetInstance().userInfo.name;
            level.text = GameManager.GetInstance().userInfo.level.ToString();
            gold.text = GameManager.GetInstance().userInfo.currency.gold.ToString();
            jem.text = GameManager.GetInstance().userInfo.currency.jem.ToString();

            exp.fillAmount = GameManager.GetInstance().userInfo.exp;
            profileImage.sprite = Resources.Load<Sprite>(AtentsProGlobalSettings.Instance.profileImagePath + GameManager.GetInstance().userInfo.profile);

            CharacterAsset homeCharacterAsset = Resources.Load<CharacterAsset>(AtentsProGlobalSettings.Instance.characterAssetPath + GameManager.GetInstance().userInfo.homeCharacter);
            homeCharacter.SetAsset(homeCharacterAsset);
            homeCharacter.gameObject.SetActive(true);
        }

        public override void Init()
        {
            GameObject obj = Instantiate(homeCharacterPrefab, homeCharacterTransform);
            homeCharacter = obj.GetComponent<Character>();
            obj.SetActive(false);
        }
        #endregion

    }
}
