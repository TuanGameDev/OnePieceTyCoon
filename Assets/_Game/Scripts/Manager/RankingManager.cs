using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using _Game.Scripts.Helper;
using TMPro;

namespace _Game.Scripts.Manager
{

    [System.Serializable]
    public class UserInformation
    {
        public string UserName;
        public int UserLevel = 1;
        public string MasterPlayerID;
        public int CombatPower = 0;
        public int Beli = 0;
        public int Diamond = 0;
    }

    public class RankingManager : MonoBehaviour
    {
        public UserInformation UserInformation;

        public List<UserInformation> Users = new List<UserInformation>();
        public static RankingManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        [Button("FetchLeaderboard")]
        public void FetchLeaderboard()
        {
            var request = new GetLeaderboardRequest
            {
                StatisticName = "CombatPower",
                StartPosition = 0,
                MaxResultsCount = 10
            };
            PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardFetchSuccess, OnLeaderboardFetchError);
        }

        private void OnLeaderboardFetchSuccess(GetLeaderboardResult result)
        {
            Users.Clear();

            foreach (var item in result.Leaderboard)
            {
                Users.Add(new UserInformation
                {
                    UserName = item.DisplayName,
                    MasterPlayerID = item.PlayFabId,
                    CombatPower = item.StatValue,
                });
            }
        }

        private void OnLeaderboardFetchError(PlayFabError error)
        {
            Debug.LogError("Error fetching leaderboard: " + error.GenerateErrorReport());
        }

        public void AddBeli(int amount)
        {
            UserInformation.Beli += amount;
        }

        public void AddDiamond(int amount)
        {
            UserInformation.Diamond += amount;
        }

        public void UpdateBeliAndDiamondText(TextMeshProUGUI beliTxt, TextMeshProUGUI diamondTxt)
        {
            beliTxt.text = FormatNumber(UserInformation.Beli);
            diamondTxt.text = FormatNumber(UserInformation.Diamond);
        }

        private string FormatNumber(long number)
        {
            if (number >= 1_000_000)
                return (number / 1_000_000f).ToString("0.#") + "m";
            else if (number >= 1_000)
                return (number / 1_000).ToString("0.#") + "k";
            else
                return number.ToString("N0");
        }
    }
}
