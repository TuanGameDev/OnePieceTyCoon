using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using _Game.Scripts.Helper;
using _Game.Scripts.Manager;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace _Game.Scripts.UI
{
    public class RankingBossUI : Singleton<RankingBossUI>
    {
        [SerializeField]
        private SlotRankingBossUI _slotRankingBossUIPrefab;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private List<UserBoss> _userBosses;

        private List<SlotRankingBossUI> _slots;

        public int ScoreUser;

        private const string ScoreUserKey = "ScoreUser";

        private void Start()
        {
            LoadScoreUser();
        }

        public void AddScoreBoss(int scoreAmount)
        {
            ScoreUser += scoreAmount;
            UpdateLeaderboards(ScoreUser);

            SaveScoreUser();
        }

        private void UpdateLeaderboards(int totalScore)
        {
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "RankingBoss",
                        Value = totalScore
                    }
                }
            };

            PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdateSuccess, OnUpdateError);
        }

        private void OnUpdateSuccess(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("Score updated successfully!");
        }

        private void OnUpdateError(PlayFabError error)
        {
            Debug.LogError("Error updating leaderboard: " + error.GenerateErrorReport());
        }

        public void FetchLeaderboard()
        {
            var request = new GetLeaderboardRequest
            {
                StatisticName = "RankingBoss",
                StartPosition = 0,
                MaxResultsCount = 10
            };

            PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardFetchSuccess, OnLeaderboardFetchError);
        }

        private void OnLeaderboardFetchSuccess(GetLeaderboardResult result)
        {
            if (_userBosses == null)
            {
                _userBosses = new List<UserBoss>();
            }

            _userBosses.Clear();

            foreach (var item in result.Leaderboard)
            {
                _userBosses.Add(new UserBoss
                {
                    MasterIDUser = item.PlayFabId,
                    Top = item.Position + 1,
                    NameUser = string.IsNullOrEmpty(item.DisplayName) ? "Anonymous" : item.DisplayName,
                    Score = item.StatValue
                });
            }

            Debug.Log($"Fetched {_userBosses.Count} users from leaderboard.");
            UpdateUI();
        }


        private void OnLeaderboardFetchError(PlayFabError error)
        {
            Debug.LogError("Error fetching leaderboard: " + error.GenerateErrorReport());
        }

        private void UpdateUI()
        {
            if (_slots == null)
            {
                _slots = new List<SlotRankingBossUI>();
            }

            foreach (var slot in _slots)
            {
                if (slot != null)
                {
                    Destroy(slot.gameObject);
                }
            }
            _slots.Clear();

            if (_userBosses == null || _userBosses.Count == 0)
            {
                return;
            }

            foreach (var userBoss in _userBosses)
            {
                var userBossLocal = userBoss;

                var request = new GetUserDataRequest
                {
                    PlayFabId = userBossLocal.MasterIDUser
                };

                PlayFabClientAPI.GetUserData(request, result =>
                {
                    if (result.Data != null)
                    {
                        if (result.Data.ContainsKey("AvatarPath"))
                        {
                            string avatarPath = result.Data["AvatarPath"].Value;
                            string fullPath = "Portrait/" + avatarPath;

                            Sprite avatarSprite = Resources.Load<Sprite>(fullPath);

                            if (avatarSprite == null)
                            {
                                avatarSprite = Resources.Load<Sprite>("Portrait/DefaultAvatar");
                            }

                            var slot = Instantiate(_slotRankingBossUIPrefab, _container);
                            slot.SetData(userBossLocal.Top, userBossLocal.NameUser, userBossLocal.Score, avatarSprite);
                            _slots.Add(slot);
                        }
                        else
                        {
                            var slot = Instantiate(_slotRankingBossUIPrefab, _container);
                            slot.SetData(userBossLocal.Top, userBossLocal.NameUser, userBossLocal.Score, Resources.Load<Sprite>("Portrait/DefaultAvatar"));
                            _slots.Add(slot);
                        }
                    }
                }, error =>
                {
                    Debug.LogError($"Error fetching user data for {userBossLocal.MasterIDUser}: {error.GenerateErrorReport()}");
                });
            }
        }

        private void SaveScoreUser()
        {
            PlayerPrefs.SetInt(ScoreUserKey, ScoreUser);
            PlayerPrefs.Save();
        }

        private void LoadScoreUser()
        {
            ScoreUser = PlayerPrefs.GetInt(ScoreUserKey, 0);
        }
    }

    [Serializable]
    public class UserBoss
    {
        public string MasterIDUser;
        public int Top;
        public string NameUser;
        public int Score;
    }
}
