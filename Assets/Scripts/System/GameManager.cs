using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SceneManagement;
using static AtentsPro.WebClient;

namespace AtentsPro
{
    [System.Serializable]
    public struct HomeToBattle
    {
        public string stageName;
        public Dictionary<string,WebClient.CharacterinfoResponse> characterDic;
        public string[] team;
        public WebClient.SubstageInfo[] substages;
    }

    [System.Serializable]
    public struct BattleToHome
    {
        public List<KeyValuePair<string, int>> rewards;
    }

    [System.Serializable]
    public struct SceneLoadParam
    {
        public Action actionToLoad;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static GameManager GetInstance() { return Instance; }

        public string userId;
        public string jwt;

        public WebClient.UserInfoResponse userInfo;
        public WebClient.GlobalInfoResponse globalInfo;

        public SceneLoadParam sceneLoadParam;

        public HomeToBattle homeToBattle;
        public BattleToHome battleToHome;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(Instance);
            }
        }

        public void LoadLoadingScene(SceneLoadParam param)
        {
            sceneLoadParam = param;
            SceneManager.LoadScene(AtentsProGlobalSettings.Instance.scenesPath+"Loading");
        }
    }
}
