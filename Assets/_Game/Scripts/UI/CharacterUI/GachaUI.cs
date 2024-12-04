using _Game.Scripts.Characters;
using _Game.Scripts.Enums;
using _Game.Scripts.Helper;
using _Game.Scripts.Manager;
using _Game.Scripts.Non_Mono;
using _Game.Scripts.Scriptable_Object;
using _Game.Scripts.UI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace _Game.Scripts.UI
{
    enum StateGacha
    {
        Beli,
        Diamond
    }
    public class GachaUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private BoxHeroDictionary _boxBeliDictionary;

        [SerializeField]
        private BoxHeroDictionary _boxDiamondDictionary;

        [SerializeField]
        private StateGachaAndIcon _stateGachaAndIcon;

        [SerializeField, ReadOnly]
        private StateGacha _stateGacha = StateGacha.Beli;

        [Header("UI")]
        [SerializeField]
        private SlotGachaUI _slotGachaUI;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private GameObject _panelListHero;

        [SerializeField]
        private Button _gachaCommonBtn, _gachaLegendBtn, _getHeroBtn, _helpBtn,_x1Btn,_x10Btn;

        [SerializeField]
        private GameObject _commonPopupUI, _legendPopupUI, _introVideoGacha, _helpPopupUI;

        [SerializeField]
        private Image _iconX1, _iconX10;

        [SerializeField]
        private TextMeshProUGUI _nameBoxTxt, _messageTxt, _beliTxt, _diamondTxt;

        [SerializeField]
        private List<HeroData> _gachaHeroes = new List<HeroData>();

        [SerializeField]
        private RankingManager _rankingManager;

        [SerializeField]
        private VideoPlayerFromStreamingAssets _videoPlayerFSA;

        private void Awake()
        {
            if (_rankingManager == null)
            {
                _rankingManager = FindObjectOfType<RankingManager>();
            }

            _gachaCommonBtn.onClick.AddListener(() => SetGachaState(StateGacha.Beli));
            _gachaLegendBtn.onClick.AddListener(() => SetGachaState(StateGacha.Diamond));
            _getHeroBtn.onClick.AddListener(GetHero);
        }

        private void OnEnable()
        {
            RankingManager.Instance.UpdateBeliAndDiamondText(_beliTxt, _diamondTxt);
            _videoPlayerFSA.videoPlayer.loopPointReached += OnVideoEnd;
        }

        private void OnDisable()
        {
            _videoPlayerFSA.videoPlayer.loopPointReached -= OnVideoEnd;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerPress == _helpBtn.gameObject) _helpPopupUI.SetActive(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerPress == _helpBtn.gameObject) _helpPopupUI.SetActive(false);
        }

        private void SetGachaState(StateGacha state)
        {
            _stateGacha = state;

            if (state == StateGacha.Beli)
            {
                _commonPopupUI.SetActive(true);
                _legendPopupUI.SetActive(false);
                _gachaCommonBtn.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                _gachaLegendBtn.transform.localScale = Vector3.one;
            }
            else if (state == StateGacha.Diamond)
            {
                _commonPopupUI.SetActive(false);
                _legendPopupUI.SetActive(true);
                _gachaCommonBtn.transform.localScale = Vector3.one;
                _gachaLegendBtn.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }

            UpdateGachaIcons();
            _nameBoxTxt.text = "Box " + _stateGacha.ToString();
        }

        private void UpdateGachaIcons()
        {
            if (_stateGachaAndIcon.TryGetValue(_stateGacha, out Sprite icon))
            {
                _iconX1.sprite = icon;
                _iconX10.sprite = icon;
            }
        }

        private void GetHero()
        {
            if (_gachaHeroes.Count == 0)
            {
                Debug.LogWarning("Không có hero nào để thêm.");
                return;
            }
            foreach (var hero in _gachaHeroes)
            {
                HeroManager.Instance.AddHero(hero);
            }

            _gachaHeroes.Clear();

            _panelListHero.SetActive(false);
            _x1Btn.interactable = true;
            _x10Btn.interactable = true;
            foreach (Transform child in _container)
            {
                Destroy(child.gameObject);
            }
            HeroesUI.Instance.UpdateHeroSlotText();
            UserManagerUI.Instance.UpdateDisplayUser();
        }


        public void GachaX1(int amount)
        {
            if (HeroManager.Instance.GetAvailableHeroes().Count >= HeroManager.Instance.MaxHeroSlot)
            {
                _messageTxt.text = "Hero is complete. Unable to execute Gacha x1.";
                _messageTxt.color = Color.red;
                StartCoroutine(HideTxt(1f));
                return;
            }

            if ((_stateGacha == StateGacha.Beli && _rankingManager.UserInformation.Beli >= amount) ||
                (_stateGacha == StateGacha.Diamond && _rankingManager.UserInformation.Diamond >= amount))
            {
                if (_stateGacha == StateGacha.Beli)
                {
                    _rankingManager.UserInformation.Beli -= amount;
                    AddHeroToGachaListNormal();
                }
                else if (_stateGacha == StateGacha.Diamond)
                {
                    _rankingManager.UserInformation.Diamond -= amount;
                    AddHeroToGachaListLegend();
                }
                RankingManager.Instance.UpdateBeliAndDiamondText(_beliTxt, _diamondTxt);
                HeroesUI.Instance.UpdateHeroSlotText();
                _introVideoGacha.SetActive(true);
                //_videoPlayerFSA.videoPlayer.Play();
                UserManagerUI.Instance.SaveUserInformation();
            }
            else
            {
                _messageTxt.gameObject.SetActive(true);
                _messageTxt.text = "Not enough money to do Gacha x1.";
                _messageTxt.color = Color.red;
                StartCoroutine(HideTxt(1f));
                return;
            }
        }

        public void GachaX10(int amount)
        {
            int totalAmount = amount * 10;

            if (HeroManager.Instance.GetAvailableHeroes().Count + 10 > HeroManager.Instance.MaxHeroSlot)
            {
                _messageTxt.text = "Hero is complete. Unable to execute Gacha x10.";
                _messageTxt.color = Color.red;
                StartCoroutine(HideTxt(1f));
                return;
            }

            if ((_stateGacha == StateGacha.Beli && _rankingManager.UserInformation.Beli >= totalAmount) ||
                (_stateGacha == StateGacha.Diamond && _rankingManager.UserInformation.Diamond >= totalAmount))
            {
                if (_stateGacha == StateGacha.Beli)
                {
                    _rankingManager.UserInformation.Beli -= totalAmount;
                    for (int i = 0; i < 10; i++)
                    {
                        AddHeroToGachaListNormal();
                    }
                }
                else if (_stateGacha == StateGacha.Diamond)
                {
                    _rankingManager.UserInformation.Diamond -= totalAmount;
                    for (int i = 0; i < 10; i++)
                    {
                        AddHeroToGachaListLegend();
                    }
                }
                RankingManager.Instance.UpdateBeliAndDiamondText(_beliTxt,_diamondTxt);
                HeroesUI.Instance.UpdateHeroSlotText();
                _introVideoGacha.SetActive(true);
                //_videoPlayerFSA.videoPlayer.Play();
                UserManagerUI.Instance.SaveUserInformation();
            }
            else
            {
                _messageTxt.gameObject.SetActive(true);
                _messageTxt.text = "Not enough money to do Gacha x10.";
                _messageTxt.color = Color.red;
                StartCoroutine(HideTxt(1f));
                return;
            }
        }


        private void OnVideoEnd(VideoPlayer vp)
        {
            _introVideoGacha.SetActive(false);
            _panelListHero.SetActive(true);
            _getHeroBtn.gameObject.SetActive(false);
            StartCoroutine(InstantiateHeroesWithDelay());
            _x1Btn.interactable = false;
            _x10Btn.interactable = false;
        }

        private IEnumerator InstantiateHeroesWithDelay()
        {
            int totalHeroes = _gachaHeroes.Count;

            for (int i = 0; i < totalHeroes; i++)
            {
                var hero = _gachaHeroes[i];
                var slotHero = Instantiate(_slotGachaUI, _container);
                slotHero.SetHeroUI(hero.IconAvatarPath, hero);

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
            foreach (var hero in _boxBeliDictionary.Values)
            {
                if (hero.Rarity == randomRarity)
                {
                    heroesWithRarity.Add(hero);
                }
            }

            if (heroesWithRarity.Count > 0)
            {
                HeroDataSO selectedHeroDataSO = heroesWithRarity[UnityEngine.Random.Range(0, heroesWithRarity.Count)];

                CharacterStat characterStatClone = selectedHeroDataSO.CharacterStat.Clone();

                List<LevelStats> levelStatsClone = new List<LevelStats>();
                foreach (var levelStat in selectedHeroDataSO.LevelStats)
                {
                    levelStatsClone.Add(new LevelStats
                    {
                        StatLevel = levelStat.StatLevel.Clone()
                    });
                }

                HeroData newHeroData = new HeroData
                {
                    HeroID = selectedHeroDataSO.HeroID,
                    HeroName = selectedHeroDataSO.HeroName,
                    HeroAvatar = selectedHeroDataSO.HeroAvatar,
                    CharacterStat = characterStatClone,
                    HeroType = selectedHeroDataSO.HeroType,
                    Rarity = randomRarity,
                    Elemental = selectedHeroDataSO.Elemental,
                    Power = selectedHeroDataSO.Power,
                    IconAvatarPath = $"Portrait/{selectedHeroDataSO.IconAvatar.name}",
                    HeroAvatarPath = $"Artworks/{selectedHeroDataSO.HeroAvatar.name}",
                    LevelStats = levelStatsClone
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
            var randomValue = UnityEngine.Random.Range(0, 100);
            Debug.Log($"Tỉ lệ Gacha Common:{randomValue}%");

            if (randomValue < 15)
            {
                return Rarity.A;
            }
            else
            {
                var rarities = new List<Rarity> { Rarity.B, Rarity.C };
                return rarities[UnityEngine.Random.Range(0, rarities.Count)];
            }
        }

        #endregion

        #region RateGachaLegend

        private void AddHeroToGachaListLegend()
        {
            Rarity randomRarity = GetRandomRarityLegend();

            var heroesWithRarity = new List<HeroDataSO>();
            foreach (var hero in _boxDiamondDictionary.Values)
            {
                if (hero.Rarity == randomRarity)
                {
                    heroesWithRarity.Add(hero);
                }
            }

            if (heroesWithRarity.Count > 0)
            {
                HeroDataSO selectedHeroDataSO = heroesWithRarity[UnityEngine.Random.Range(0, heroesWithRarity.Count)];

                CharacterStat characterStatClone = selectedHeroDataSO.CharacterStat.Clone();

                List<LevelStats> levelStatsClone = new List<LevelStats>();
                foreach (var levelStat in selectedHeroDataSO.LevelStats)
                {
                    levelStatsClone.Add(new LevelStats
                    {
                        StatLevel = levelStat.StatLevel.Clone()
                    });
                }


                HeroData newHeroData = new HeroData
                {
                    HeroID = selectedHeroDataSO.HeroID,
                    HeroName = selectedHeroDataSO.HeroName,
                    HeroAvatar = selectedHeroDataSO.HeroAvatar,
                    CharacterStat = characterStatClone,
                    HeroType = selectedHeroDataSO.HeroType,
                    Rarity = randomRarity,
                    Elemental = selectedHeroDataSO.Elemental,
                    Power = selectedHeroDataSO.Power,
                    IconAvatarPath = $"Portrait/{selectedHeroDataSO.IconAvatar.name}",
                    HeroAvatarPath = $"Artworks/{selectedHeroDataSO.HeroAvatar.name}",
                    LevelStats = levelStatsClone
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
            Debug.Log($"Tỉ lệ Gacha Legend:{randomValue}%");

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

        private IEnumerator HideTxt(float delay)
        {
            yield return new WaitForSeconds(delay);
            _messageTxt.gameObject.SetActive(false);
        }
    }
}
[System.Serializable]
internal class StateGachaAndIcon : UnitySerializedDictionary<StateGacha, Sprite>
{
   
}
