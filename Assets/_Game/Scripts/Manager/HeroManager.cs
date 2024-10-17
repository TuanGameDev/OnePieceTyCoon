using _Game.Scripts.Scriptable_Object;
using PlayFab.ClientModels;
using PlayFab;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Game.Scripts.Characters;
using _Game.Scripts.UI;
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

        public static HeroManager Instance;

        private Dictionary<HeroData, GameObject> spawnedHeroes = new Dictionary<HeroData, GameObject>();

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

        public void SpawnHeroes()
        {
            if (HeroesReady.Count == 0)
            {
                Debug.Log("No heroes in the HeroesReady list to spawn.");
                return;
            }

            int spawnCount = 0;

            for (int i = 0; i < HeroesReady.Count; i++)
            {
                HeroDataList heroDataList = HeroesReady[i];

                foreach (HeroData heroData in heroDataList.heroes)
                {
                    if (spawnCount >= _spawnPoints.Length)
                    {
                        return;
                    }

                    if (spawnedHeroes.ContainsKey(heroData))
                    {
                        Debug.Log("Hero already spawned.");
                        continue;
                    }

                    HeroDataSO tempHeroDataSO = ScriptableObject.CreateInstance<HeroDataSO>();
                    tempHeroDataSO.HeroID = heroData.HeroID;
                    tempHeroDataSO.HeroName = heroData.HeroName;
                    tempHeroDataSO.HeroAvatar = heroData.HeroAvatar;
                    tempHeroDataSO.CharacterStat = heroData.CharacterStat;
                    tempHeroDataSO.Rank = heroData.Rank;
                    tempHeroDataSO.CharacterName = heroData.CharacterName;

                    HeroController heroInstance = Instantiate(_heroPrefab, _spawnPoints[spawnCount].position, Quaternion.identity);
                    CharacterNameAndRank key = new CharacterNameAndRank(tempHeroDataSO.CharacterName, tempHeroDataSO.Rank);

                    if (CharOutLook.CharOut.TryGetValue(key, out OutLook outLook))
                    {
                        if (outLook.Root != null)
                        {
                            heroInstance.BaseRoot = Instantiate(outLook.Root, heroInstance.ReverObject);
                            heroInstance.BaseRoot.name = outLook.Root.name;
                        }
                    }

                    heroInstance.SetHeroData(tempHeroDataSO);
                    spawnedHeroes[heroData] = heroInstance.gameObject;

                    spawnCount++;
                }
            }
        }

        public GameObject GetSpawnedHero(HeroData heroData)
        {
            if (spawnedHeroes.ContainsKey(heroData))
            {
                return spawnedHeroes[heroData];
            }
            return null;
        }

        public void RemoveSpawnedHero(HeroData heroData)
        {
            if (spawnedHeroes.ContainsKey(heroData))
            {
                Destroy(spawnedHeroes[heroData]);
                spawnedHeroes.Remove(heroData);
            }
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
                else
                {
                    Debug.Log("Không tìm thấy dữ liệu hero.");
                }
            },
            error =>
            {
                Debug.LogError("Lỗi khi tải dữ liệu hero: " + error.ErrorMessage);
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
