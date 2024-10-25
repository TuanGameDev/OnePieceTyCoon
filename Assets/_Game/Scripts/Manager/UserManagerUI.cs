using PlayFab.ClientModels;
using PlayFab;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace _Game.Scripts.Manager
{
    public class UserManagerUI : MonoBehaviour
    {
        public UserInfomation UserInfomation;

        [SerializeField]
        private TMP_InputField _userNameInputField;

        [SerializeField]
        private GameObject UserNamePopup;

        [SerializeField]
        private Button _continueBtn;

        [SerializeField]
        private TextMeshProUGUI _userNameTxt;

        [SerializeField]
        private TextMeshProUGUI _userLevelTxt;

        [SerializeField]
        private TextMeshProUGUI _userCombatPowerTxt;

        private void Start()
        {
            _continueBtn.interactable = false;
            _userNameInputField.onValueChanged.AddListener(ValidateUserName);
            _continueBtn.onClick.AddListener(SaveUserDataToPlayFab);

            CheckIfUserNameExists();
        }

        private void CheckIfUserNameExists()
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = PlayFabManager.Instance.PlayFabId
            },
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("UserInfomation"))
                {
                    UserInfomation = JsonUtility.FromJson<UserInfomation>(result.Data["UserInfomation"].Value);

                    HideUserNamePopup();
                    UpdateUserInfo();
                }
                else
                {
                    ShowUserNamePopup();
                }
            },
            error =>
            {
                Debug.LogError("Error fetching user data: " + error.GenerateErrorReport());
            });
        }

        private void UpdateUserInfo()
        {
            _userNameTxt.text = UserInfomation.UserName;
            _userLevelTxt.text ="Lv. " + UserInfomation.UserLevel.ToString("N0");
            _userCombatPowerTxt.text = "Lực chiến: " + UserInfomation.CombatPower.ToString("N0");
        }
        private void ShowUserNamePopup()
        {
            UserNamePopup.SetActive(true);
        }

        private void HideUserNamePopup()
        {
            UserNamePopup.SetActive(false);
        }

        private void ValidateUserName(string userName)
        {
            if (userName.Length >= 5 && userName.Length <= 10)
            {
                _continueBtn.interactable = true;
            }
            else
            {
                _continueBtn.interactable = false;
            }
        }
        private void SaveUserDataToPlayFab()
        {
            if (_continueBtn.interactable)
            {
                UserInfomation.UserName = _userNameInputField.text;

                string playerDataJson = JsonUtility.ToJson(UserInfomation);

                var request = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>
            {
                { "UserInfomation", playerDataJson }
            }
                };

                PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDataSendError);
                UpdateUserInfo();
            }
        }

        private void OnDataSendSuccess(UpdateUserDataResult result)
        {
            Debug.Log("User data saved successfully!");
            HideUserNamePopup();
        }

        private void OnDataSendError(PlayFabError error)
        {
            Debug.LogError("Error saving user data: " + error.GenerateErrorReport());
        }

        public void AddCombatPower(int amount)
        {
            UserInfomation.CombatPower += amount;
            UpdateLeaderboards(UserInfomation.CombatPower);
        }

        private void UpdateLeaderboards(int combatPower)
        {
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "CombatPower",
                        Value = combatPower
                    }
                }
            };

            PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdateSuccess, OnUpdateError);
        }

        private void OnUpdateSuccess(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("Combat Power updated successfully!");
        }

        private void OnUpdateError(PlayFabError error)
        {
            Debug.LogError("Error updating Combat Power: " + error.GenerateErrorReport());
        }
    }

    [System.Serializable]
    public class UserInfomation
    {
        public string UserName;
        public int UserLevel = 1;
        public int CombatPower = 0;
    }
}
