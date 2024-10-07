using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.Manager
{
    public class PlayFabManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _emailInputField;

        [SerializeField]
        private TMP_InputField _passwordInputField;

        [SerializeField]
        private TMP_InputField _forgotPasswordInputField;

        [SerializeField]
        private TextMeshProUGUI _messageText;

        [SerializeField]
        private Button _loginBtn;

        [SerializeField]
        private Button _registerBtn;

        [SerializeField]
        private Button _forgotPasswordBtn;

        [SerializeField]
        private Canvas _playfabPopup;

        [SerializeField]
        private Canvas _gameUIPopup;

        [ReadOnly]
        public string PlayFabId;

        public static PlayFabManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _loginBtn.onClick.AddListener(Login);
            _registerBtn.onClick.AddListener(Register);
            _forgotPasswordBtn.onClick.AddListener(ForgotPassword);

            // Load saved email and password
            LoadAccountInfo();
        }

        public void SetPlayFabId(string playFabId)
        {
            PlayFabId = playFabId;
        }

        private bool ValidateInputFields()
        {
            if (string.IsNullOrEmpty(_emailInputField.text) || string.IsNullOrEmpty(_passwordInputField.text))
            {
                _messageText.text = "Vui lòng nhập tài khoản và mật khẩu!";
                return false;
            }
            return true;
        }

        public void Register()
        {
            if (!ValidateInputFields()) return;

            var registerRequest = new RegisterPlayFabUserRequest
            {
                Email = _emailInputField.text,
                Password = _passwordInputField.text,
                RequireBothUsernameAndEmail = false
            };

            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnError);
        }

        private void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            _messageText.text = "Đăng ký thành công!";
            SaveAccountInfo();  // Save the account after registration
            StartCoroutine(HideTxt(new WaitForSeconds(3)));
        }

        public void Login()
        {
            if (!ValidateInputFields()) return;

            var loginRequest = new LoginWithEmailAddressRequest
            {
                Email = _emailInputField.text,
                Password = _passwordInputField.text,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetUserAccountInfo = true
                }
            };

            PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnError);
        }

        private void OnLoginSuccess(LoginResult result)
        {
            var accountInfo = result.InfoResultPayload.AccountInfo;
            if (accountInfo != null && accountInfo.PrivateInfo != null)
            {
                string playerId = result.PlayFabId;
                SetPlayFabId(playerId);
                _messageText.text = "Đăng nhập thành công!";
                HeroManager.Instance.LoadDataHero();
                _playfabPopup.enabled = false;
                _gameUIPopup.enabled = true;
                if (string.IsNullOrEmpty(accountInfo.PrivateInfo.Email))
                {
                    _messageText.text = "Email chưa được xác minh. Hãy kiểm tra email của bạn.";
                }

                SaveAccountInfo();
                StartCoroutine(HideTxt(new WaitForSeconds(3)));
            }
        }

        public void ForgotPassword()
        {
            if (string.IsNullOrEmpty(_forgotPasswordInputField.text))
            {
                _messageText.text = "Vui lòng nhập email để khôi phục mật khẩu!";
                return;
            }

            var passwordRecoveryRequest = new SendAccountRecoveryEmailRequest
            {
                Email = _forgotPasswordInputField.text,
                TitleId = PlayFabSettings.TitleId
            };

            PlayFabClientAPI.SendAccountRecoveryEmail(passwordRecoveryRequest, OnPasswordRecoverySuccess, OnError);
        }

        private void OnPasswordRecoverySuccess(SendAccountRecoveryEmailResult result)
        {
            _messageText.text = "Email khôi phục mật khẩu đã được gửi! Vui lòng kiểm tra hộp thư của bạn.";
            StartCoroutine(HideTxt(new WaitForSeconds(3)));
        }

        // Method to hide the text message after a delay
        public IEnumerator HideTxt(WaitForSeconds delay)
        {
            yield return delay;
            _messageText.text = "";
        }

        private void OnError(PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
            {
                _messageText.text = "Email đã tồn tại. Vui lòng sử dụng email khác.";
            }
            else
            {
                _messageText.text = "Tài khoản hoặc mật khẩu không chính xác.";
            }

            StartCoroutine(HideTxt(new WaitForSeconds(3)));
        }

        private void SaveAccountInfo()
        {
            PlayerPrefs.SetString("SavedEmail", _emailInputField.text);
            PlayerPrefs.SetString("SavedPassword", _passwordInputField.text);
            PlayerPrefs.Save();
        }

        private void LoadAccountInfo()
        {
            if (PlayerPrefs.HasKey("SavedEmail") && PlayerPrefs.HasKey("SavedPassword"))
            {
                _emailInputField.text = PlayerPrefs.GetString("SavedEmail");
                _passwordInputField.text = PlayerPrefs.GetString("SavedPassword");
            }
        }
    }
}
