using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;

namespace _Game.Scripts.Manager
{
    public class RankingManager : MonoBehaviour
    {
        public List<User> Users = new List<User>();

        private void Start()
        {
            FetchLeaderboard();
        }
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
                Users.Add(new User
                {
                    Name = item.Position + 1,
                    MasterPlayerID = item.PlayFabId,
                    CombatPower = item.StatValue
                });
            }
        }

        private void OnLeaderboardFetchError(PlayFabError error)
        {
            Debug.LogError("Error fetching leaderboard: " + error.GenerateErrorReport());
        }
    }
}

[System.Serializable]
public class User
{
    public int Name;
    public string MasterPlayerID;
    public int CombatPower;
}
