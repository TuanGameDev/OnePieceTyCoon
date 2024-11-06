using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Game.Scripts.Manager
{
    public class PlayFabManager : MonoBehaviour
    {

        public bool isLoggedIn;

        public static PlayFabManager Instance;

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            Login();
        }
        public void Login()
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }
        private void OnLoginSuccess(LoginResult result)
        {
            RankingManager.Instance.UserInformation.MasterPlayerID = result.PlayFabId;
            isLoggedIn = true;
            Debug.Log("Đăng nhập thành công với PlayFabId: " + RankingManager.Instance.UserInformation.MasterPlayerID);
        }

        private void OnLoginFailure(PlayFabError error)
        {
            Debug.LogError("Đăng nhập thất bại: " + error.GenerateErrorReport());
        }

        [Button("Delete Account")]
        public void DeleteAccount()
        {
            if (!isLoggedIn)
            {
                Debug.LogError("Bạn cần đăng nhập trước khi xóa tài khoản.");
                return;
            }

            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "DeletePlayer",
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request, OnDeleteSuccess, OnDeleteFailure);
        }
        private void OnDeleteSuccess(ExecuteCloudScriptResult result)
        {

        }
        private void OnDeleteFailure(PlayFabError error)
        {
            Debug.LogError("Xóa tài khoản thất bại: " + error.GenerateErrorReport());
        }
    }
}
