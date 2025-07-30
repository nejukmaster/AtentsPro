using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.LookDev;
using UnityEngine.TextCore.Text;

namespace AtentsPro
{
    public class WebClient : MonoBehaviour
    {
        public static WebClient instance;
        public static WebClient GetInstance() { return instance; }

        #region POST 메서드용 Body 구조체
        [System.Serializable]
        public struct LoginPostBody
        {
            public string id;
            public string password;
        }

        [System.Serializable]
        public struct RecruitPostBody
        {
            public string userId;
            public string eventId;
        }

        [System.Serializable]
        public struct StageStartPostBody
        {
            public string userId;
            public string stage;
        }

        [System.Serializable]
        public struct UpdateCharacterLineupBody
        {
            public string userId;
            public int index;
            public string character;
        }

        [System.Serializable]
        public struct UseItemBody
        {
            public string userId;
            public string itemId;
        }
        #endregion

        #region Json 파싱용 구조체 & 클래스
        [System.Serializable]
        public struct ItemInfo
        {
            public string id;
            public int count; 
        }

        [System.Serializable]
        public struct UserCurrencyInfo
        {
            public int gold;
            public int jem;
        }

        [System.Serializable]
        public struct StageInfo
        {
            public string name;
            public bool isCleared;
        }

        [System.Serializable]
        public struct SubstageInfo
        {
            public string[] enemies;
        }

        public class WebClientResponse { }

        [System.Serializable]
        public class LoginResponse : WebClientResponse
        {
            public string status;
            public string message;
            public string token;
            public string id;
        }

        [System.Serializable]
        public class EndStageResponse : WebClientResponse
        {
            public List<ItemInfo> rewards;
        }

        [System.Serializable]
        public class UserInfoResponse : WebClientResponse
        {
            public string name;
            public int level;
            public float exp;
            public string profile;
            public UserCurrencyInfo currency;
            public StageInfo[] enableStages;
            public string homeCharacter;
            public string[] owningCharacters;
            public string[] characterLineup;
            public ItemInfo[] inventory;
        }

        [System.Serializable]
        public class CharacterinfoResponse : WebClientResponse
        {
            public string name;
            public Status status;
            public string targetingMethod;
            public string[] skillSet;
        }

        [System.Serializable]
        public class GlobalInfoResponse : WebClientResponse
        {
            public string[] events;
        }

        [System.Serializable]
        public class StageStartResponse : WebClientResponse
        {
            public string name;
            public bool canEnter;
            public CharacterinfoResponse[] characters;
            public string[] team;
            public SubstageInfo[] substages;
        }

        [System.Serializable]
        public class RecruitResultResponse : WebClientResponse
        {
            public string status;
            public string message;
            public string recruitedCharacter;
            public UserInfoResponse updatedUserInfo;
        }

        [System.Serializable]
        public class UseItemResponse : WebClientResponse
        {
            public string status;
            public string message;
            public UserInfoResponse updatedUserInfo;
        }
        #endregion

        public string serverAddress;
        public Action<WebClientResponse> responseCallback;

        private void Awake()
        {
            instance = this;
        }

        #region GET Request 메서드
        
        //스테이지 완료 요청
        public void MakeEndStageRequest(string stageName)
        {
            IEnumerator Co()
            {
                var webRequest = UnityWebRequest.Get(serverAddress + $"/end_stage?stage={stageName}&userId={GameManager.GetInstance().userId}");
                webRequest.SetRequestHeader("Authorization", "Bearer " + GameManager.GetInstance().jwt);
                yield return webRequest.SendWebRequest();
                if(webRequest.error == null)
                {
                    EndStageResponse response = JsonUtility.FromJson<EndStageResponse>(webRequest.downloadHandler.text);
                    responseCallback?.Invoke(response);

                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
            StartCoroutine(Co());
        }
        //유저 정보 요청
        public void MakeUserInfoRequest(string userId)
        {
            IEnumerator Co()
            {
                var webRequest = UnityWebRequest.Get(serverAddress + $"/user_info?userId={userId}");
                webRequest.SetRequestHeader("Authorization", "Bearer " + GameManager.GetInstance().jwt);
                yield return webRequest.SendWebRequest();
                if (webRequest.error == null)
                {
                    UserInfoResponse response = JsonUtility.FromJson<UserInfoResponse>(webRequest.downloadHandler.text);
                    responseCallback?.Invoke(response);

                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
            StartCoroutine(Co());
        }
        //캐릭터 정보 요청
        public void MakeCharacterInfoRequest(string characterName)
        {
            IEnumerator Co()
            {
                var webRequest = UnityWebRequest.Get(serverAddress + $"/character_info?character={characterName}");
                yield return webRequest.SendWebRequest();
                if (webRequest.error == null)
                {
                    CharacterinfoResponse response = JsonUtility.FromJson<CharacterinfoResponse>(webRequest.downloadHandler.text);
                    responseCallback?.Invoke(response);

                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
            StartCoroutine(Co());
        }
        //글로벌 정보요청(공지, 진행중인 이벤트 등등)
        public void MakeGlobalInfoRequest()
        {
            IEnumerator Co()
            {
                var webRequest = UnityWebRequest.Get(serverAddress + "/global_info");
                yield return webRequest.SendWebRequest();
                if (webRequest.error == null)
                {
                    GlobalInfoResponse response = JsonUtility.FromJson<GlobalInfoResponse>(webRequest.downloadHandler.text);
                    responseCallback?.Invoke(response);

                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
            StartCoroutine(Co());
        }
        #endregion

        #region POST Request 메서드
        //로그인 요청
        public void MakeLoginRequest(string id, string password)
        {
            IEnumerator Co()
            {
                var loginJson = JsonUtility.ToJson(new LoginPostBody { id = id, password = password });
                var webRequest = new UnityWebRequest(serverAddress + "/login", "POST");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(loginJson);

                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();
                if (webRequest.error == null)
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(webRequest.downloadHandler.text);
                    responseCallback?.Invoke(response);
                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
            StartCoroutine(Co());
        }

        //스테이지 개시 요청
        public void MakeStageStartRequest(string stageName)
        {
            IEnumerator Co()
            {
                var stageStartJson = JsonUtility.ToJson(new StageStartPostBody { userId = GameManager.GetInstance().userId, stage = stageName });
                var webRequest = new UnityWebRequest(serverAddress + $"/start_stage?userId={GameManager.GetInstance().userId}", "POST");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(stageStartJson);

                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", "Bearer " + GameManager.GetInstance().jwt);

                yield return webRequest.SendWebRequest();
                if (webRequest.error == null)
                {
                    StageStartResponse response = JsonUtility.FromJson<StageStartResponse>(webRequest.downloadHandler.text);
                    responseCallback?.Invoke(response);
                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
            StartCoroutine(Co());
        }

        //캐릭터 모집 요청
        public void MakeRecruitRequest(string eventId, Action<RecruitResultResponse> callback)
        {
            IEnumerator Co()
            {
                var stageStartJson = JsonUtility.ToJson(new RecruitPostBody { userId = GameManager.GetInstance().userId, eventId = eventId });
                var webRequest = new UnityWebRequest(serverAddress + $"/recruit?userId={GameManager.GetInstance().userId}", "POST");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(stageStartJson);

                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", "Bearer " + GameManager.GetInstance().jwt);

                yield return webRequest.SendWebRequest();
                if (webRequest.error == null)
                {
                    RecruitResultResponse response = JsonUtility.FromJson<RecruitResultResponse>(webRequest.downloadHandler.text);
                    callback?.Invoke(response);
                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
            StartCoroutine(Co());
        }

        //캐릭터 파티 편성 변경 요청
        public void MakeUpdateCharacterLineupRequest(int index, string character, Action<UserInfoResponse> callback)
        {
            IEnumerator Co()
            {
                var updateCharacterLineupJson = JsonUtility.ToJson(new UpdateCharacterLineupBody { userId = GameManager.GetInstance().userId, index = index, character = character });
                var webRequest = new UnityWebRequest(serverAddress + $"/update_character_lineup?userId={GameManager.GetInstance().userId}", "POST");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(updateCharacterLineupJson);

                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", "Bearer " + GameManager.GetInstance().jwt);
                yield return webRequest.SendWebRequest();
                if (webRequest.error == null)
                {
                    UserInfoResponse response = JsonUtility.FromJson<UserInfoResponse>(webRequest.downloadHandler.text);
                    callback?.Invoke(response);
                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
            StartCoroutine(Co());
        }

        //아이템 사용 요청
        public void MakeUseItemRequest(string itemId, Action<UseItemResponse> callback)
        {
            IEnumerator Co()
            {
                var useItemJson = JsonUtility.ToJson(new UseItemBody { userId = GameManager.GetInstance().userId, itemId = itemId });
                var webRequest = new UnityWebRequest(serverAddress + $"/use_item?userId={GameManager.GetInstance().userId}", "POST");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(useItemJson);

                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", "Bearer " + GameManager.GetInstance().jwt);
                yield return webRequest.SendWebRequest();
                if (webRequest.error == null)
                {
                    UseItemResponse response = JsonUtility.FromJson<UseItemResponse>(webRequest.downloadHandler.text);
                    callback?.Invoke(response);
                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
            StartCoroutine(Co());
        }
        #endregion
    }
}
