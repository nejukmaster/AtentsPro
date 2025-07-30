using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AtentsPro
{
    public class LoginSystem : MonoBehaviour
    {
        public static LoginSystem instance;
        public static LoginSystem GetInstance() { return instance; }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            WebClient.GetInstance().responseCallback += (response) => {
                if(response is WebClient.LoginResponse)
                {
                    var login = (WebClient.LoginResponse)response;
                    if(login.status == "success")
                    {
                        GameManager.GetInstance().userId = login.id;
                        GameManager.GetInstance().jwt = login.token;
                        SceneLoadParam param = new SceneLoadParam
                        {
                            actionToLoad = () =>
                            {
                                WebClient.GetInstance().responseCallback += (response) =>
                                {
                                    if(response is WebClient.UserInfoResponse)
                                    {
                                        var info = (WebClient.UserInfoResponse)response;
                                        GameManager.GetInstance().userInfo = info;
                                        SceneManager.LoadScene(AtentsProGlobalSettings.Instance.scenesPath + "Home");
                                    }
                                    else if(response is WebClient.GlobalInfoResponse)
                                    {
                                        var info = (WebClient.GlobalInfoResponse)response;
                                        GameManager.GetInstance().globalInfo = info;
                                    }
                                };
                                WebClient.GetInstance().MakeGlobalInfoRequest();
                                WebClient.GetInstance().MakeUserInfoRequest(GameManager.GetInstance().userId);
                            }
                        };
                        GameManager.GetInstance().LoadLoadingScene(param);
                    }
                }
            };
        }
    }
}
