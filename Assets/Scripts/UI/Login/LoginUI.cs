using UnityEngine;
using UnityEngine.UIElements;

namespace AtentsPro
{
    public class LoginUI : MonoBehaviour
    {
        private TextField usernameField;
        private TextField passwordField;
        private Button loginButton;

        private Button _closeButton;

        private VisualElement _popupContainer;

        private const string POPUP_HIDDEN_CLASS = "popup-hidden";
        private const string POPUP_VISIBLE_CLASS = "popup-visible";

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            usernameField = root.Q<TextField>("usernameField");
            passwordField = root.Q<TextField>("passwordField");
            loginButton = root.Q<Button>("loginButton");

            _closeButton = root.Q<Button>("closeButton");
            _popupContainer = root.Q<VisualElement>("popupContainer");

            loginButton.clicked += OnLoginClicked;
            
            _closeButton.RegisterCallback<ClickEvent>(evt => HidePopup());
        }

        public void Start()
        {
            WebClient.GetInstance().responseCallback += (response) => {
                if (response is WebClient.LoginResponse)
                {
                    var login = (WebClient.LoginResponse)response;
                    if (login.status != "success")
                    {
                        ShowPopup();
                    }
                }
            };
        }

        private void OnLoginClicked()
        {
            string username = usernameField.value;
            string password = passwordField.value;

            WebClient.GetInstance().MakeLoginRequest(username, password);
        }

        private void ShowPopup()
        {
            _popupContainer.RemoveFromClassList(POPUP_HIDDEN_CLASS);
            _popupContainer.AddToClassList(POPUP_VISIBLE_CLASS);
        }

        private void HidePopup()
        {
            _popupContainer.RemoveFromClassList(POPUP_VISIBLE_CLASS);
            _popupContainer.AddToClassList(POPUP_HIDDEN_CLASS);
        }
    }
}
