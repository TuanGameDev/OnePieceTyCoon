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
using _Game.Scripts.Manager;

namespace _Game.Scripts.UI
{
    public class UserManagerUI : Singleton<UserManagerUI>
    {
        [SerializeField]
        private RankingManager _rankingManager;

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

        [SerializeField]
        private TextMeshProUGUI _userBeliTxt;

        [SerializeField]
        private TextMeshProUGUI _userDiamondTxt;

        #region DisPlayUser

        public Image AvatarUser;

        #endregion

        private void Awake()
        {
            if (_rankingManager == null)
            {
                _rankingManager = FindObjectOfType<RankingManager>();
            }
        }

        private void Start()
        {
            _selectedBtn.interactable = false;
            _userNameInputField.onValueChanged.AddListener(ValidateUserName);

            _userNameInputField.onSelect.AddListener(delegate { OnInputFieldSelected(_userNameInputField); });
            LoadDataUser();
            Invoke(nameof(UpdateDisplayUser), 1f);

            HeroManager.Instance.OnAddHero += RecalculateCombatPower;
            HeroManager.Instance.OnRemoveHero += RecalculateCombatPower;
        }
        private void OnInputFieldSelected(TMP_InputField inputField)
        {
            if (inputField != null)
            {
                inputField.ActivateInputField();
                StartCoroutine(UpdateInputField(inputField));
            }
        }

        private IEnumerator UpdateInputField(TMP_InputField inputField)
        {
            TouchScreenKeyboard keyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default);

            while (!keyboard.done)
            {
                if (keyboard.active)
                {
                    inputField.text = keyboard.text;
                }
                yield return null;
            }
            inputField.text = keyboard.text;
        }
        private void OnDestroy()
        {
            HeroManager.Instance.OnAddHero -= RecalculateCombatPower;
            HeroManager.Instance.OnRemoveHero -= RecalculateCombatPower;
        }

        public void RecalculateCombatPower()
        {
            int totalPower = 0;
            foreach (var hero in HeroManager.Instance.GetAvailableHeroes())
            {
                totalPower += hero.Power;
            }

            _rankingManager.UserInformation.CombatPower = totalPower;
            UpdateDisplayUser();
            UpdateLeaderboards(_rankingManager.UserInformation.CombatPower);
            SaveUserDataToPlayFab();
        }

        public void RewardCoin()
        {
            _rankingManager.UserInformation.Beli += 100000;
            _rankingManager.UserInformation.Diamond += 100000;
        }

        public void UpdateDisplayUser()
        {
            _userCombatPowerTxt.text = _rankingManager.UserInformation.CombatPower.ToString("N0");
            _userBeliTxt.text = _rankingManager.UserInformation.Beli.ToString("N0");
            _userDiamondTxt.text = _rankingManager.UserInformation.Diamond.ToString("N0");
        }

        private void LoadDataUser()
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), accountInfoResult =>
            {
                if (accountInfoResult.AccountInfo != null && !string.IsNullOrEmpty(accountInfoResult.AccountInfo.TitleInfo.DisplayName))
                {
                    _rankingManager.UserInformation.UserName = accountInfoResult.AccountInfo.TitleInfo.DisplayName;

                    PlayFabClientAPI.GetUserData(new GetUserDataRequest
                    {
                        PlayFabId = _rankingManager.UserInformation.MasterPlayerID
                    },
                    userDataResult =>
                    {
                        if (userDataResult.Data != null && userDataResult.Data.ContainsKey("UserInfomation"))
                        {
                            var userInformation = JsonUtility.FromJson<UserInformation>(userDataResult.Data["UserInfomation"].Value);

                            _rankingManager.UserInformation.CombatPower = userInformation.CombatPower;
                            _rankingManager.UserInformation.Beli = userInformation.Beli;
                            _rankingManager.UserInformation.Diamond = userInformation.Diamond;

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
                else
                {
                    ShowUserNamePopup();
                }
            },
            accountInfoError =>
            {
                Debug.LogError("Error fetching account info: " + accountInfoError.GenerateErrorReport());
            });
        }


        private void UpdateUserInfo()
        {
            _userNameTxt.text = _rankingManager.UserInformation.UserName;
            _userLevelTxt.text = "Lv. " + _rankingManager.UserInformation.UserLevel.ToString();
            _userCombatPowerTxt.text = "Power: " + _rankingManager.UserInformation.CombatPower.ToString("N0");
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
            _selectedBtn.interactable = userName.Length >= 5 && userName.Length <= 10;
        }

        public void SaveUserDataToPlayFab()
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = _userNameInputField.text
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
            {
                _rankingManager.UserInformation.UserName = result.DisplayName;
                UpdateUserInfo();
                HideUserNamePopup();
            },
            error =>
            {
                //Debug.LogError("Error updating DisplayName: " + error.GenerateErrorReport());
            });

            string playerDataJson = JsonUtility.ToJson(_rankingManager.UserInformation);

            var dataRequest = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string> { { "UserInfomation", playerDataJson } }
            };

            PlayFabClientAPI.UpdateUserData(dataRequest, OnDataSendSuccess, OnDataSendError);
        }

        public void SaveUserInformation()
        {
            string playerDataJson = JsonUtility.ToJson(_rankingManager.UserInformation);

            var dataRequest = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
        {
            { "UserInfomation", playerDataJson } 
        }
            };

            PlayFabClientAPI.UpdateUserData(dataRequest, OnSaveSuccess, OnSaveError);
        }

        private void OnSaveSuccess(UpdateUserDataResult result)
        {
            //Debug.Log("Beli and Diamond values saved successfully!");
        }

        private void OnSaveError(PlayFabError error)
        {
           // Debug.LogError("Error saving Beli and Diamond: " + error.GenerateErrorReport());
        }

        private void OnDataSendSuccess(UpdateUserDataResult result)
        {
           // Debug.Log("User data saved successfully!");
        }

        private void OnDataSendError(PlayFabError error)
        {
          //  Debug.LogError("Error saving user data: " + error.GenerateErrorReport());
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
           // Debug.LogError("Error updating leaderboard: " + error.GenerateErrorReport());
        }
    }
}
