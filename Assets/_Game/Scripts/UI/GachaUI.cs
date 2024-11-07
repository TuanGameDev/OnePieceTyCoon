using _Game.Scripts.Enums;
using _Game.Scripts.Manager;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Scriptable_Object;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace _Game.Scripts.UI
{
    enum StateGacha
    {
        Normal,
        Legend
    }

    public class GachaUI : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private StateGacha _stateGacha = StateGacha.Normal;

        [Header("UI")]
        [SerializeField]
        private SlotGachaUI _slotGachaUI;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private GameObject _panelListHero;

        [SerializeField]
        private Button _gachaNormalBtn, _gachaLegendBtn, _getHeroBtn;

        [SerializeField]
        private GameObject _normalPopupUI, _legendPopupUI, _introVideoGacha;

        [SerializeField]
        private VideoPlayer _videoPlayer;

        [SerializeField]
        private List<HeroData> _gachaHeroes = new List<HeroData>();

        [SerializeField]
        private RankingManager _rankingManager;

        private void Awake()
        {
            if (_rankingManager == null)
            {
                _rankingManager = FindObjectOfType<RankingManager>();
            }

            _gachaNormalBtn.onClick.AddListener(() => SetGachaState(StateGacha.Normal));
            _gachaLegendBtn.onClick.AddListener(() => SetGachaState(StateGacha.Legend));
            _getHeroBtn.onClick.AddListener(GetHero);
        }

        private void OnEnable()
        {
            _videoPlayer.loopPointReached += OnVideoEnd;
        }

        private void OnDisable()
        {
            _videoPlayer.loopPointReached -= OnVideoEnd;
        }

        private void SetGachaState(StateGacha state)
        {
            _stateGacha = state;

            if (state == StateGacha.Normal)
            {
                _normalPopupUI.SetActive(true);
                _legendPopupUI.SetActive(false);
                _gachaNormalBtn.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                _gachaLegendBtn.transform.localScale = Vector3.one;
            }
            else if (state == StateGacha.Legend)
            {
                _normalPopupUI.SetActive(false);
                _legendPopupUI.SetActive(true);
                _gachaNormalBtn.transform.localScale = Vector3.one;
                _gachaLegendBtn.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
        }

        private void GetHero()
        {
            foreach (var hero in _gachaHeroes)
            {
                HeroManager.Instance.HeroesAvailable[0].heroes.Add(hero);
            }

            _gachaHeroes.Clear();

            _panelListHero.SetActive(false);
            foreach (Transform child in _container)
            {
                Destroy(child.gameObject);
            }
            HeroManager.Instance.SaveDataHero();
            UserManagerUI.Instance.UpdateCombatPowerDisplay();
        }

        public void GachaX1(int amount)
        {
            if ((_stateGacha == StateGacha.Normal && _rankingManager.UserInformation.Beli >= amount) ||
                (_stateGacha == StateGacha.Legend && _rankingManager.UserInformation.Diamond >= amount))
            {
                if (_stateGacha == StateGacha.Normal)
                {
                    _rankingManager.UserInformation.Beli -= amount;
                    AddHeroToGachaListNormal();
                }
                else if (_stateGacha == StateGacha.Legend)
                {
                    _rankingManager.UserInformation.Diamond -= amount;
                    AddHeroToGachaListLegend();
                }

                _introVideoGacha.SetActive(true);
                _videoPlayer.Play();
                UserManagerUI.Instance.SaveUserInformation();
            }
            else
            {
                Debug.Log("Không đủ tiền để thực hiện Gacha X1.");
                return;
            }
        }

        public void GachaX10(int amount)
        {
            int totalAmount = amount * 10;

            if ((_stateGacha == StateGacha.Normal && _rankingManager.UserInformation.Beli >= totalAmount) ||
                (_stateGacha == StateGacha.Legend && _rankingManager.UserInformation.Diamond >= totalAmount))
            {
                if (_stateGacha == StateGacha.Normal)
                {
                    _rankingManager.UserInformation.Beli -= totalAmount;
                    for (int i = 0; i < 10; i++)
                    {
                        AddHeroToGachaListNormal();
                    }
                }
                else if (_stateGacha == StateGacha.Legend)
                {
                    _rankingManager.UserInformation.Diamond -= totalAmount;
                    for (int i = 0; i < 10; i++)
                    {
                        AddHeroToGachaListLegend();
                    }
                }

                _introVideoGacha.SetActive(true);
                _videoPlayer.Play();
                UserManagerUI.Instance.SaveUserInformation();
            }
            else
            {
                Debug.Log("Không đủ tiền để thực hiện Gacha X10.");
                return;
            }
        }

        private void OnVideoEnd(VideoPlayer vp)
        {
            _introVideoGacha.SetActive(false);
            _panelListHero.SetActive(true);
            _getHeroBtn.gameObject.SetActive(false);
            StartCoroutine(InstantiateHeroesWithDelay());
        }

        private IEnumerator InstantiateHeroesWithDelay()
        {
            int totalHeroes = _gachaHeroes.Count;

            for (int i = 0; i < totalHeroes; i++)
            {
                var hero = _gachaHeroes[i];
                var slotHero = Instantiate(_slotGachaUI, _container);
                slotHero.SetHeroUI(hero.IconAvatarPath);

                if (totalHeroes == 1 && i == 0)
                {
                    _getHeroBtn.gameObject.SetActive(true);
                }

                yield return new WaitForSeconds(0.5f);
            }
            if (totalHeroes == 10)
            {
                _getHeroBtn.gameObject.SetActive(true);
            }
        }


        #region RateGachaNormal

        private void AddHeroToGachaListNormal()
        {
            Rarity randomRarity = GetRandomRarityNormal();

            var heroesWithRarity = new List<HeroDataSO>();
            foreach (var hero in HeroManager.Instance.HeroNormalDictionary.Values)
            {
                if (hero.Rarity == randomRarity)
                {
                    heroesWithRarity.Add(hero);
                }
            }

            if (heroesWithRarity.Count > 0)
            {
                HeroDataSO selectedHeroDataSO = heroesWithRarity[UnityEngine.Random.Range(0, heroesWithRarity.Count)];

                HeroData newHeroData = new HeroData
                {
                    HeroID = selectedHeroDataSO.HeroID,
                    HeroAvatar = selectedHeroDataSO.HeroAvatar,
                    CharacterStat = selectedHeroDataSO.CharacterStat,
                    Rarity = randomRarity,
                    Elemental = selectedHeroDataSO.Elemental,
                    CharacterName = selectedHeroDataSO.CharacterName,
                    Power = selectedHeroDataSO.Power,
                    IconAvatarPath = "Portrait/" + selectedHeroDataSO.IconAvatar.name,
                    HeroAvatarPath = "Artworks/" + selectedHeroDataSO.HeroAvatar.name
                };

                _gachaHeroes.Add(newHeroData);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy hero nào với rarity được chọn trong HeroNormalDictionary.");
            }
        }

        public Rarity GetRandomRarityNormal()
        {
            var rarities = new List<Rarity> { Rarity.A, Rarity.B, Rarity.C, Rarity.D };
            return rarities[UnityEngine.Random.Range(0, rarities.Count)];
        }

        #endregion

        #region RateGachaLegend

        private void AddHeroToGachaListLegend()
        {
            Rarity randomRarity = GetRandomRarityLegend();

            var heroesWithRarity = new List<HeroDataSO>();
            foreach (var hero in HeroManager.Instance.HeroLegendDictionary.Values)
            {
                if (hero.Rarity == randomRarity)
                {
                    heroesWithRarity.Add(hero);
                }
            }

            if (heroesWithRarity.Count > 0)
            {
                HeroDataSO selectedHeroDataSO = heroesWithRarity[UnityEngine.Random.Range(0, heroesWithRarity.Count)];

                HeroData newHeroData = new HeroData
                {
                    HeroID = selectedHeroDataSO.HeroID,
                    HeroAvatar = selectedHeroDataSO.HeroAvatar,
                    CharacterStat = selectedHeroDataSO.CharacterStat,
                    Rarity = randomRarity,
                    Elemental = selectedHeroDataSO.Elemental,
                    CharacterName = selectedHeroDataSO.CharacterName,
                    Power = selectedHeroDataSO.Power,
                    IconAvatarPath = "Portrait/" + selectedHeroDataSO.IconAvatar.name,
                    HeroAvatarPath = "Artworks/" + selectedHeroDataSO.HeroAvatar.name
                };

                _gachaHeroes.Add(newHeroData);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy hero nào với rarity được chọn trong HeroLegendDictionary.");
            }
        }

        public Rarity GetRandomRarityLegend()
        {
            int randomValue = UnityEngine.Random.Range(0, 100);
            Debug.Log("Tỉ lệ Gacha:" + randomValue);

            if (randomValue < 1)
            {
                return Rarity.SS;
            }
            else if (randomValue < 10)
            {
                return Rarity.S;
            }
            else
            {
                return Rarity.A;
            }
        }

        #endregion

    }
}
