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
using System.Linq;
using _Game.Scripts.Characters;

namespace _Game.Scripts.Manager
{
    public class HeroManager : MonoBehaviour
    {
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

        public void AddHero(HeroData hero)
        {
            if (hero == null)
            {
                Debug.LogWarning("Hero bị null, không thể thêm.");
                return;
            }

            if (HeroesAvailable.Count == 0)
            {
                HeroesAvailable.Add(new HeroDataList { heroes = new List<HeroData>() });
            }

            var availableHeroList = HeroesAvailable[0].heroes;

            if (availableHeroList.Count >= MaxHeroSlot)
            {
                Debug.LogWarning("Hero đã đầy! Không thể thêm hero mới.");
                return;
            }
            HeroData newHero = CloneHero(hero);

            availableHeroList.Add(newHero);
            Debug.Log($"Hero {newHero.HeroName} đã được thêm.");
            OnAddHero?.Invoke();
            SaveDataHero();
        }
        private HeroData CloneHero(HeroData original)
        {
            HeroData clone = new HeroData
            {
                HeroID = original.HeroID,
                HeroName = original.HeroName,
                HeroAvatar = original.HeroAvatar,
                IconAvatar = original.IconAvatar,
                HeroAvatarPath = original.HeroAvatarPath,
                IconAvatarPath = original.IconAvatarPath,
                Power = original.Power,
                CharacterStat = CloneCharacterStat(original.CharacterStat),
                HeroType = original.HeroType,
                Rarity = original.Rarity,
                Elemental = original.Elemental,
                LevelStats = new List<LevelStats>(original.LevelStats.Count)
            };

            foreach (var levelStat in original.LevelStats)
            {
                clone.LevelStats.Add(CloneLevelStat(levelStat));
            }

            return clone;
        }
        private CharacterStat CloneCharacterStat(CharacterStat original)
        {
            if (original == null) return null;

            return new CharacterStat
            {
                HeroLevel = original.HeroLevel,
                Hp = original.Hp,
                Def = original.Def,
                MoveSpeed = original.MoveSpeed,
                AttackDamage = original.AttackDamage,
                CurrentExp = original.CurrentExp,
                ExpToLevelUp = original.ExpToLevelUp
            };
        }
        private LevelStats CloneLevelStat(LevelStats original)
        {
            if (original == null) return null;

            return new LevelStats
            {
                StatLevel = CloneCharacterStat(original.StatLevel)
            };
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
                //Debug.Log("Dữ liệu hero đã được lưu thành công lên PlayFab!");
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

        public void RefreshAvailableHeroes()
        {
            HeroesAvailable = HeroesAvailable.Where(hero => hero != null).ToList();
        }

    }

    [System.Serializable]
    public class HeroDataList
    {
        public List<HeroData> heroes;
    }
}
