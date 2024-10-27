using _Game.Scripts.Scriptable_Object;
using PlayFab.ClientModels;
using PlayFab;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Character.Hero;

namespace _Game.Scripts.Manager
{
    public class HeroManager : MonoBehaviour
    {
        [SerializeField]
        private HeroController _heroPrefab;

        [SerializeField]
        private Transform[] _spawnPoints;

        [SerializeField]
        public HeroDictionary _heroDictionary;

        public CharOutLook CharOutLook;

        public List<HeroDataList> HeroesReady = new List<HeroDataList>();

        public List<HeroDataList> HeroesAvailable = new List<HeroDataList>();

        [SerializeField]
        private Dictionary<HeroData, GameObject> spawnedHeroes = new Dictionary<HeroData, GameObject>();

        [SerializeField]
        private Dictionary<HeroData, int> heroSpawnPoints = new Dictionary<HeroData, int>();

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
        [Button]
        public void AddHero()
        {
            if (HeroesAvailable.Count == 0)
            {
                HeroesAvailable.Add(new HeroDataList { heroes = new List<HeroData>() });
            }
            var availableHeroList = HeroesAvailable[0].heroes;

            foreach (var heroEntry in _heroDictionary)
            {
                var heroDataSO = heroEntry.Value;

                HeroData newHeroData = new HeroData
                {
                    HeroID = heroDataSO.HeroID,
                    HeroName = heroDataSO.HeroName,
                    HeroAvatar = heroDataSO.HeroAvatar,
                    CharacterStat = heroDataSO.CharacterStat,
                    Rank = heroDataSO.Rank,
                    CharacterName = heroDataSO.CharacterName,
                    HeroAvatarPath = "Portrait/" + heroDataSO.HeroAvatar.name
                };

                availableHeroList.Add(newHeroData);
            }

            SaveDataHero();
        }


        public void LoadDataHero()
        {
            if (string.IsNullOrEmpty(PlayFabManager.Instance.PlayFabId))
            {
                Debug.LogError("PlayFab ID chưa được thiết lập.");
                return;
            }

            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = PlayFabManager.Instance.PlayFabId,
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
                        Sprite heroSprite = Resources.Load<Sprite>(hero.HeroAvatarPath);

                        if (heroSprite != null)
                        {
                            hero.HeroAvatar = heroSprite;
                        }
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
            if (string.IsNullOrEmpty(PlayFabManager.Instance.PlayFabId))
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


        public List<HeroData> GetAvailableHeroesReady()
        {
            if (HeroesReady.Count > 0)
            {
                return HeroesReady[0].heroes;
            }
            return new List<HeroData>();
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
