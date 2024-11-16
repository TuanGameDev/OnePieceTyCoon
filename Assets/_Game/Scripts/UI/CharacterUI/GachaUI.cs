using _Game.Scripts.Enums;
using _Game.Scripts.Helper;
using _Game.Scripts.Manager;
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
        Common,
        Legend
    }
    public class GachaUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private StateGachaAndIcon _stateGachaAndIcon;

        [SerializeField, ReadOnly]
        private StateGacha _stateGacha = StateGacha.Common;

        [Header("UI")]
        [SerializeField]
        private SlotGachaUI _slotGachaUI;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private GameObject _panelListHero;

        [SerializeField]
        private Button _gachaCommonBtn, _gachaLegendBtn, _getHeroBtn, _helpBtn;

        [SerializeField]
        private GameObject _commonPopupUI, _legendPopupUI, _introVideoGacha, _helpPopupUI;

        [SerializeField]
        private Image _iconX1, _iconX10;

        [SerializeField]
        private VideoPlayer _videoPlayer;

        [SerializeField]
        private TextMeshProUGUI _nameBoxTxt, _messageTxt, _beliTxt, _diamondTxt;

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

            _gachaCommonBtn.onClick.AddListener(() => SetGachaState(StateGacha.Common));
            _gachaLegendBtn.onClick.AddListener(() => SetGachaState(StateGacha.Legend));
            _getHeroBtn.onClick.AddListener(GetHero);
            Invoke(nameof(UpdateBeliAndDiamond), 1f);
        }

        private void OnEnable()
        {
            _videoPlayer.loopPointReached += OnVideoEnd;
        }

        private void OnDisable()
        {
            _videoPlayer.loopPointReached -= OnVideoEnd;
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

            if (state == StateGacha.Common)
            {
                _commonPopupUI.SetActive(true);
                _legendPopupUI.SetActive(false);
                _gachaCommonBtn.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                _gachaLegendBtn.transform.localScale = Vector3.one;
            }
            else if (state == StateGacha.Legend)
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
            HeroesUI.Instance.UpdateHeroSlotText();
            UserManagerUI.Instance.UpdateCombatPowerDisplay();
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

            if ((_stateGacha == StateGacha.Common && _rankingManager.UserInformation.Beli >= amount) ||
                (_stateGacha == StateGacha.Legend && _rankingManager.UserInformation.Diamond >= amount))
            {
                if (_stateGacha == StateGacha.Common)
                {
                    _rankingManager.UserInformation.Beli -= amount;
                    AddHeroToGachaListNormal();
                }
                else if (_stateGacha == StateGacha.Legend)
                {
                    _rankingManager.UserInformation.Diamond -= amount;
                    AddHeroToGachaListLegend();
                }
                UpdateBeliAndDiamond();
                HeroesUI.Instance.UpdateHeroSlotText();
                _introVideoGacha.SetActive(true);
                _videoPlayer.Play();
                UserManagerUI.Instance.SaveUserInformation();
            }
            else
            {
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

            if ((_stateGacha == StateGacha.Common && _rankingManager.UserInformation.Beli >= totalAmount) ||
                (_stateGacha == StateGacha.Legend && _rankingManager.UserInformation.Diamond >= totalAmount))
            {
                if (_stateGacha == StateGacha.Common)
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
                UpdateBeliAndDiamond();
                HeroesUI.Instance.UpdateHeroSlotText();
                _introVideoGacha.SetActive(true);
                _videoPlayer.Play();
                UserManagerUI.Instance.SaveUserInformation();
            }
            else
            {
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
            foreach (var hero in HeroManager.Instance.HeroCommonDictionary.Values)
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
            var randomValue = UnityEngine.Random.Range(0, 100);
            Debug.Log("Tỉ lệ Gacha Common:" + randomValue);

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
            Debug.Log("Tỉ lệ Gacha Legend:" + randomValue);

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
            _messageTxt.text = "";
        }

        private void UpdateBeliAndDiamond()
        {
            _beliTxt.text = RankingManager.Instance.UserInformation.Beli.ToString("N0");
            _diamondTxt.text = RankingManager.Instance.UserInformation.Diamond.ToString("N0");
        }

    }
}
[System.Serializable]
internal class StateGachaAndIcon : UnitySerializedDictionary<StateGacha, Sprite>
{
   
}
