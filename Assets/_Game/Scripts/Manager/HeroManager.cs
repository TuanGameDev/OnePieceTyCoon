using _Game.Scripts.Scriptable_Object;
using PlayFab.ClientModels;
using PlayFab;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Game.Scripts.Characters;
using _Game.Scripts.UI;
using _Game.Scripts.Non_Mono;

namespace _Game.Scripts.Manager
{
    public class HeroManager : MonoBehaviour
    {
        [SerializeField]
        public HeroDictionary _heroDictionary;

        public CharOutLook CharOutLook;

        public List<HeroDataList> HeroesReady = new List<HeroDataList>();

        public List<HeroDataList> HeroesAvailable = new List<HeroDataList>();

        [SerializeField]
        private HeroesUI _heroesUI;

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

                    if (_heroesUI != null)
                    {
                        _heroesUI.DisplayHeroes(heroDataList.heroes);
                    }

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


        [Button("Add Hero")]
        public void AddHero()
        {
            if (_heroDictionary == null || _heroDictionary.Count == 0)
            {
                Debug.LogError("HeroDictionary is empty or not set.");
                return;
            }
            if (HeroesAvailable.Count == 0)
            {
                HeroesAvailable.Add(new HeroDataList { heroes = new List<HeroData>() });
            }
            var randomIndex = Random.Range(0, _heroDictionary.Count);
            var randomHeroDataSO = _heroDictionary.Values.ElementAt(randomIndex);

            HeroData heroData = HeroDataConverter.ConvertHeroDataSOToHeroData(randomHeroDataSO);

            HeroesAvailable[0].heroes.Add(heroData);

            SaveHeroData(randomHeroDataSO);

            if (_heroesUI != null)
            {
                _heroesUI.DisplayHeroes(HeroesAvailable[0].heroes);
            }
        }

        public void SaveHeroData(HeroDataSO heroDataSO)
        {
            if (string.IsNullOrEmpty(PlayFabManager.Instance.PlayFabId))
            {
                Debug.LogError("PlayFab ID chưa được thiết lập.");
                return;
            }

            HeroData heroData = HeroDataConverter.ConvertHeroDataSOToHeroData(heroDataSO);

            if (HeroesAvailable != null && HeroesAvailable.Count > 0)
            {
                var heroDataList = HeroesAvailable[0];
                var heroDataListJson = JsonUtility.ToJson(heroDataList);

                PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string> { { "HeroData", heroDataListJson } }
                },
                result =>
                {
                    Debug.Log("Dữ liệu hero đã được lưu thành công!");
                },
                error =>
                {
                    Debug.LogError("Lỗi khi lưu dữ liệu hero: " + error.ErrorMessage);
                });
            }
            else
            {
                Debug.LogWarning("Chưa có hero nào trong danh sách HeroesAvailable.");
            }
        }
    }
    [System.Serializable]
    public class HeroDataList
    {
        public List<HeroData> heroes;
    }
}
