using _Game.Scripts.Scriptable_Object;
using PlayFab.ClientModels;
using PlayFab;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Character.Hero;
using _Game.Scripts.Helper;
using System;
using UnityEngine.Events;

namespace _Game.Scripts.Manager
{
    public class HeroManager : MonoBehaviour
    {
        public HeroDictionary HeroCommonDictionary;
        public HeroDictionary HeroLegendDictionary;

        public List<HeroDataList> HeroesAvailable = new List<HeroDataList>();

        public Action OnAddHero;
        public Action OnRemoveHero;

        public int MaxHeroSlot;

        public static HeroManager Instance;

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

        private void Start()
        {
            LoadDataHero();
        }

        [Button("AddHero")]
        public void AddHero()
        {
            if (HeroesAvailable.Count == 0)
            {
                HeroesAvailable.Add(new HeroDataList { heroes = new List<HeroData>() });
            }

            var availableHeroList = HeroesAvailable[0].heroes;

            // Kiểm tra nếu danh sách đã đầy
            if (availableHeroList.Count >= MaxHeroSlot)
            {
                Debug.LogWarning("Hero đã đầy! Không thể thêm hero mới.");
                return;
            }

            foreach (var heroEntry in HeroCommonDictionary)
            {
                var heroDataSO = heroEntry.Value;

                HeroData newHeroData = new HeroData
                {
                    HeroID = heroDataSO.HeroID,
                    HeroAvatar = heroDataSO.HeroAvatar,
                    CharacterStat = heroDataSO.CharacterStat,
                    Rarity = heroDataSO.Rarity,
                    Elemental = heroDataSO.Elemental,
                    CharacterName = heroDataSO.CharacterName,
                    Power = heroDataSO.Power,
                    IconAvatarPath = "Portrait/" + heroDataSO.IconAvatar.name,
                    HeroAvatarPath = "Artworks/" + heroDataSO.HeroAvatar.name
                };

                availableHeroList.Add(newHeroData);
                OnAddHero?.Invoke();

                if (availableHeroList.Count >= MaxHeroSlot)
                {
                    Debug.LogWarning("Danh sách hero đã đạt giới hạn sau khi thêm hero mới.");
                    break;
                }
            }

            SaveDataHero();
        }

        [Button("RemoveHero")]
        public void RemoveHero()
        {
            if (HeroesAvailable.Count == 0)
            {
                HeroesAvailable.Add(new HeroDataList { heroes = new List<HeroData>() });
            }

            var availableHeroList = HeroesAvailable[0].heroes;

            foreach (var heroEntry in HeroCommonDictionary)
            {
                var heroDataSO = heroEntry.Value;

                HeroData heroToRemove = availableHeroList.Find(hero => hero.HeroID == heroDataSO.HeroID);

                if (heroToRemove != null)
                {
                    availableHeroList.Remove(heroToRemove);
                    OnRemoveHero?.Invoke();
                }
            }

            SaveDataHero();
        }

        public void LoadDataHero()
        {
            if (string.IsNullOrEmpty(RankingManager.Instance.UserInformation.MasterPlayerID))
            {
                Debug.LogError("PlayFab ID chưa được thiết lập.");
                return;
            }

            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = RankingManager.Instance.UserInformation.MasterPlayerID,
                Keys = null
            },
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("HeroData"))
                {
                    string heroDataJson = result.Data["HeroData"].Value;
                    HeroDataList heroDataList = JsonUtility.FromJson<HeroDataList>(heroDataJson);

                    foreach (var hero in heroDataList.heroes)
                    {
                        hero.IconAvatar = Resources.Load<Sprite>(hero.IconAvatarPath);
                        hero.HeroAvatar = Resources.Load<Sprite>(hero.HeroAvatarPath);
                    }

                    HeroesAvailable.Clear();
                    HeroesAvailable.Add(heroDataList);
                    Debug.Log("Dữ liệu hero đã được tải thành công!");
                }
            },
            error =>
            {
                Debug.LogError("Lỗi khi tải dữ liệu hero: " + error.ErrorMessage);
            });
        }

        public void SaveDataHero()
        {
            if (string.IsNullOrEmpty(RankingManager.Instance.UserInformation.MasterPlayerID))
            {
                Debug.LogError("PlayFab ID chưa được thiết lập.");
                return;
            }

            if (HeroesAvailable.Count == 0)
            {
                return;
            }

            HeroDataList heroDataList = HeroesAvailable[0];
            string heroDataJson = JsonUtility.ToJson(heroDataList);

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string> { { "HeroData", heroDataJson } }
            },
            result =>
            {
                Debug.Log("Dữ liệu hero đã được lưu thành công lên PlayFab!");
            },
            error =>
            {
                Debug.LogError("Lỗi khi lưu dữ liệu hero: " + error.ErrorMessage);
            });
        }

        public List<HeroData> GetAvailableHeroes()
        {
            if (HeroesAvailable.Count > 0)
            {
                return HeroesAvailable[0].heroes;
            }
            return new List<HeroData>();
        }
    }

    [System.Serializable]
    public class HeroDataList
    {
        public List<HeroData> heroes;
    }
}
