using PlayFab.ClientModels;
using PlayFab;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using _Game.Scripts.Helper;
using System;

namespace _Game.Scripts.Manager
{
    public class UserManagerUI : Singleton<UserManagerUI>
    {
        public UserInfomation UserInfomation;

        [SerializeField]
        private TMP_InputField _userNameInputField;

        [SerializeField]
        private GameObject UserNamePopup;

        [SerializeField]
        private Button _selectedBtn;

        [SerializeField]
        private TextMeshProUGUI _userNameTxt;

        [SerializeField]
        private TextMeshProUGUI _userLevelTxt;

        [SerializeField]
        private TextMeshProUGUI _userCombatPowerTxt;

        private void Start()
        {
            _selectedBtn.interactable = false;
            _userNameInputField.onValueChanged.AddListener(ValidateUserName);
            _selectedBtn.onClick.AddListener(SaveUserDataToPlayFab);
            HeroManager.Instance.OnAddHero += UpdateUserInfo;

            CheckIfUserNameExists();
        }
        private void OnDestroy()
        {
            HeroManager.Instance.OnAddHero -= UpdateUserInfo;
        }
        public void AddCombatPower(int amount)
        {
            UserInfomation.CombatPower += amount;
            UpdateLeaderboards(UserInfomation.CombatPower);
            SaveUserDataToPlayFab();
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
            _userLevelTxt.text ="Lv. " + UserInfomation.UserLevel.ToString();
            _userCombatPowerTxt.text = "Power: " + UserInfomation.CombatPower.ToString("N0");
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
                _selectedBtn.interactable = true;
            }
            else
            {
                _selectedBtn.interactable = false;
            }
        }
        public void SaveUserDataToPlayFab()
        {
            if (_selectedBtn.interactable)
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
